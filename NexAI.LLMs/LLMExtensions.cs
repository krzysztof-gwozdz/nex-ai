using Microsoft.Extensions.DependencyInjection;
using NexAI.Config;
using NexAI.LLMs.Common;
using NexAI.LLMs.Fake;
using NexAI.LLMs.MongoDb;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs;

public static class LLMExtensions
{
    public static IServiceCollection AddLLM(this IServiceCollection services, Options options) =>
        services.AddSingleton<PromptReader>()
            .AddSingleton<ConversationMongoDbStructure>()
            .AddSingleton<ConversationMongoDbCollection>()
            .AddLLMSpecificServices(options.Get<LLMsOptions>().Mode);

    private static IServiceCollection AddLLMSpecificServices(this IServiceCollection services, string mode) =>
        mode switch
        {
            LLM.OpenAI => services
                .AddSingleton<TextEmbedder, OpenAITextEmbedder>()
                .AddSingleton<OpenAIChat>()
                .AddSingleton<Chat>(sp => new MongoDbConversationChat(sp.GetRequiredService<OpenAIChat>(), sp.GetRequiredService<ConversationMongoDbCollection>())),
            LLM.Ollama => services
                .AddSingleton<TextEmbedder, OllamaTextEmbedder>()
                .AddSingleton<OllamaChat>()
                .AddSingleton<Chat>(sp => new MongoDbConversationChat(sp.GetRequiredService<OllamaChat>(), sp.GetRequiredService<ConversationMongoDbCollection>())),
            LLM.Fake => services
                .AddSingleton<TextEmbedder, FakeTextEmbedder>()
                .AddSingleton<FakeChat>()
                .AddSingleton<Chat>(sp => new MongoDbConversationChat(sp.GetRequiredService<FakeChat>(), sp.GetRequiredService<ConversationMongoDbCollection>())),
            _ => throw new($"Unknown LLM or unsupported mode: {mode}")
        };
}