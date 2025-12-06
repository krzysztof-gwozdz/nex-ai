using Microsoft.AspNetCore.Mvc;
using NexAI.LLMs.Common;

namespace NexAI.Api.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] Chat chat,
        [FromQuery] ChatDto dto,
        CancellationToken cancellationToken) =>
        Ok(await chat.Ask(dto.SystemMessage, dto.Message, cancellationToken));
}

public record ChatDto(string SystemMessage, string Message);