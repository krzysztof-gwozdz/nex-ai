using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NexAI.Zendesk;

public readonly record struct ZendeskGroupId
{
    public Guid Value { get; init; }

    public ZendeskGroupId(Guid value)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value);
        Value = value;
    }

    public static ZendeskGroupId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ZendeskGroupId id) => id.Value;

    public static implicit operator string(ZendeskGroupId id) => id.Value.ToString();
}

public static class ArgumentExceptionExtensions
{
    public static void ThrowIfNullOrEmpty([NotNull] Guid? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null || argument == Guid.Empty)
        {
            throw new ArgumentException("Argument cannot be null or empty", paramName);
        }
    }
}