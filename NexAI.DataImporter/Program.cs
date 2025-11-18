using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataImporter;
using NexAI.DataImporter.Git;
using NexAI.DataImporter.Zendesk;
using NexAI.Git;
using NexAI.MongoDb;
using NexAI.Qdrant;
using NexAI.ServiceBus;
using NexAI.Zendesk;
using Spectre.Console;

Console.OutputEncoding = System.Text.Encoding.UTF8;
AnsiConsole.Write(new FigletText("Nex AI - Data Importer").Color(Color.Red1));
var cancellationTokenSource = new CancellationTokenSource();
try
{
    using var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
            services.AddSingleton(new Options(GetConfiguration()));
            services.AddZendesk();
            services.AddGit();
            services.AddMongoDb();
            services.AddQdrant();
            services.AddRabbitMQ();
            services.AddSingleton<RabbitMQStructure>();
            services.AddSingleton<ZendeskTicketImporter>();
            services.AddSingleton<ZendeskUserAndGroupsImporter>();
            services.AddSingleton<GitImporter>();
        })
        .Build();

    await host.Services.GetRequiredService<RabbitMQStructure>().Create(cancellationTokenSource.Token);
    await host.Services.GetRequiredService<ZendeskTicketImporter>().Import(cancellationTokenSource.Token);
    await host.Services.GetRequiredService<ZendeskUserAndGroupsImporter>().Import(cancellationTokenSource.Token);
    await host.Services.GetRequiredService<GitImporter>().Import(cancellationTokenSource.Token);
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
    .AddJsonFile("appsettings.data_importer.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();