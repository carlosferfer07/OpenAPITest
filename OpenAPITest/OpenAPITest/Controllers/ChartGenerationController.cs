using BusinessService.Interfaces;
using BusinessService.Models.Charts;
using Microsoft.AspNetCore.Mvc;

namespace OpenAPITest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartGenerationController : ControllerBase
    {

        private readonly IChartAiService _ai;
        private readonly IChartBuildService _chartBuildService;

        public ChartGenerationController(IChartAiService ai
            , IChartBuildService chartBuildService)
        {
            _ai = ai;
            _chartBuildService = chartBuildService;
        }

        /// <summary>
        /// Generates a chart specification based on a natural language prompt and builds the corresponding chart data.
        /// </summary>
        /// <param name="req">User data request</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        [HttpPost("generate-from-prompt")]
        public async Task<ActionResult<ChartSpec>> GenerateFromPrompt([FromBody] ChartRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Prompt))
                return BadRequest("Prompt is required.");

            var plan = await _ai.CreatePlanAsync(req.Prompt, ct);
                        
            if (plan == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate chart plan.");
                        
            return plan.Dataset switch
            {
                "overtime_by_month" => Ok(await _chartBuildService.BuildOvertimeByMonth(plan)),
                "hours_by_employee" => Ok(await _chartBuildService.BuildHoursByEmployee(plan)),
                "absences_by_week" => Ok(await _chartBuildService.BuildAbsencesByWeek(plan)),

                _ => BadRequest($"Unsupported dataset: {plan.Dataset}")
            };
        }
    }
}
