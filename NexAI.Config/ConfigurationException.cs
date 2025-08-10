namespace NexAI.Config;

public class ConfigurationException(IEnumerable<string?> validationResults)
    : Exception(string.Join(Environment.NewLine, validationResults));