using NexAI.Config;

namespace NexAI.Zendesk.Queries;

public class FindSimilarIssuesToSpecificIssueQuery(Options options)
{
    public async Task<List<SimilarIssue>> Handle(string number, ulong limit)
    {
        var getZendeskIssueByNumberQuery = new GetZendeskIssueByNumberQuery(options);
        var zendeskIssue = await getZendeskIssueByNumberQuery.Handle(number);
        if (zendeskIssue == null)
            return [];
        var findSimilarIssuesByPhraseQuery = new FindSimilarIssuesByPhraseQuery(options);
        var similarIssues = await findSimilarIssuesByPhraseQuery.Handle(zendeskIssue.CombinedContent(), limit + 1);
        return similarIssues
            .Where(issue => issue.Number != number)
            .Take((int)limit)
            .ToList();
    }
}