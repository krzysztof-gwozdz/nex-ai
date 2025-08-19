using NexAI.Config;

namespace NexAI.DataImporter;

public class DataImporterOptions : IOptions
{
    public bool CreateSchema { get; init; }
    public bool ImportData { get; init; }
}