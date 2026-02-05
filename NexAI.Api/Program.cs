using NexAI.Agents;
using NexAI.Api;
using NexAI.Api.HealthChecks;
using NexAI.AzureDevOps;
using NexAI.Config;
using NexAI.LLMs;
using NexAI.MongoDb;
using NexAI.Neo4j;
using NexAI.Qdrant;
using NexAI.Zendesk;
using Scalar.AspNetCore;

var options = new Options(GetConfiguration());
var builder = WebApplication.CreateBuilder(args);
var applicationName = builder.Environment.ApplicationName;

builder.Services.AddObservability(applicationName);
builder.Logging.AddObservability(applicationName);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(options);

builder.Services.AddAgents(options);
builder.Services.AddAzureDevOps();
builder.Services.AddZendesk();
builder.Services.AddMongoDb();
builder.Services.AddNeo4j();
builder.Services.AddQdrant();
builder.Services.AddLLM(options);
builder.Services.AddHealthChecks(applicationName);

var app = builder.Build();
app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.MapControllers();
app.UseHealthChecks();
app.Run();

IConfigurationRoot GetConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

#pragma warning disable ASP0027
public partial class Program; // used in tests
#pragma warning restore ASP0027