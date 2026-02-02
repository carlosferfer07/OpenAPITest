using BusinessService.Interfaces;
using BusinessService.Models;
using BusinessService.Services;
using DataConnection.DBContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var openAiSection = builder.Configuration.GetSection("OpenAI");
var openAiOpts = openAiSection.Get<OpenAiOptions>() ?? new OpenAiOptions();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(openAiOpts);
// Register Business Services with HttpClient support
builder.Services.AddHttpClient<IChartAiService, ChartAiService>();
builder.Services.AddHttpClient<IChartDataService, ChartDataService>();
builder.Services.AddHttpClient<IChartBuildService, ChartBuilderService>();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("AiChartsDb")); 

var app = builder.Build();

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
