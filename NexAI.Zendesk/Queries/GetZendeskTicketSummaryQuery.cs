using MongoDB.Bson;
using NexAI.Config;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketSummaryQuery(Options options)
{
    public async Task<string> Handle(ZendeskTicket zendeskTicket)
    {
        var systemPrompt = options.Get<LLMsOptions>().Prompts.ZendeskTicketSummary;
        var chat = Chat.GetInstance(options);
        var json = zendeskTicket.ToJson();
        return await chat.Ask(systemPrompt, json ?? string.Empty);
    }
}