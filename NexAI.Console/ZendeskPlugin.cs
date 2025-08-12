using System.ComponentModel;
using Microsoft.SemanticKernel;
using NexAI.Config;
using NexAI.Zendesk;
using NexAI.Zendesk.Queries;
using Spectre.Console;

namespace NexAI.Console;

public class ZendeskPlugin(Options options)
{
    [KernelFunction("get_zendesk_issue_by_number")]
    [Description("Retrieves a Zendesk issue by its number.")]
    public async Task<ZendeskIssue?> GetIssueByNumber(string number)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool get_zendesk_issue_by_number. Retrieving Zendesk issue with number: {number}[/]");
        return await new GetZendeskIssueByNumberQuery(options).Handle(number);
    }

    [KernelFunction("find_similar_issue_to_specific_issue")]
    [Description("Finds similar issues based on the issue number.")]
    public async Task<List<SimilarIssue>> FindSimilarIssuesToSpecificIssueQuery(string number, ulong limit)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool find_similar_issues_by_number. Finding similar issues for number: {number} with limit: {limit}[/]");
        return await new FindSimilarIssuesToSpecificIssueQuery(options).Handle(number, limit);
    }

    [KernelFunction("find_similar_issues_by_phrase")]
    [Description("Finds similar issues based on a phrase.")]
    public async Task<List<SimilarIssue>> FindSimilarIssuesByPhrase(string phrase, ulong limit)
    {
        AnsiConsole.MarkupLine($"[yellow]Using tool find_similar_issues_by_phrase. Finding similar issues for phrase: {phrase} with limit: {limit}[/]");
        return await new FindSimilarIssuesByPhraseQuery(options).Handle(phrase, limit);
    }
}