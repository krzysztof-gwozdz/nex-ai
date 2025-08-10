using NexAI.Config;
using NexAI.DataImporter;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI - Data Importer").Color(Color.Red1));
    var options = new Options(Configuration.Get());
    var zendeskIssueStore = new ZendeskIssueImporter(options);
    await zendeskIssueStore.Initialize();
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();