using System.ComponentModel;
using Microsoft.SemanticKernel;
using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Plugins;

public class ZendeskTicketsPlugin(Options options)
{
    [KernelFunction("get_zendesk_ticket_by_external_id")]
    [Description("Retrieves a Zendesk ticket by its external id.")]
    public async Task<ZendeskTicket?> GetTicketByExternalId(string externalId)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_zendesk_ticket_by_external_id. Retrieving Zendesk ticket with external id: {externalId}[/]");
        return await new GetZendeskTicketByExternalIdQuery(options).Handle(externalId);
    }

    [KernelFunction("get_zendesk_tickets_by_external_ids")]
    [Description("Retrieves Zendesk tickets by their external ids.")]
    public async Task<ZendeskTicket[]> GetTicketsByExternalIds(string[] externalIds)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_zendesk_tickets_by_external_ids. Retrieving Zendesk tickets with external ids: {string.Join(", ", externalIds)}[/]");
        return await new GetZendeskTicketsByExternalIdsQuery(options).Handle(externalIds);
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