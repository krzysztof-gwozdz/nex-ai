using System.Text;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NexAI.Config;
using NexAI.Console;
using NexAI.Console.Features;
using NexAI.LLMs.Common;
using Spectre.Console;

try
{
    Console.OutputEncoding = Encoding.UTF8;
    AnsiConsole.Write(new FigletText("Nex AI").Color(Color.Aquamarine1));
    
    try
    {
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }
    catch
    {
        // ignored
    }
    
    var options = new Options(GetConfiguration());
    var features = new[]
    {
        "Start Conversation with Nex AI",
        "Summarize the Ticket",
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
        case "Summarize the Ticket":
            await new SummarizeZendeskTicketFeature(options).Run();
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