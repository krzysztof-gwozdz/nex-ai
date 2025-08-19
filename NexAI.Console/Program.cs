using Microsoft.Extensions.Configuration;
using NexAI.Config;
using NexAI.Console;
using NexAI.Console.Features;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
    var options = new Options(GetConfiguration());

    var features = new[]
    {
        "Start Conversation with Nex AI",
        "Search for Tickets by Phrase",
        "Search for Azure Work Items by Phrase",
        "Search for Info About Ticket"
    };
    var feature = new SelectionPrompt<string>().AddChoices(features).UseConverter(choice => choice);
    switch (AnsiConsole.Prompt(feature))
    {
        case "Start Conversation with Nex AI":
            var agent = new NexAIAgent(options);
            await agent.StartConversation();
            break;
        case "Search for Tickets by Phrase":
            await new SearchForZendeskTicketsByPhraseFeature(options).Run(10);
            break;
        case "Search for Azure Work Items by Phrase":
            await new SearchForAzureWorkItemsByPhraseFeature(options).Run(10);
            break;
        case "Search for Info About Ticket":
            await new SearchForInfoAboutTicketFeature(options).Run();
            break;
        default:
            throw new InvalidOperationException("Invalid feature selected.");
    }
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