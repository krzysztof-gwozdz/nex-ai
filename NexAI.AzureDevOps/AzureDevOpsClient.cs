using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using NexAI.Config;

namespace NexAI.AzureDevOps;

public class AzureDevOpsClient
{
    private readonly AzureDevOpsOptions _azureDevOpsOptions;
    private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;

    public AzureDevOpsClient(Options options)
    {
        _azureDevOpsOptions = options.Get<AzureDevOpsOptions>();
        var connection = new VssConnection(_azureDevOpsOptions.OrganizationUrl, new VssBasicCredential(string.Empty, _azureDevOpsOptions.PersonalAccessToken));
        _workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
    }

    public async Task<WorkItemQueryResult> GetOrCreateQuery(string query, int limit) => 
        await _workItemTrackingHttpClient.QueryByWiqlAsync(new() { Query = query }, _azureDevOpsOptions.ProjectName, top: limit);

    public async Task<List<WorkItem>> GetWorkItems(WorkItemQueryResult query)
    {
        var workItems = new List<WorkItem>();
        if (!query.WorkItems.Any())
        {
            return workItems;
        }

        var skip = 0;
        const int batchSize = 100;
        WorkItemReference[] workItemRefs;
        do
        {
            workItemRefs = query.WorkItems.Skip(skip).Take(batchSize).ToArray();
            if (workItemRefs.Length != 0)
            {
                workItems.AddRange(await _workItemTrackingHttpClient.GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id)));
            }

            skip += batchSize;
        } while (workItemRefs.Length == batchSize);

        return workItems;
    }
}