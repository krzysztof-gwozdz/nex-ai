using Microsoft.Extensions.DependencyInjection;
using NexAI.AzureDevOps.Queries;

namespace NexAI.AzureDevOps;

public static class AzureDevOpsExtensions
{
    public static IServiceCollection AddAzureDevOps(this IServiceCollection services) =>
        services
            .AddScoped<AzureDevOpsClient>()
            .AddScoped<GetAzureDevopsWorkItemsQuery>()
            .AddScoped<GetAzureDevopsWorkItemsRelatedToZendeskTicketQuery>();
}