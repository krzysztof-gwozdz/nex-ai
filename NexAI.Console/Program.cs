using NexAI.Config;
using NexAI.Console;
using NexAI.Console.Features;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
    var options = new Options(Configuration.Get());

    var features = new[]
    {
        "Start Conversation with Nex AI",
        "Search for Issues by Phrase",
        "Search for Azure Work Items by Phrase",
        "Search for Info About Issue"
    };
    var feature = new SelectionPrompt<string>().AddChoices(features).UseConverter(choice => choice);
    switch (AnsiConsole.Prompt(feature))
    {
        case "Start Conversation with Nex AI":
            var agent = new NexAIAgent(options);
            await agent.StartConversation();
            break;
        case "Search for Issues by Phrase":
            await new SearchForZendeskIssuesByPhraseFeature(options).Run(10);
            break;
        case "Search for Azure Work Items by Phrase":
            await new SearchForAzureWorkItemsByPhraseFeature(options).Run(10);
            break;
        case "Search for Info About Issue":
            await new SearchForInfoAboutIssueFeature(options).Run();
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