// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.LLMs.Common;

public class LLMsOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Mode { get; init; } = null!;
}