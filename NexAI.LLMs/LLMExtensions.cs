using Microsoft.Extensions.DependencyInjection;
using NexAI.Config;
using NexAI.LLMs.Common;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs;

public static class LLMExtensions
{
    public static IServiceCollection AddLLM(this IServiceCollection services, Options options) =>
        options.Get<LLMsOptions>().Mode switch
        {
            LLM.OpenAI => services.AddSingleton<TextEmbedder, OpenAITextEmbedder>().AddSingleton<Chat, OpenAIChat>(),
            LLM.Ollama => services.AddSingleton<TextEmbedder, OllamaTextEmbedder>().AddSingleton<Chat, OllamaChat>(),
            _ => throw new($"Unknown LLM or unsupported mode: {options.Get<LLMsOptions>().Mode}")
        };
}