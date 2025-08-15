using NexAI.Config;

namespace NexAI.AzureDevOps.Queries;

public class GetAzureDevopsWorkItemsRelatedToZendeskIssueQuery(Options options)
{
    private readonly AzureDevOpsClient _azureDevOpsClient = new(options);

    public async Task<AzureDevOpsWorkItem[]> Handle(string zendeskIssueNumber, int limit)
    {
        var query = await _azureDevOpsClient.GetOrCreateQuery(GetQuery(zendeskIssueNumber), limit);
        var workItems = await _azureDevOpsClient.GetWorkItems(query);
        return workItems.Select(workItem => new AzureDevOpsWorkItem(workItem)).ToArray();
    }

    private static string GetQuery(string zendeskIssueNumber) =>
        $@"
        SELECT
            [System.Id],
            [System.WorkItemType],
            [System.Title],
            [System.AssignedTo],
            [System.State],
            [System.Tags]
        FROM workitems
        WHERE
            [System.TeamProject] = 'Easy'
            AND (
                [System.Description] CONTAINS WORDS 'zendesk'
                OR [System.Title] CONTAINS 'zendesk'
                OR [System.Tags] CONTAINS 'Zendesk'
            )
            AND (
                [System.Title] CONTAINS '{zendeskIssueNumber}'
                OR [System.Description] CONTAINS WORDS '{zendeskIssueNumber}'
            )
        ";
}