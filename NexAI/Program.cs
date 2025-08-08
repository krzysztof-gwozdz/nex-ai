using NexAI;
using NexAI.Config;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
    var options = new Options(Configuration.Get());
    var agent = new Agent(options);
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