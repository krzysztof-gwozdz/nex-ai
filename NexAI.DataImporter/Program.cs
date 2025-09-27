using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexAI.Config;
using NexAI.DataImporter;
using NexAI.DataImporter.Zendesk;
using NexAI.RabbitMQ;
using NexAI.Zendesk;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI - Data Importer").Color(Color.Red1));

    using var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
            services.AddSingleton(new Options(GetConfiguration()));
            services.AddRabbitMQ();
            services.AddZendesk();
            services.AddSingleton<RabbitMQStructure>();
            services.AddSingleton<ZendeskTicketImporter>();
        })
        .Build();

    await host.Services.GetRequiredService<RabbitMQStructure>().Create();
    await host.Services.GetRequiredService<ZendeskTicketImporter>().Import();
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
    .AddJsonFile("appsettings.data_importer.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();