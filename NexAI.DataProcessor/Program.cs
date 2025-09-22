using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataProcessor.ConsumerServices;
using NexAI.DataProcessor.Zendesk;
using NexAI.RabbitMQ;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI - Data Processor").Color(Color.Red3));
    var options = new Options(GetConfiguration());

    await new ZendeskTicketJsonExporter(options).CreateSchema();
    await new ZendeskTicketMongoDbExporter(options).CreateSchema();
    await new ZendeskTicketQdrantExporter(options).CreateSchema();

    using var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
            services.AddSingleton(options);
            services.AddSingleton<RabbitMQClient>();
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

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
return;

IConfigurationRoot GetConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.data_processor.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();