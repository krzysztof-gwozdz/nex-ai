using Microsoft.AspNetCore.Mvc;
using NexAI.LLMs.Common;

namespace NexAI.Api.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> NewChat([FromServices] Chat chat, [FromBody] NewChatRequest request, CancellationToken cancellationToken)
    {
        var messages = request.Messages.Select(message => new ChatMessage(message.Role, message.Content)).ToArray();
        return request.Stream
            ? Ok(chat.StreamNextResponse(new ConversationId(request.ConversationId), messages, cancellationToken))
            : Ok(await chat.GetNextResponse(new ConversationId(request.ConversationId), messages, cancellationToken));
    }
}

public record NewChatRequest(Guid ConversationId, NewChatRequest.Message[] Messages, bool Stream = false)
{
    public record Message(string Role, string Content);
}