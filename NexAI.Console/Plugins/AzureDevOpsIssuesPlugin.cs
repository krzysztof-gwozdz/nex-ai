using System.ComponentModel;
using Microsoft.SemanticKernel;
using NexAI.AzureDevOps;
using NexAI.AzureDevOps.Queries;
using Spectre.Console;

namespace NexAI.Console.Plugins;

public class AzureDevOpsIssuesPlugin(
    GetAzureDevopsWorkItemsQuery getAzureDevopsWorkItemsQuery,
    GetAzureDevopsWorkItemsRelatedToZendeskTicketQuery getAzureDevopsWorkItemsRelatedToZendeskTicketQuery
)
{
    [KernelFunction("get_azure_devops_work_item_by_phrase")]
    [Description("Get Azure DevOps work items by phrase")]
    public async Task<AzureDevOpsWorkItem[]> FindAzureDevOpsWorkItemsByPhrase(string phrase, int limit)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool find_azure_devops_work_items_by_phrase. Finding work items for phrase: {phrase} with limit: {limit}[/]");
        return await getAzureDevopsWorkItemsQuery.Handle(phrase, limit);
    }

    [KernelFunction("get_azure_devops_work_items_related_to_zendesk_ticket")]
    [Description("Get Azure DevOps work items related to Zendesk ticket")]
    public async Task<AzureDevOpsWorkItem[]> GetAzureDevOpsWorkItemsRelatedToZendeskTicket(string zendeskTicketId, int limit)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_azure_devops_work_items_related_to_zendesk_ticket. Getting work items related to Zendesk ticket: {zendeskTicketId}[/]");
        return await getAzureDevopsWorkItemsRelatedToZendeskTicketQuery.Handle(zendeskTicketId, limit);
    }
}