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
        "Search for Similar Issues to Specific Issue",
        "Search for Issues by Phrase"
    };
    var feature = new SelectionPrompt<string>().AddChoices(features).UseConverter(choice => choice);
    switch (AnsiConsole.Prompt(feature))
    {
        case "Start Conversation with Nex AI":
            var agent = new NexAIAgent(options);
            await agent.StartConversation();
            break;
        case "Search for Similar Issues to Specific Issue":
            await new SearchForSimilarZendeskIssuesByNumberFeature(options).Run(10);
            break;
        case "Search for Issues by Phrase":
            await new SearchForSimilarZendeskIssuesByPhraseFeature(options).Run(10);
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