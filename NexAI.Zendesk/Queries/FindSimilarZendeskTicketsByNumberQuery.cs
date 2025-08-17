using NexAI.Config;

namespace NexAI.Zendesk.Queries;

public class FindSimilarZendeskTicketsByNumberQuery(Options options)
{
    public async Task<List<SimilarTicket>> Handle(string number, int limit)
    {
        var getZendeskTicketByNumberQuery = new GetZendeskTicketByNumberQuery(options);
        var zendeskTicket = await getZendeskTicketByNumberQuery.Handle(number);
        if (zendeskTicket == null)
            return [];
        var findSimilarTicketsByPhraseQuery = new FindSimilarZendeskTicketsByPhraseQuery(options);
        var similarTickets = await findSimilarTicketsByPhraseQuery.Handle(zendeskTicket.CombinedContent(), limit + 1);
        return similarTickets
            .Where(ticket => ticket.Number != number)
            .Take(limit)
            .ToList();
    }
}