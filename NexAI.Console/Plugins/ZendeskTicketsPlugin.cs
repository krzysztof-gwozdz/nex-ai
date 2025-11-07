using System.ComponentModel;
using Microsoft.SemanticKernel;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console.Plugins;

public class ZendeskTicketsPlugin(
    GetZendeskTicketByExternalIdQuery getZendeskTicketByExternalIdQuery,
    GetZendeskTicketsByExternalIdsQuery getZendeskTicketsByExternalIdsQuery,
    FindSimilarZendeskTicketsByPhraseQuery findSimilarZendeskTicketsByPhraseQuery,
    FindZendeskTicketsThatContainPhraseQuery findZendeskTicketsThatContainPhraseQuery,
    GetInfoAboutZendeskHierarchyQuery getInfoAboutZendeskHierarchyQuery)
{
    [KernelFunction("get_zendesk_ticket_by_external_id")]
    [Description("Retrieves a Zendesk ticket by its external id.")]
    public async Task<ZendeskTicket?> GetTicketByExternalId(string externalId, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_zendesk_ticket_by_external_id. Retrieving Zendesk ticket with external id: {externalId}[/]");
        return await getZendeskTicketByExternalIdQuery.Handle(externalId, cancellationToken);
    }

    [KernelFunction("get_zendesk_tickets_by_external_ids")]
    [Description("Retrieves Zendesk tickets by their external ids.")]
    public async Task<ZendeskTicket[]> GetTicketsByExternalIds(string[] externalIds, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_zendesk_tickets_by_external_ids. Retrieving Zendesk tickets with external ids: {string.Join(", ", externalIds)}[/]");
        return await getZendeskTicketsByExternalIdsQuery.Handle(externalIds, cancellationToken);
    }

    [KernelFunction("find_similar_zendesk_tickets_by_phrase")]
    [Description("Finds similar tickets based on a phrase. It uses embedding to find similar tickets.")]
    public async Task<SearchResult[]> FindSimilarTicketsByPhrase(string phrase, int limit, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool find_similar_tickets_by_phrase. Finding similar tickets for phrase: {phrase} with limit: {limit}[/]");
        return await findSimilarZendeskTicketsByPhraseQuery.Handle(phrase, limit, cancellationToken);
    }

    [KernelFunction("find_zendesk_tickets_by_phrase")]
    [Description("Finds tickets based on a phrase. It uses full-text search to find tickets.")]
    public async Task<SearchResult[]> FindZendeskTicketsByPhrase(string phrase, int limit, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool find_zendesk_tickets_by_phrase. Finding tickets for phrase: {phrase} with limit: {limit}[/]");
        return await findZendeskTicketsThatContainPhraseQuery.Handle(phrase, limit, cancellationToken);
    }

    [KernelFunction("get_info_about_zendesk_hierarchy")]
    [Description("Gets information about the Zendesk hierarchy: users and groups. The input is a Cypher query to be executed against the Neo4j database." +
                 "Cypher structure:" +
                 "Nodes: User(id, zendeskId, name), Group(id, zendeskId, name)" +
                 "Relationships: MEMBER_OF(from User to Group)")]
    public async Task<string> GetInfoAboutZendeskHierarchy(string cypherQuery)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_info_about_zendesk_hierarchy. Retrieving Zendesk hierarchy information with Cypher query: {cypherQuery.EscapeMarkup()}[/]");
        return await getInfoAboutZendeskHierarchyQuery.Handle(cypherQuery);
    }
}