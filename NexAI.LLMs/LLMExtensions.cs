using Microsoft.Extensions.DependencyInjection;
using NexAI.Config;
using NexAI.LLMs.Common;
using NexAI.LLMs.Fake;
using NexAI.LLMs.Langfuse;
using NexAI.LLMs.MongoDb;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace NexAI.LLMs;

public static class LLMExtensions
{
    public static IServiceCollection AddLLM(this IServiceCollection services, Options options) =>
        services.AddSingleton<PromptReader>()
            .AddScoped<ConversationMongoDbStructure>()
            .AddScoped<ConversationMongoDbCollection>()
            .AddLLMSpecificServices(options.Get<LLMsOptions>().Mode);

    private static IServiceCollection AddLLMSpecificServices(this IServiceCollection services, string mode) =>
        LLM.ForAll(mode,
            () => services.AddScoped<TextEmbedder, OpenAITextEmbedder>()
                .AddScoped<OpenAIChat>()
                .AddScoped<Chat>(sp => new MongoDbChatDecorator(
                    new LangfuseChatDecorator(
                        sp.GetRequiredService<OpenAIChat>(), sp.GetRequiredService<IOtelLangfuseTrace>()),
                    sp.GetRequiredService<ConversationMongoDbCollection>())
                ),
            () => services.AddScoped<TextEmbedder, OllamaTextEmbedder>()
                .AddScoped<OllamaChat>()
                .AddScoped<Chat>(sp => new MongoDbChatDecorator(
                    new LangfuseChatDecorator(
                        sp.GetRequiredService<OllamaChat>(), sp.GetRequiredService<IOtelLangfuseTrace>()),
                    sp.GetRequiredService<ConversationMongoDbCollection>())
                ),
            () => services.AddScoped<TextEmbedder, FakeTextEmbedder>()
                .AddScoped<FakeChat>()
                .AddScoped<Chat>(sp => new MongoDbChatDecorator(
                    new LangfuseChatDecorator(
                        sp.GetRequiredService<FakeChat>(), sp.GetRequiredService<IOtelLangfuseTrace>()),
                    sp.GetRequiredService<ConversationMongoDbCollection>())
                )
        );
}