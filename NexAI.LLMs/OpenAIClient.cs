using System.Text.Json;
using NexAI.Config;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Images;

namespace NexAI.LLMs;

public sealed class OpenAIClient
{
    private readonly ChatClient _chatClient;
    private readonly AudioClient _audioClient;
    private readonly ImageClient _imageClient;
    private readonly EmbeddingClient _embeddingClient;

    public OpenAIClient(Options options)
    {
        var apiKey = options.Get<OpenAIOptions>().ApiKey;
        _chatClient = new("gpt-4.1", apiKey);
        _audioClient = new("whisper-1", apiKey);
        _imageClient = new("gpt-image-1", apiKey);
        _embeddingClient = new("text-embedding-3-small", apiKey); // 1536 dimensions
    }

    public async Task<string> Ask(string message, string systemMessage, ChatCompletionOptions? chatCompletionOptions = null)
    {
        chatCompletionOptions ??= new();
        chatCompletionOptions.Temperature = 0f;

        List<ChatMessage> messages =
        [
            ChatMessage.CreateUserMessage(message),
            ChatMessage.CreateSystemMessage(systemMessage),
        ];
        var result = await _chatClient.CompleteChatAsync(messages, chatCompletionOptions);
        return result?.Value?.Content[0]?.Text ?? string.Empty;
    }

    public async Task<TAnswer> Ask<TAnswer>(string message, string systemMessage, ChatCompletionOptions? chatCompletionOptions = null)
    {
        chatCompletionOptions ??= new();
        chatCompletionOptions.Temperature = 0f;

        List<ChatMessage> messages =
        [
            ChatMessage.CreateUserMessage(message),
            ChatMessage.CreateSystemMessage(systemMessage)
        ];
        var result = await _chatClient.CompleteChatAsync(messages, chatCompletionOptions);
        var text = result?.Value?.Content[0]?.Text ?? string.Empty;
        return JsonSerializer.Deserialize<TAnswer>(text) ?? throw new($"AI did not return valid result. Expected JSON, got: {text}");
    }

    public async Task<string> Ask(BinaryData[] images, string message, string systemMessage)
    {
        List<ChatMessage> messages = [];
        messages.AddRange(images.Select(image => ChatMessage.CreateUserMessage(ChatMessageContentPart.CreateImagePart(image, "image/jpg"))));
        messages.Add(ChatMessage.CreateUserMessage(message));
        messages.Add(ChatMessage.CreateSystemMessage(systemMessage));
        var result = await _chatClient.CompleteChatAsync(messages);
        return result?.Value?.Content[0]?.Text ?? string.Empty;
    }

    public async Task<ChatCompletion> Chat(List<ChatMessage> messages, ChatCompletionOptions? chatCompletionOptions = null)
    {
        var result = await _chatClient.CompleteChatAsync(messages, chatCompletionOptions);
        if (result?.Value is null)
        {
            throw new("AI did not return any result.");
        }

        return result.Value;
    }

    public async Task<string> Transcribe(string filePath)
    {
        AudioTranscription transcription = await _audioClient.TranscribeAudioAsync(filePath);
        return transcription.Text;
    }

    public async Task<Uri> GenerateImage(string prompt)
    {
        var result = await _imageClient.GenerateImageAsync(prompt);
        return result.Value.ImageUri;
    }

    public async Task<float[]> GenerateEmbedding(string text)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
        return embedding.Value.ToFloats().ToArray();
    }
}