# AI Chart Generator

**Natural language in, chart out, with the AI kept away from the data.**

A full-stack .NET + Angular project exploring a common business problem: letting non-technical users explore internal data (hours, overtime, absences) by asking a question in plain English, without giving an LLM access to the database.

Type *"overtime by month for 2025"* and the app renders the chart.

> Personal project, built to explore patterns for integrating LLMs safely into line-of-business applications.

---

## The problem

Business users often need quick answers from operational data, but every new chart usually means a ticket to a developer or a BI team. LLMs make a natural-language interface possible, but the naive approach (let the model write SQL) brings real risks:

- the model can read or change data it shouldn't
- the queries it writes are unpredictable and hard to audit
- business rules live in a prompt instead of in code

## The approach

The AI does exactly one thing: it **interprets intent**. Everything else stays deterministic and stays in the backend.

```
User prompt
   │
   ▼
OpenAI (Structured Outputs + JSON Schema)
   │   returns a validated ChartSpec, never SQL
   ▼
Backend maps ChartSpec.Dataset to a predefined query
   │   unknown datasets are rejected
   ▼
Entity Framework runs the query
   │
   ▼
Chart.js-ready JSON  →  Angular renders the chart
```

The contract between the AI and the backend is a small, strict plan:

```csharp
public class ChartSpec
{
    public string Dataset { get; set; }    // overtime_by_month | hours_by_employee | absences_by_week
    public string ChartType { get; set; }  // bar | line | pie
    public string Title { get; set; }
    public int? Year { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? EmployeeId { get; set; }
}
```

What this gives you:

- **No AI access to the database.** The model never sees a connection string, a schema, or a row of data.
- **Predictable queries.** Every possible query is written and reviewed in code.
- **Business rules stay in the backend.** Adding a new chart is a new dataset and a new builder method, not prompt tweaking.
- **Safe failure.** If the model returns something unexpected, the API returns a 400 instead of running it.

## Tech stack

| Layer    | Technology |
|----------|------------|
| API      | ASP.NET Core Web API, Swagger |
| AI       | OpenAI Responses API with Structured Outputs (JSON Schema) |
| Data     | Entity Framework Core (InMemory provider, seeded demo data) |
| Frontend | Angular (standalone components), Angular Material, Chart.js via ng2-charts |

## Solution structure

```
OpenAPITest/       API layer: controllers, DI wiring, orchestration
BusinessService/   AI intent handling (ChartAiService) and chart building (ChartBuilderService)
DataConnection/    EF Core entities, DbContext, seed data
AngularApp/        Frontend: prompt input, chart rendering, layout
```

Each layer has one job. The API layer doesn't know about OpenAI, and the AI service doesn't know about EF.

## API

`POST /api/ChartGeneration/generate-from-prompt`

```json
{ "prompt": "overtime by month for 2025" }
```

Returns labels and datasets ready to pass straight into Chart.js. Full docs are in Swagger at `/swagger`.

## Running it locally

**Prerequisites:** .NET SDK, Node.js, an OpenAI API key.

**Backend**

```bash
cd OpenAPITest
dotnet user-secrets set "OpenAI:ApiKey" "sk-xxxx"   # or set OpenAI__ApiKey as an env var
dotnet run
```

The API won't start without a key, so a missing one shows up right away.

**Frontend**

```bash
cd AngularApp/generating-charts-app
npm install
npm start
```

The Angular dev proxy forwards `/api` calls to the backend.

## Trade-offs and next steps

Scope was deliberately kept small to focus on the architecture. Natural next steps:

- Unit and integration tests, starting with the ChartSpec-to-query mapping
- Replace string identifiers with enums end to end
- Better feedback when a prompt is ambiguous or out of scope
- Multi-series and stacked charts
- Persisting chart history and prompts
- Swap the InMemory provider for SQL Server
- Authentication and role-based data access, so users only see data they're allowed to see
- Logging and observability for AI plans (which prompts produce which specs)
- ESLint for the Angular project
