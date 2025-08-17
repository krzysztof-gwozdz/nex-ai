using NexAI.Config;

namespace NexAI.AzureDevOps.Queries;

public class GetAzureDevopsWorkItemsRelatedToZendeskTicketQuery(Options options)
{
    private readonly AzureDevOpsClient _azureDevOpsClient = new(options);

    public async Task<AzureDevOpsWorkItem[]> Handle(string zendeskTicketNumber, int limit)
    {
        var query = await _azureDevOpsClient.GetOrCreateQuery(GetQuery(zendeskTicketNumber), limit);
        var workItems = await _azureDevOpsClient.GetWorkItems(query);
        return workItems.Select(workItem => new AzureDevOpsWorkItem(workItem)).ToArray();
    }

    private static string GetQuery(string zendeskTicketNumber) =>
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
                [System.Title] CONTAINS '{zendeskTicketNumber}'
                OR [System.Description] CONTAINS WORDS '{zendeskTicketNumber}'
            )
        ";
}