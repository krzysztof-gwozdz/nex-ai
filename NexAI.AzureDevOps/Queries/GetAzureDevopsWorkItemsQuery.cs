using NexAI.Config;

namespace NexAI.AzureDevOps.Queries;

public class GetAzureDevopsWorkItemsQuery(Options options)
{
    private readonly AzureDevOpsClient _azureDevOpsClient = new(options);

    public async Task<AzureDevOpsWorkItem[]> Handle(string phrase, int limit)
    {
        var query = await _azureDevOpsClient.GetOrCreateQuery(GetQuery(phrase), limit);
        var workItems = await _azureDevOpsClient.GetWorkItems(query);
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