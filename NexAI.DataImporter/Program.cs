using NexAI.Config;
using NexAI.DataImporter;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI - Data Importer").Color(Color.Red1));
    var options = new Options(Configuration.Get());
    var zendeskIssueUpdater = new ZendeskIssueUpdater(options);
    await zendeskIssueUpdater.Update();
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();