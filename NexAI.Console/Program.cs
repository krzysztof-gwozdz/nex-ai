using NexAI.Config;
using NexAI.Console;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
    var options = new Options(Configuration.Get());
    var agent = new Agent(options);
    //await agent.SearchForSimilarIssues();
    //await agent.SearchForIssues();
    await agent.StartConversation();
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();