using NexAI.Config;
using NexAI.LLMs.Ollama;
using NexAI.LLMs.OpenAI;

namespace NexAI.LLMs.Common;

public abstract class Chat
{   
    public abstract Task<string> Ask(string systemMessage, string message);
    
    public abstract IAsyncEnumerable<string> AskStream(string systemMessage, string message);
    
    public static Chat GetInstance(Options options) => options.Get<LLMsOptions>().Mode switch
    {
        LLM.OpenAI => new OpenAIChat(options),
        LLM.Ollama => new OllamaChat(options),
        _ => throw new($"Unknown LLM or unsupported mode: {options.Get<LLMsOptions>().Mode}")
    };
}