using Microsoft.Extensions.DependencyInjection;
using NexAI.Config;
using NexAI.LLMs.Common;
using NexAI.LLMs.Fake;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs;

public static class LLMExtensions
{
    public static IServiceCollection AddLLM(this IServiceCollection services, Options options) =>
        services.AddSingleton<PromptReader>().AddLLMSpecificServices(options);

    private static IServiceCollection AddLLMSpecificServices(this IServiceCollection services, Options options) =>
        options.Get<LLMsOptions>().Mode switch
        {
            LLM.OpenAI => services.AddSingleton<TextEmbedder, OpenAITextEmbedder>().AddSingleton<Chat, OpenAIChat>(),
            LLM.Ollama => services.AddSingleton<TextEmbedder, OllamaTextEmbedder>().AddSingleton<Chat, OllamaChat>(),
            LLM.Fake => services.AddSingleton<TextEmbedder, FakeTextEmbedder>().AddSingleton<Chat, FakeChat>(),
            _ => throw new($"Unknown LLM or unsupported mode: {options.Get<LLMsOptions>().Mode}")
        };
}