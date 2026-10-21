using BusinessService.Interfaces;
using BusinessService.Models;
using BusinessService.Models.Charts;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BusinessService.Services
{
    public sealed class ChartAiService : IChartAiService
    {
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _http;
        private readonly OpenAiOptions _opts;

        public ChartAiService(HttpClient http, IOptions<OpenAiOptions> opts)
        {
            _http = http;
            _opts = opts.Value;
        }

        /// <summary>
        /// This method creates a chart plan based on the provided prompt by interacting with the OpenAI API.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ChartSpec> CreatePlanAsync(string prompt, CancellationToken ct = default)
        {            
            var requestBody = new
            {
                model = _opts.Model,
                input = new object[]
                {
                new { role = "system", content =
                    """
                    You convert a user request into a chart plan for an HR/time reporting app.
                    Choose ONE dataset from the allowed list and fill the parameters.
                    Return JSON ONLY that matches the schema. No extra keys.
                    Allowed datasets:
                    - overtime_by_month
                    - hours_by_employee
                    - absences_by_week
                    """
                },
                new { role = "user", content = prompt }
                },
                text = new
                {
                    format = new
                    {
                        type = "json_schema",
                        name = "ai_chart_plan",
                        strict = true,
                        schema = new
                        {
                            type = "object",
                            additionalProperties = false,
                            properties = new
                            {
                                dataset = new { type = "string", @enum = new[] { "overtime_by_month", "hours_by_employee", "absences_by_week" } },
                                chartType = new { type = "string", @enum = new[] { "bar", "line", "pie" } },
                                title = new { type = "string" },

                                year = new { type = new[] { "integer", "null" } },
                                from = new { type = new[] { "string", "null" }, format = "date-time" },
                                to = new { type = new[] { "string", "null" }, format = "date-time" },
                                employeeId = new { type = new[] { "integer", "null" } }
                            },
                            required = new[] { "dataset", "chartType", "title", "year", "from", "to", "employeeId" }
                        }
                    }
                }
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(_opts.BaseUrl), "responses"))
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody, JsonOpts), Encoding.UTF8, "application/json")
            };

            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opts.ApiKey);

            using var res = await _http.SendAsync(req, ct);
            var json = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException($"OpenAI error {(int)res.StatusCode}: {json}");

            // The model's text output is already JSON, since we asked for a json_schema response.
            // The Web serializer defaults handle the camelCase -> PascalCase mapping for us.
            var planJson = ExtractFirstOutputText(json);
            try
            {
                return JsonSerializer.Deserialize<ChartSpec>(planJson, JsonOpts)
                       ?? throw new InvalidOperationException("The AI returned an empty chart plan.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("The AI returned a chart plan we couldn't read.", ex);
            }
        }

        /// <summary>
        /// Extracts the first text content from the "output" array in the specified JSON response.
        /// </summary>
        /// <remarks>This method parses the JSON response to locate the first "text" property within the
        /// "content" array of the "output" array. If no such text is found, or if the JSON structure does not match the
        /// expected format, an <see cref="InvalidOperationException"/> is thrown.</remarks>
        /// <param name="responseJson">A JSON string representing the response, which must contain an "output" array with nested "content" and
        /// "text" properties.</param>
        /// <returns>The first text content found in the "output" array.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the "output" array, "content" array, or "text" property is missing, or if the "text" property is
        /// empty.</exception>
        private static string ExtractFirstOutputText(string responseJson)
        {
            using var doc = JsonDocument.Parse(responseJson);

            // Response.output is an array; we want first message content text
            // (Exact shape can include multiple output items.)
            var output = doc.RootElement.GetProperty("output");

            foreach (var item in output.EnumerateArray())
            {
                if (item.TryGetProperty("content", out var contentArr))
                {
                    foreach (var content in contentArr.EnumerateArray())
                    {
                        if (content.TryGetProperty("text", out var textEl))
                        {
                            return textEl.GetString() ?? throw new InvalidOperationException("Empty output text.");
                        }
                    }
                }
            }

            throw new InvalidOperationException("Could not find output content text in response.");
        }
    }
}
