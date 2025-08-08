using Microsoft.Extensions.Configuration;

namespace NexAI.Config;

public static class Configuration
{
    public static IConfigurationRoot Get() => new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile("appsettings.local.json", optional: true)
        .AddEnvironmentVariables()
        .Build();
}