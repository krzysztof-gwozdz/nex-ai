using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace NexAI.AzureDevOps;

public record AzureDevOpsWorkItem(string Id, string Title, string Description, string State, string AssignedTo)
{
    public AzureDevOpsWorkItem(WorkItem workItem) : this(
        workItem.Id?.ToString() ?? string.Empty,
        workItem.Fields.GetValueOrDefault("System.Title")?.ToString() ?? string.Empty,
        workItem.Fields.GetValueOrDefault("System.Description")?.ToString() ?? string.Empty,
        workItem.Fields.GetValueOrDefault("System.State")?.ToString() ?? string.Empty,
        (workItem.Fields.GetValueOrDefault("System.AssignedTo") as Microsoft.VisualStudio.Services.WebApi.IdentityRef)?.DisplayName ?? string.Empty)
    {
    }
}