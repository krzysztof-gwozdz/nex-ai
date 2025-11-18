using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataProcessor;
using NexAI.Git;
using NexAI.LLMs;
using NexAI.MongoDb;
using NexAI.Neo4j;
using NexAI.Qdrant;
using NexAI.ServiceBus;
using NexAI.Zendesk;
using NexAI.Zendesk.MongoDb;
using NexAI.Zendesk.QdrantDb;
using Spectre.Console;

Console.OutputEncoding = System.Text.Encoding.UTF8;
AnsiConsole.Write(new FigletText("Nex AI - Data Processor").Color(Color.Red3));
var cancellationTokenSource = new CancellationTokenSource();
try
{
    var options = new Options(GetConfiguration(new ConfigurationBuilder()).Build());

    using var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((_, config) => GetConfiguration(config))
        .ConfigureServices((_, services) =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
            services.AddSingleton(options);
            services.AddMongoDb();
            services.AddNeo4j();
            services.AddQdrant();
            services.AddZendesk();
            services.AddGit();
            services.AddLLM(options);
        })
        .UseServiceBus("data_processor")
        .Build();

    var dataProcessorOptions = options.Get<DataProcessorOptions>();
    await host.Services.GetRequiredService<ZendeskMongoDbStructure>().Create(dataProcessorOptions.Recreate, cancellationTokenSource.Token);
    await host.Services.GetRequiredService<ZendeskQdrantStructure>().Create(dataProcessorOptions.Recreate, cancellationTokenSource.Token);

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

IConfigurationBuilder GetConfiguration(IConfigurationBuilder configurationBuilder) =>
    configurationBuilder
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile("appsettings.data_processor.json", optional: false)
        .AddJsonFile("appsettings.local.json", optional: true)
        .AddEnvironmentVariables();