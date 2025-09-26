using Microsoft.Extensions.Configuration;
using NexAI.Config;

namespace NexAI.LLMs.Tests;

public class LLMTestBase
{
    protected static Options GetOptions(string llmMode) =>
        new(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["LLMs:Mode"] = llmMode
            })
            .Build());
}