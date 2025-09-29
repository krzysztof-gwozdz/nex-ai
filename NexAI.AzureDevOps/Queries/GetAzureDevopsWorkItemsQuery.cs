namespace NexAI.AzureDevOps.Queries;

public class GetAzureDevopsWorkItemsQuery(AzureDevOpsClient azureDevOpsClient)
{
    public async Task<AzureDevOpsWorkItem[]> Handle(string phrase, int limit, CancellationToken cancellationToken)
    {
        var query = await azureDevOpsClient.GetOrCreateQuery(GetQuery(phrase), limit, cancellationToken);
        var workItems = await azureDevOpsClient.GetWorkItems(query);
        return workItems.Select(workItem => new AzureDevOpsWorkItem(workItem)).ToArray();
    }

    private static string GetQuery(string phrase) =>
        $@"
        SELECT 
            [System.Id],
            [System.Title],
            [System.AssignedTo],
            [System.State],                  
            [System.Description]        
        FROM workitems
        WHERE
            [System.TeamProject] = @project
            AND (
                [System.Title] CONTAINS WORDS '{phrase}'
                OR [System.Description] CONTAINS WORDS '{phrase}'
            )
        ";
}