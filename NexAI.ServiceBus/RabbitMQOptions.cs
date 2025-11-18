using System.ComponentModel.DataAnnotations;
using NexAI.Config;

namespace NexAI.ServiceBus;

public record RabbitMQOptions : IOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Host { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public int Port { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string Username { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Password { get; init; } = null!;

    public string ConnectionString => $"host={Host}:{Port};username={Username};password={Password}";
}