using Microsoft.Extensions.DependencyInjection;
using NexAI.Config;
using NexAI.LLMs.Common;

namespace NexAI.Agents;

public static class AgentsExtension
{
    public static IServiceCollection AddAgents(this IServiceCollection services, Options options) =>
        services.AddNexAIAgent(options);
    private static IServiceCollection AddNexAIAgent(this IServiceCollection services, Options options) =>
        options.Get<LLMsOptions>().Mode switch
        {
            LLM.OpenAI or LLM.Ollama  => services.AddSingleton<INexAIAgent, NexAIAgent>(),
            LLM.Fake => services.AddSingleton<INexAIAgent, FakeNexAIAgent>(),
            _ => throw new($"Unknown LLM or unsupported mode: {options.Get<LLMsOptions>().Mode}")
        };
}