using NexAI;
using NexAI.Config;
using NexAI.Zendesk;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
    var options = new Options(Configuration.Get());
    var zendeskIssueStore = new ZendeskIssueStore(options);
    await zendeskIssueStore.Initialize();
    var agent = new Agent(options, zendeskIssueStore);
    // await agent.StartConversation();
    // await agent.SearchForSimilarIssues();
    await agent.SearchForIssues();
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}
Console.WriteLine("Press any key to exit...");
Console.ReadKey();