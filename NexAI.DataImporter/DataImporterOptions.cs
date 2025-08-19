using NexAI.Config;

namespace NexAI.DataImporter;

public class DataImporterOptions : IOptions
{
    public bool Recreate { get; init; }
}