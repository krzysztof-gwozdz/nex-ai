using Microsoft.Extensions.DependencyInjection;
using NexAI.Config;
using NexAI.LLMs.Common;

namespace NexAI.Agents;

public static class AgentsExtension
{
    public static IServiceCollection AddAgents(this IServiceCollection services, Options options) =>
        services.AddNexAIAgent(options);

    private static IServiceCollection AddNexAIAgent(this IServiceCollection services, Options options) =>
        LLM.ForAll(
            options.Get<LLMsOptions>().Mode,
            services.AddScoped<INexAIAgent, NexAIAgent>,
            services.AddScoped<INexAIAgent, NexAIAgent>,
            services.AddScoped<INexAIAgent, FakeNexAIAgent>
        );
}