using Microsoft.AspNetCore.Mvc;
using NexAI.Agents;
using NexAI.LLMs.Common;

namespace NexAI.Api.Controllers;

[ApiController]
[Route("agent")]
public class AgentController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] INexAIAgent nexAIAgent, [FromBody] AgentRequest request, CancellationToken cancellationToken)
    {
        nexAIAgent.StartNewChat(request.Messages.Select(message => new ChatMessage(message.Role, message.Content)).ToArray());
        return request.Stream
            ? Ok(nexAIAgent.StreamResponse(cancellationToken))
            : Ok(await nexAIAgent.GetResponse(cancellationToken));
    }
}

public record AgentRequest(AgentRequest.Message[] Messages, bool Stream = false)
{
    public record Message(string Role, string Content);
}