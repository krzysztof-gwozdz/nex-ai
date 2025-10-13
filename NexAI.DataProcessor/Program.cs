using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataProcessor.ConsumerServices;
using NexAI.DataProcessor.Zendesk;
using NexAI.LLMs;
using NexAI.MongoDb;
using NexAI.Neo4j;
using NexAI.Qdrant;
using NexAI.RabbitMQ;
using NexAI.Zendesk;
using Spectre.Console;

Console.OutputEncoding = System.Text.Encoding.UTF8;
AnsiConsole.Write(new FigletText("Nex AI - Data Processor").Color(Color.Red3));
var cancellationTokenSource = new CancellationTokenSource();
try
{
    var options = new Options(GetConfiguration());

    using var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
            services.AddSingleton(options);
            services.AddMongoDb();
            services.AddNeo4j();
            services.AddQdrant();
            services.AddZendesk();
            services.AddRabbitMQ();
            services.AddLLM(options);
            services.AddSingleton<ZendeskTicketJsonExporter>();
            services.AddSingleton<ZendeskTicketMongoDbExporter>();
            services.AddSingleton<ZendeskTicketQdrantExporter>();
            services.AddHostedService<JsonConsumerService>();
            services.AddHostedService<MongoDbConsumerService>();
            services.AddHostedService<QdrantConsumerService>();
        })
        .Build();

    Console.WriteLine("Starting consumers...");
    await host.RunAsync();
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}
cancellationTokenSource.Cancel();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
return;

IConfigurationRoot GetConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.data_processor.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();