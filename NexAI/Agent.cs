using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using NexAI.Config;
using NexAI.Zendesk;
using Spectre.Console;

namespace NexAI;

public class Agent
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly OpenAIPromptExecutionSettings _openAIPromptExecutionSettings;
    private readonly ZendeskIssueStore _zendeskIssueStore;

    public Agent(Options options, ZendeskIssueStore zendeskIssueStore)
    {
        var openAIOptions = options.Get<OpenAIOptions>();
        var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(openAIOptions.Model, openAIOptions.ApiKey);
        _kernel = builder.Build();
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        _openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        _zendeskIssueStore = zendeskIssueStore;
    }

    public async Task StartConversation()
    {
        while (true)
        {
            var chatHistory = new ChatHistory();
            while (true)
            {
                PrintAIMessage("Welcome to Nex AI! Type your message below. Type [bold]RESET[/] to reset the conversation or [bold]STOP[/] to exit.");
                var userMessage = AnsiConsole.Prompt(new TextPrompt<string>(">"));
                if (userMessage == "RESET")
                    break;
                if (userMessage == "STOP")
                    return;
                chatHistory.AddUserMessage(userMessage);
                var result = await GetAIResponse(chatHistory);
                var assistantResponse = result.Content ?? string.Empty;
                PrintAIMessage(assistantResponse);
                chatHistory.AddMessage(result.Role, assistantResponse);
            }

            AnsiConsole.Write(new Rule());
        }
    }

    public async Task SearchForSimilarIssues()
    {
        while (true)
        {
            PrintAIMessage("Welcome to Similar Issues Search! Enter an issue number to find similar issues. Type [bold]STOP[/] to exit.");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("Issue number > "));

            if (userMessage.ToUpper() == "STOP")
                return;

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                PrintAIMessage("Please enter a valid issue number.");
                continue;
            }

            try
            {
                var targetIssue = await _zendeskIssueStore.GetIssueByNumber(userMessage);
                if (targetIssue is null)
                {
                    PrintAIMessage($"[red]Issue with number '{userMessage}' not found.[/]");
                    continue;
                }
                var similarIssues = await _zendeskIssueStore.FindSimilarIssuesByNumber(userMessage);
                PrintAIMessage($"[green]Found issue: {targetIssue.Title}[/]");
                DisplaySimilarIssues(similarIssues);
            }
            catch (Exception ex)
            {
                PrintAIMessage($"[red]Error: {ex.Message}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    public async Task SearchForIssues()
    {
        while (true)
        {
            PrintAIMessage("Welcome to Issues Search! Enter search phrase. Type [bold]STOP[/] to exit.");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));

            if (userMessage.ToUpper() == "STOP")
                return;

            try
            {
                var similarIssues = await _zendeskIssueStore.FindSimilarIssuesByPhrase(userMessage, 10);
                DisplaySimilarIssues(similarIssues);
            }catch (Exception ex)
            {
                PrintAIMessage($"[red]Error: {ex.Message}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private static void DisplaySimilarIssues(List<SimilarIssue>? similarIssues)
    {
        if (similarIssues is null || similarIssues.Count == 0)
        {
            PrintAIMessage("[yellow]No similar issues found.[/]");
        }
        else
        {
            PrintAIMessage($"[bold]Found {similarIssues.Count} similar issues:[/]");
            var table = new Table()
                .AddColumn("Number")
                .AddColumn("Title")
                .AddColumn("Similarity Score");
            foreach (var issue in similarIssues)
            {
                table.AddRow(
                    issue.Number,
                    issue.Title,
                    $"{issue.Similarity:P1}"
                );
            }
            AnsiConsole.Write(table);
        }
    }

    private static void PrintAIMessage(string message) => AnsiConsole.MarkupLine($"[Aquamarine1]{message.EscapeMarkup()}[/]");

    private async Task<ChatMessageContent> GetAIResponse(ChatHistory chatHistory) =>
        await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: _openAIPromptExecutionSettings,
            kernel: _kernel);
}