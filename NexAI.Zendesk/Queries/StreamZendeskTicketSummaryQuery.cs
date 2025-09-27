using MongoDB.Bson;
using NexAI.Config;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Queries;

public class StreamZendeskTicketSummaryQuery(Chat chat, Options options)
{
    public IAsyncEnumerable<string> Handle(ZendeskTicket zendeskTicket)
    {
        var systemPrompt = options.Get<LLMsOptions>().Prompts.ZendeskTicketSummary;
        var json = zendeskTicket.ToJson();
        return chat.AskStream(systemPrompt, json ?? string.Empty);
    }
}