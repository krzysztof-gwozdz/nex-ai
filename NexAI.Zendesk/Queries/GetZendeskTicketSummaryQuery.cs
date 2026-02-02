using System.Text.Json;
using NexAI.LLMs;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketSummaryQuery(Chat chat, PromptReader promptReader)
{
    public async Task<string> Handle(ZendeskTicket zendeskTicket, CancellationToken cancellationToken)
    {
        var systemPrompt = promptReader.Read("ZendeskTicketSummary");
        var json = JsonSerializer.Serialize(zendeskTicket, new JsonSerializerOptions { WriteIndented = true });
        return await chat.Ask(systemPrompt, json ?? string.Empty, cancellationToken);
    }
}