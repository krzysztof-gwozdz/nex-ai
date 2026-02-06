// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using NexAI.Config;

namespace NexAI.DataProcessor;

public class DataProcessorOptions : IOptions
{
    public bool Recreate { get; init; }
}