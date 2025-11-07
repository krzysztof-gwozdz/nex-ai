using NexAI.Config;

namespace NexAI.Git;

public readonly record struct GitAuthorId
{
    public Guid Value { get; init; }

    public GitAuthorId(Guid value)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value);
        Value = value;
    }

    public static GitAuthorId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(GitAuthorId id) => id.Value;

    public static implicit operator string(GitAuthorId id) => id.Value.ToString();
}