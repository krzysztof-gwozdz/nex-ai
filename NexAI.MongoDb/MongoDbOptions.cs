using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.MongoDb;

public record MongoDbOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Host { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public int Port { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string Database { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Username { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Password { get; init; } = null!;
    
    public string ConnectionString => $"mongodb://{Username}:{Password}@{Host}:{Port}/{Database}";
}