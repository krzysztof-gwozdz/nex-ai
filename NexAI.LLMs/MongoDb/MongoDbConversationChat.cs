using System.Runtime.CompilerServices;
using MongoDB.Driver;
using NexAI.LLMs.Common;

namespace NexAI.LLMs.MongoDb;

public class MongoDbConversationChat(Chat chat, ConversationMongoDbCollection collection) : Chat
{
    public override async Task<string> Ask(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken)
    {
        var messages = new ChatMessage[] { new("system", systemMessage), new("user", message) };
        var response = await chat.Ask(conversationId, systemMessage, message, cancellationToken);
        await UpsertConversationIfCurrent(conversationId, messages, response, cancellationToken);
        return response;
    }

    public override async Task<TResponse> Ask<TResponse>(ConversationId conversationId, string systemMessage, string message, CancellationToken cancellationToken)
    {
        var messages = new ChatMessage[] { new("system", systemMessage), new("user", message) };
        var response = await chat.Ask<TResponse>(conversationId, systemMessage, message, cancellationToken);
        await UpsertConversationIfCurrent(conversationId, messages, response.ToString() ?? string.Empty, cancellationToken);
        return response;
    }

    public override async IAsyncEnumerable<string> AskStream(ConversationId conversationId, string systemMessage, string message, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var messages = new ChatMessage[] { new("system", systemMessage), new("user", message) };
        var fullResponse = new List<string>();
        await foreach (var chunk in chat.AskStream(conversationId, systemMessage, message, cancellationToken))
        {
            fullResponse.Add(chunk);
            yield return chunk;
        }
        var response = string.Concat(fullResponse);
        await UpsertConversationIfCurrent(conversationId, messages, response, cancellationToken);
    }

    public override async Task<string> GetNextResponse(ConversationId conversationId, ChatMessage[] messages, CancellationToken cancellationToken)
    {
        var response = await chat.GetNextResponse(conversationId, messages, cancellationToken);
        await UpsertConversationIfCurrent(conversationId, messages, response, cancellationToken);
        return response;
    }

    public override async IAsyncEnumerable<string> StreamNextResponse(ConversationId conversationId, ChatMessage[] messages, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var fullResponse = new List<string>();
        await foreach (var chunk in chat.StreamNextResponse(conversationId, messages, cancellationToken))
        {
            fullResponse.Add(chunk);
            yield return chunk;
        }
        var response = string.Concat(fullResponse);
        await UpsertConversationIfCurrent(conversationId, messages, response, cancellationToken);
    }

    private async Task UpsertConversationIfCurrent(ConversationId conversationId, ChatMessage[] messages, string assistantResponse, CancellationToken cancellationToken)
    {
        var allMessages = messages.Append(new ChatMessage("assistant", assistantResponse)).ToArray();
        var conversation = new Conversation(conversationId, allMessages);
        var document = ConversationMongoDbDocument.Create(conversation);

        var filter = Builders<ConversationMongoDbDocument>.Filter.Eq(d => d.Id, conversationId);
        await collection.Collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }
}