using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NexAI.Config;

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