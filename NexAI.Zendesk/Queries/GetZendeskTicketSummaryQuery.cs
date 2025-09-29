using MongoDB.Bson;
using NexAI.Config;
using NexAI.LLMs.Common;

namespace NexAI.Zendesk.Queries;

public class GetZendeskTicketSummaryQuery(Chat chat, Options options)
{
    public async Task<string> Handle(ZendeskTicket zendeskTicket, CancellationToken cancellationToken)
    {
        var systemPrompt = options.Get<LLMsOptions>().Prompts.ZendeskTicketSummary;
        var json = zendeskTicket.ToJson();
        return await chat.Ask(systemPrompt, json ?? string.Empty, cancellationToken);
    }
}