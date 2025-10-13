using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexAI.AzureDevOps;
using NexAI.Config;
using NexAI.LLMs;
using NexAI.MCP.Tools;
using NexAI.MongoDb;
using NexAI.Neo4j;
using NexAI.Qdrant;
using NexAI.RabbitMQ;
using NexAI.Zendesk;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

var options = new Options(GetConfiguration());

builder.Services
    .AddSingleton(options)
    .AddAzureDevOps()
    .AddZendesk()
    .AddMongoDb()
    .AddNeo4j()
    .AddQdrant()
    .AddRabbitMQ()
    .AddLLM(options)
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<AzureDevOpsIssuesTools>()
    .WithTools<ZendeskTicketsTools>();

await builder.Build().RunAsync();
return;

IConfigurationRoot GetConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();