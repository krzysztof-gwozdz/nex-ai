using MongoDB.Bson;
using NexAI.LLMs;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Queries;

public class StreamZendeskTicketSummaryQuery(Chat chat, PromptReader promptReader)
{
    public IAsyncEnumerable<string> Handle(ZendeskTicket zendeskTicket, CancellationToken cancellationToken)
    {
        var systemPrompt = promptReader.Read("ZendeskTicketSummary");
        var json = zendeskTicket.ToJson();
        return chat.AskStream(systemPrompt, json ?? string.Empty, cancellationToken);
    }
}