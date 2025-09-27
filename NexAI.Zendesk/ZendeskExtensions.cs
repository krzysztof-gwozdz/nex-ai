using Microsoft.Extensions.DependencyInjection;
using NexAI.Zendesk.Api;
using NexAI.Zendesk.Queries;

namespace NexAI.Zendesk;

public static class ZendeskExtensions
{
    public static IServiceCollection AddZendesk(this IServiceCollection services) =>
        services
            .AddSingleton<ZendeskApiClient>()
            .AddSingleton<FindSimilarZendeskTicketsByPhraseQuery>()
            .AddSingleton<FindZendeskTicketsThatContainPhraseQuery>()
            .AddSingleton<GetZendeskTicketByExternalIdQuery>()
            .AddSingleton<GetZendeskTicketsByIdQuery>()
            .AddSingleton<GetZendeskTicketsByExternalIdsQuery>()
            .AddSingleton<GetZendeskTicketsByIdsQuery>()
            .AddSingleton<GetZendeskTicketSummaryQuery>()
            .AddSingleton<StreamZendeskTicketSummaryQuery>();
}