using System.ComponentModel;
using ModelContextProtocol.Server;
using NexAI.AzureDevOps;
using NexAI.AzureDevOps.Queries;

namespace NexAI.MCP.Tools;

public class AzureDevOpsIssuesTools(
    GetAzureDevopsWorkItemsQuery getAzureDevopsWorkItemsQuery,
    GetAzureDevopsWorkItemsRelatedToZendeskTicketQuery getAzureDevopsWorkItemsRelatedToZendeskTicketQuery
)
{
    [McpServerTool(Name = "get_azure_devops_work_item_by_phrase")]
    [Description("Get Azure DevOps work items by phrase")]
    public async Task<AzureDevOpsWorkItem[]> FindAzureDevOpsWorkItemsByPhrase(string phrase, int limit, CancellationToken cancellationToken) => 
        await getAzureDevopsWorkItemsQuery.Handle(phrase, limit, cancellationToken);

    [McpServerTool(Name = "get_azure_devops_work_items_related_to_zendesk_ticket")]
    [Description("Get Azure DevOps work items related to Zendesk ticket")]
    public async Task<AzureDevOpsWorkItem[]> GetAzureDevOpsWorkItemsRelatedToZendeskTicket(string zendeskTicketId, int limit, CancellationToken cancellationToken) => 
        await getAzureDevopsWorkItemsRelatedToZendeskTicketQuery.Handle(zendeskTicketId, limit, cancellationToken);
}