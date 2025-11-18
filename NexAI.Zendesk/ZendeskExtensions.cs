using Microsoft.Extensions.DependencyInjection;
using NexAI.Zendesk.Api;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.MongoDb;
using NexAI.Zendesk.QdrantDb;
using NexAI.Zendesk.Queries;

namespace NexAI.Zendesk;

public static class ZendeskExtensions
{
    public static IServiceCollection AddZendesk(this IServiceCollection services) =>
        services
            .AddSingleton<ZendeskApiClient>()
            .AddSingleton<ZendeskMongoDbStructure>()
            .AddSingleton<ZendeskQdrantStructure>()
            .AddSingleton<UpsertZendeskGroupCommand>()
            .AddSingleton<UpsertZendeskMembersOfRelationshipCommand>()
            .AddSingleton<UpsertZendeskUserCommand>()
            .AddSingleton<FindSimilarZendeskTicketsByPhraseQuery>()
            .AddSingleton<FindZendeskTicketsThatContainPhraseQuery>()
            .AddSingleton<GetInfoAboutZendeskHierarchyQuery>()
            .AddSingleton<GetZendeskGroupByNameQuery>()
            .AddSingleton<GetZendeskTicketByExternalIdQuery>()
            .AddSingleton<GetZendeskTicketsByIdQuery>()
            .AddSingleton<GetZendeskTicketsByExternalIdsQuery>()
            .AddSingleton<GetZendeskTicketsByIdsQuery>()
            .AddSingleton<GetZendeskTicketSummaryQuery>()
            .AddSingleton<GetZendeskUsersOfGroupQuery>()
            .AddSingleton<StreamZendeskTicketSummaryQuery>();
}