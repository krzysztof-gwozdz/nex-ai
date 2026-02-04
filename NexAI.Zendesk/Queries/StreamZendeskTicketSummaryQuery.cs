using System.Text.Json;
using NexAI.LLMs;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Queries;

public class StreamZendeskTicketSummaryQuery(Chat chat, PromptReader promptReader)
{
    public IAsyncEnumerable<string> Handle(ZendeskTicket zendeskTicket, CancellationToken cancellationToken)
    {
        var systemPrompt = promptReader.Read("ZendeskTicketSummary");
        var json = JsonSerializer.Serialize(zendeskTicket, new JsonSerializerOptions { WriteIndented = true });
        return chat.AskStream(ConversationId.New(), systemPrompt, json, cancellationToken);
    }
}