// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.Git;

public record GitOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string[] RepositoryPaths { get; init; } = [];
}
