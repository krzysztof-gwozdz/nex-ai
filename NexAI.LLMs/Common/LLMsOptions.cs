using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.LLMs.Common;

public class LLMsOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Mode { get; init; } = null!;

    [Required]
    public Promprts Prompts { get; init; } = null!;
}

public class Promprts
{
    [Required(AllowEmptyStrings = false)]
    public string ZendeskTicketSummary { get; init; } = null!;
}