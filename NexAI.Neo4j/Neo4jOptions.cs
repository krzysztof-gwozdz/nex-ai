// ReSharper disable InconsistentNaming

using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.Neo4j;

public record Neo4jOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ConnectionString { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Username { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Password { get; init; } = null!;
}