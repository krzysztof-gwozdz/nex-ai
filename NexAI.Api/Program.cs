using NexAI.Agents;
using NexAI.AzureDevOps;
using NexAI.Config;
using NexAI.LLMs;
using NexAI.MongoDb;
using NexAI.Neo4j;
using NexAI.Qdrant;
using NexAI.Zendesk;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var options = new Options(GetConfiguration());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(meterProviderBuilder => meterProviderBuilder
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter());

builder.Logging.AddOpenTelemetry(openTelemetryLoggerOptions => openTelemetryLoggerOptions
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddOtlpExporter());

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
builder.Services.AddSingleton(options);

builder.Services.AddSingleton<INexAIAgent, NexAIAgent>();
builder.Services.AddAzureDevOps();
builder.Services.AddZendesk();
builder.Services.AddMongoDb();
builder.Services.AddNeo4j();
builder.Services.AddQdrant();
builder.Services.AddLLM(options);

var app = builder.Build();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

IConfigurationRoot GetConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

#pragma warning disable ASP0027
public partial class Program; // used in tests
#pragma warning restore ASP0027