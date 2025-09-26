using MongoDB.Bson;
using NexAI.Config;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Queries;

public class StreamZendeskTicketSummaryQuery(Options options)
{
    public IAsyncEnumerable<string> Handle(ZendeskTicket zendeskTicket)
    {
        var systemPrompt = options.Get<LLMsOptions>().Prompts.ZendeskTicketSummary;
        var chat = Chat.GetInstance(options);
        var json = zendeskTicket.ToJson();
        return chat.AskStream(systemPrompt, json ?? string.Empty);
    }
}