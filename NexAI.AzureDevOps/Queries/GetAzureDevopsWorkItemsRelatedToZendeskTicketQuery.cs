namespace NexAI.AzureDevOps.Queries;

public class GetAzureDevopsWorkItemsRelatedToZendeskTicketQuery(AzureDevOpsClient azureDevOpsClient)
{
    public async Task<AzureDevOpsWorkItem[]> Handle(string zendeskTicketId, int limit, CancellationToken cancellationToken)
    {
        var query = await azureDevOpsClient.GetOrCreateQuery(GetQuery(zendeskTicketId), limit, cancellationToken);
        var workItems = await azureDevOpsClient.GetWorkItems(query);
        return workItems.Select(workItem => new AzureDevOpsWorkItem(workItem)).ToArray();
    }

    private static string GetQuery(string zendeskTicketId) =>
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
                [System.Title] CONTAINS '{zendeskTicketId}'
                OR [System.Description] CONTAINS WORDS '{zendeskTicketId}'
            )
        ";
}