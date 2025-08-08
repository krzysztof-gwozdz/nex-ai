using Microsoft.Extensions.DependencyInjection;
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
        builder.Services.AddSingleton(zendeskIssueStore);
        _kernel = builder.Build();
        _kernel.Plugins.AddFromType<ZendeskPlugin>("ZendeskIssues", _kernel.Services);
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        _openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };
        _zendeskIssueStore = zendeskIssueStore;
    }

    public async Task StartConversation()
    {
        while (true)
        {
            var chatHistory = new ChatHistory();
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Nex AI! Type your message below. Type [bold]RESET[/] to reset the conversation or [bold]STOP[/] to exit.[/]");
            while (true)
            {
                var userMessage = AnsiConsole.Prompt(new TextPrompt<string>(">"));
                if (userMessage == "RESET")
                    break;
                if (userMessage == "STOP")
                    return;
                chatHistory.AddUserMessage(userMessage);
                var result = await GetAIResponse(chatHistory);
                var assistantResponse = result.Content ?? string.Empty;
                AnsiConsole.MarkupLine($"[Aquamarine1]{assistantResponse.EscapeMarkup()}[/]");
                chatHistory.AddMessage(result.Role, assistantResponse);
            }

            AnsiConsole.Write(new Rule());
        }
    }

    public async Task SearchForSimilarIssues()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Similar Issues Search! Enter an issue number to find similar issues. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("Issue number > "));

            if (userMessage.ToUpper() == "STOP")
                return;

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                AnsiConsole.MarkupLine("[Aquamarine1]Please enter a valid issue number.[/]");
                continue;
            }

            try
            {
                var targetIssue = await _zendeskIssueStore.GetIssueByNumber(userMessage);
                if (targetIssue is null)
                {
                    AnsiConsole.MarkupLine($"[red]Issue with number '{userMessage.EscapeMarkup()}' not found.[/]");
                    continue;
                }

                var similarIssues = await _zendeskIssueStore.FindSimilarIssuesByNumber(userMessage, 10);
                AnsiConsole.MarkupLine($"[green]Found issue: {targetIssue.Title.EscapeMarkup()}[/]");
                DisplaySimilarIssues(similarIssues);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    public async Task SearchForIssues()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[Aquamarine1]Welcome to Issues Search! Enter search phrase. Type [bold]STOP[/] to exit.[/]");
            var userMessage = AnsiConsole.Prompt(new TextPrompt<string>("> "));

            if (userMessage.ToUpper() == "STOP")
                return;

            try
            {
                var similarIssues = await _zendeskIssueStore.FindSimilarIssuesByPhrase(userMessage, 10);
                DisplaySimilarIssues(similarIssues);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(new Rule());
        }
    }

    private static void DisplaySimilarIssues(List<SimilarIssue>? similarIssues)
    {
        if (similarIssues is null || similarIssues.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No similar issues found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Found {similarIssues.Count} similar issues:[/]");
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

    private async Task<ChatMessageContent> GetAIResponse(ChatHistory chatHistory) =>
        await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: _openAIPromptExecutionSettings,
            kernel: _kernel);
}