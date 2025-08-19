using Microsoft.Extensions.Configuration;
using NexAI.Config;
using NexAI.DataImporter.Zendesk;
using Spectre.Console;

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI - Data Importer").Color(Color.Red1));
    var options = new Options(GetConfiguration());
    var zendeskTicketUpdater = new ZendeskTicketUpdater(options);
    await zendeskTicketUpdater.Update();
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
    .AddJsonFile("appsettings.data_importer.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();