using Microsoft.AspNetCore.Mvc;
using NexAI.Agents;

namespace NexAI.Api.Controllers;

[ApiController]
[Route("agent")]
public class AgentController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Get(
        [FromServices] NexAIAgent nexAIAgent,
        [FromBody] AgentDto dto,
        CancellationToken cancellationToken)
    {
        nexAIAgent.StartNewConversation(dto.Messages.Select(message => new NexAIAgent.Message(message.Role, message.Content)).ToArray());
        return Ok(await nexAIAgent.GetResponse(cancellationToken));
    }
}

public record AgentDto(AgentDto.Message[] Messages)
{
    public record Message(string Role, string Content);
}