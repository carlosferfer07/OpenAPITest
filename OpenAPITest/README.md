AI Chart Generator (.NET + Angular)
📌 Project Overview

This solution demonstrates how to generate charts from natural language input using AI, while keeping data access secure, controlled, and deterministic.
Users describe the data they want to see (e.g. “overtime by month for 2025”), and the system returns a rendered chart without exposing the database to AI.
The project focuses on clean architecture, separation of concerns, and production-safe AI integration.

🧰 Tech Stack

Backend
ASP.NET Core Web API (.NET)
Entity Framework (InMemory provider for demo purposes)
OpenAI Responses API (Structured Outputs with JSON Schema)

Frontend
Angular (standalone components)
Angular Material
Chart.js with ng2-charts

Solution Structure
AngularApp → Frontend UI (charts, prompt input, layout)
BusinessService → AI intent handling, chart-building logic
DataConnection → EF entities, DbContext, seed data
OpenAPITest → API layer / orchestration

🤖 How AI Is Used
AI is used only to interpret user intent, never to access or query the database directly.

Flow:
User enters a natural language prompt in the UI.
Backend sends the prompt to OpenAI.
OpenAI returns a strictly validated JSON plan (AiChartPlan).
Backend maps the plan to predefined datasets.
Entity Framework queries the data.
API returns Chart.js-ready JSON to Angular.

This approach ensures:
No AI access to the database
Predictable, safe queries
Full backend control over business rules

🚀 Setup Instructions
Prerequisites
.NET SDK
Node.js
OpenAI API key
Backend Setup
Clone the repository
Set your OpenAI API key:

set OPENAI_API_KEY "sk-xxxx"


Run the API:

dotnet run
Swagger is available at:
/swagger
Frontend Setup

Navigate to the Angular project:

cd AngularApp
Install dependencies:
npm install

Run the app:
ng serve/npm start
Ensure the Angular proxy is configured to point to the API.

⚖️ Trade-offs & Future Improvements

With more time, the following improvements could be made:
Install and configure Angular ESLint to enforce consistent code style
Implement unit and integration tests for both backend and frontend
Replace string identifiers with enums end-to-end
Improve validation and feedback for ambiguous AI prompts
Support multi-series and stacked charts
Persist chart history and prompts
Replace InMemory DB with SQL Server
Add authentication and role-based data access
Add logging and observability for AI plans
Add backend input validation

🎯 Goal

To provide a clear, safe reference architecture for turning natural language into data visualisations using AI — without compromising data security, maintainability, or clarity.