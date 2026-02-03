using Microsoft.AspNetCore.Mvc;
using NexAI.LLMs.Common;

namespace NexAI.Api.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] Chat chat, [FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        var messages = request.Messages.Select(message => new ChatMessage(message.Role, message.Content)).ToArray();
        return request.Stream
            ? Ok(chat.StreamNextResponse(messages, cancellationToken))
            : Ok(await chat.GetNextResponse(messages, cancellationToken));
    }
}

public record ChatRequest(ChatRequest.Message[] Messages, bool Stream = false)
{
    public record Message(string Role, string Content);
}