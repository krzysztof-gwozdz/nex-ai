using System.ComponentModel;
using Microsoft.SemanticKernel;
using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Plugins;

public class ZendeskTicketsPlugin(Options options)
{
    [KernelFunction("get_zendesk_ticket_by_number")]
    [Description("Retrieves a Zendesk ticket by its number.")]
    public async Task<ZendeskTicket?> GetTicketByNumber(string number)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_zendesk_ticket_by_number. Retrieving Zendesk ticket with number: {number}[/]");
        return await new GetZendeskTicketByNumberQuery(options).Handle(number);
    }

    [KernelFunction("get_zendesk_tickets_by_numbers")]
    [Description("Retrieves Zendesk tickets by their numbers.")]
    public async Task<ZendeskTicket[]> GetTicketsByNumbers(string[] numbers)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_zendesk_tickets_by_numbers. Retrieving Zendesk tickets with numbers: {string.Join(", ", numbers)}[/]");
        return await new GetZendeskTicketsByNumbersQuery(options).Handle(numbers);
    }

    [KernelFunction("find_similar_zendesk_tickets_by_phrase")]
    [Description("Finds similar tickets based on a phrase. It uses embedding to find similar tickets.")]
    public async Task<SearchResult[]> FindSimilarTicketsByPhrase(string phrase, int limit)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool find_similar_tickets_by_phrase. Finding similar tickets for phrase: {phrase} with limit: {limit}[/]");
        return await new FindSimilarZendeskTicketsByPhraseQuery(options).Handle(phrase, limit);
    }
    
    [KernelFunction("find_zendesk_tickets_by_phrase")]
    [Description("Finds tickets based on a phrase. It uses full-text search to find tickets.")]
    public async Task<SearchResult[]> FindZendeskTicketsByPhrase(string phrase, int limit)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool find_zendesk_tickets_by_phrase. Finding tickets for phrase: {phrase} with limit: {limit}[/]");
        return await new FindZendeskTicketsThatContainPhraseQuery(options).Handle(phrase, limit);
    }
}