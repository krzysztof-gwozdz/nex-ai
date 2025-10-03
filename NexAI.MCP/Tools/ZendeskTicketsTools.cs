using System.ComponentModel;
using ModelContextProtocol.Server;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;

namespace NexAI.MCP.Tools;

public class ZendeskTicketsTools(
    GetZendeskTicketByExternalIdQuery getZendeskTicketByExternalIdQuery,
    GetZendeskTicketsByExternalIdsQuery getZendeskTicketsByExternalIdsQuery,
    FindSimilarZendeskTicketsByPhraseQuery findSimilarZendeskTicketsByPhraseQuery,
    FindZendeskTicketsThatContainPhraseQuery findZendeskTicketsThatContainPhraseQuery)
{
    [McpServerTool(Name ="get_zendesk_ticket_by_external_id")]
    [Description("Retrieves a Zendesk ticket by its external id.")]
    public async Task<ZendeskTicket?> GetTicketByExternalId(string externalId, CancellationToken cancellationToken) => 
        await getZendeskTicketByExternalIdQuery.Handle(externalId, cancellationToken);

    [McpServerTool(Name ="get_zendesk_tickets_by_external_ids")]
    [Description("Retrieves Zendesk tickets by their external ids.")]
    public async Task<ZendeskTicket[]> GetTicketsByExternalIds(string[] externalIds, CancellationToken cancellationToken) => 
        await getZendeskTicketsByExternalIdsQuery.Handle(externalIds, cancellationToken);

    [McpServerTool(Name ="find_similar_zendesk_tickets_by_phrase")]
    [Description("Finds similar tickets based on a phrase. It uses embedding to find similar tickets.")]
    public async Task<SearchResult[]> FindSimilarTicketsByPhrase(string phrase, int limit, CancellationToken cancellationToken) => 
        await findSimilarZendeskTicketsByPhraseQuery.Handle(phrase, limit, cancellationToken);

    [McpServerTool(Name ="find_zendesk_tickets_by_phrase")]
    [Description("Finds tickets based on a phrase. It uses full-text search to find tickets.")]
    public async Task<SearchResult[]> FindZendeskTicketsByPhrase(string phrase, int limit, CancellationToken cancellationToken) => 
        await findZendeskTicketsThatContainPhraseQuery.Handle(phrase, limit, cancellationToken);
}