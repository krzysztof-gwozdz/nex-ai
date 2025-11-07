using NexAI.Config;

namespace NexAI.Git;

public readonly record struct GitCommitId
{
    public Guid Value { get; init; }

    public GitCommitId(Guid value)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value);
        Value = value;
    }

    public static GitCommitId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(GitCommitId id) => id.Value;

    public static implicit operator string(GitCommitId id) => id.Value.ToString();
}