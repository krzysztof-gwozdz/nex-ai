using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexAI.AzureDevOps;
using NexAI.Config;
using NexAI.Console;
using NexAI.Console.Features;
using NexAI.LLMs;
using NexAI.MongoDb;
using NexAI.Qdrant;
using NexAI.RabbitMQ;
using NexAI.Zendesk;
using Spectre.Console;

try
{
    Console.OutputEncoding = Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
    var options = new Options(GetConfiguration());

    using var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
            services.AddSingleton(options);

            services.AddAzureDevOps();
            services.AddZendesk();
            services.AddMongoDb();
            services.AddQdrant();
            services.AddRabbitMQ();
            services.AddLLM(options);

            services.AddSingleton<NexAIAgent>();
            services.AddSingleton<SummarizeZendeskTicketFeature>();
            services.AddSingleton<SearchForZendeskTicketsByPhraseFeature>();
            services.AddSingleton<SearchForAzureWorkItemsByPhraseFeature>();
            services.AddSingleton<SearchForInfoAboutTicketFeature>();
        })
        .Build();
    await Run(host.Services);
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
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

async Task Run(IServiceProvider services)
{
    var features = new[]
    {
        "Start Conversation with Nex AI",
        "Summarize the Ticket",
        "Search for Tickets by Phrase",
        "Search for Azure Work Items by Phrase",
        "Search for Info About Ticket"
    };

    switch (AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(features).UseConverter(choice => choice)))
    {
        case "Start Conversation with Nex AI":
            await services.GetRequiredService<NexAIAgent>().StartConversation();
            return;
        case "Summarize the Ticket":
            await services.GetRequiredService<SummarizeZendeskTicketFeature>().Run();
            return;
        case "Search for Tickets by Phrase":
            await services.GetRequiredService<SearchForZendeskTicketsByPhraseFeature>().Run(10);
            return;
        case "Search for Azure Work Items by Phrase":
            await services.GetRequiredService<SearchForAzureWorkItemsByPhraseFeature>().Run(10);
            return;
        case "Search for Info About Ticket":
            await services.GetRequiredService<SearchForInfoAboutTicketFeature>().Run();
            return;
        default:
            throw new InvalidOperationException("Invalid feature selected.");
    }
}