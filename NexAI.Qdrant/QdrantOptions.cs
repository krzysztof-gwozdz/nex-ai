using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.Qdrant;

public record QdrantOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Host { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public int Port { get; init; }
}