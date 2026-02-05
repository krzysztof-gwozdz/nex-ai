using Microsoft.Extensions.DependencyInjection;
using NexAI.Config;
using NexAI.Zendesk.Api;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.MongoDb;
using NexAI.Zendesk.QdrantDb;
using NexAI.Zendesk.Queries;

namespace NexAI.Zendesk;

public static class ZendeskExtensions
{
    public static IServiceCollection AddZendesk(this IServiceCollection services)
    {
        services
            .AddHttpClient<ZendeskApiClient>((serviceProvider, client) =>
            {
                var zendeskOptions = serviceProvider.GetRequiredService<Options>().Get<ZendeskOptions>();
                client.BaseAddress = new(zendeskOptions.ApiBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {zendeskOptions.AuthorizationToken}");
            });

        return services
            .AddScoped<ZendeskMongoDbStructure>()
            .AddScoped<ZendeskQdrantStructure>()
            .AddScoped<ZendeskTicketMongoDbCollection>()
            .AddScoped<UpsertZendeskGroupCommand>()
            .AddScoped<UpsertZendeskMembersOfRelationshipCommand>()
            .AddScoped<UpsertZendeskUserCommand>()
            .AddScoped<FindSimilarZendeskTicketsByPhraseQuery>()
            .AddScoped<FindZendeskTicketsThatContainPhraseQuery>()
            .AddScoped<GetInfoAboutZendeskHierarchyQuery>()
            .AddScoped<GetZendeskGroupByNameQuery>()
            .AddScoped<GetZendeskTicketByExternalIdQuery>()
            .AddScoped<GetZendeskTicketsByIdQuery>()
            .AddScoped<GetZendeskTicketsByExternalIdsQuery>()
            .AddScoped<GetZendeskTicketsByIdsQuery>()
            .AddScoped<GetZendeskTicketSummaryQuery>()
            .AddScoped<GetZendeskUsersOfGroupQuery>()
            .AddScoped<StreamZendeskTicketSummaryQuery>();
    }
}