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

Console.OutputEncoding = Encoding.UTF8;
AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
try
{
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
cancellationTokenSource.Cancel();
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
    var options = new Dictionary<string, Func<Task>>
    {
        ["Start Conversation with Nex AI"] = async () => await services.GetRequiredService<NexAIAgent>().StartConversation(cancellationTokenSource.Token),
        ["Summarize the Ticket"] = async () => await services.GetRequiredService<SummarizeZendeskTicketFeature>().Run(cancellationTokenSource.Token),
        ["Search for Tickets by Phrase"] = async () => await services.GetRequiredService<SearchForZendeskTicketsByPhraseFeature>().Run(10, cancellationTokenSource.Token),
        ["Search for Azure Work Items by Phrase"] = async () => await services.GetRequiredService<SearchForAzureWorkItemsByPhraseFeature>().Run(10, cancellationTokenSource.Token),
        ["Search for Info About Ticket"] = async () => await services.GetRequiredService<SearchForInfoAboutTicketFeature>().Run(cancellationTokenSource.Token),
    };
    var selectedOption = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(options.Keys));
    await options[selectedOption]();
}