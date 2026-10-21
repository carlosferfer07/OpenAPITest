using BusinessService.Interfaces;
using BusinessService.Models;
using BusinessService.Services;
using DataConnection.DBContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Fail fast if the OpenAI key is missing, rather than finding out on the first request
builder.Services.AddOptions<OpenAiOptions>()
    .BindConfiguration("OpenAI")
    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey) && !o.ApiKey.StartsWith("{{"),
        "OpenAI:ApiKey is missing. Set it with 'dotnet user-secrets set \"OpenAI:ApiKey\" <key>' or the OpenAI__ApiKey environment variable.")
    .ValidateOnStart();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Business Services with HttpClient support
builder.Services.AddHttpClient<IChartAiService, ChartAiService>();

// Register Business Services without HttpClient support (for dependency injection)
builder.Services.AddTransient<IChartDataService, ChartDataService>();
builder.Services.AddTransient<IChartBuildService, ChartBuilderService>();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("AiChartsDb"));

var app = builder.Build();

// Unexpected errors come back as a tidy ProblemDetails JSON instead of an HTML error page
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.MapControllers();

app.Run();
