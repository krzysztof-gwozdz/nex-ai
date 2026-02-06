using NexAI.Config;
using NexAI.Langfuse;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using zborek.Langfuse.OpenTelemetry;

namespace NexAI.Api;

public static class ObservabilityExtension
{
    public static IServiceCollection AddObservability(this IServiceCollection services, Options options, string applicationName)
    {
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(applicationName))
            .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddAspNetCoreInstrumentation(traceInstrumentationOptions =>
                {
                    traceInstrumentationOptions.Filter = context =>
                    {
                        var path = context.Request.Path.Value;
                        return path is null || !path.StartsWith("/health", StringComparison.OrdinalIgnoreCase);
                    };
                })
                .AddHttpClientInstrumentation()
                .AddLangfuseExporter(langfuseOtlpExporterOptions =>
                {
                    var langfuseOptions = options.Get<LangfuseOptions>();
                    langfuseOtlpExporterOptions.PublicKey = langfuseOptions.PublicKey;
                    langfuseOtlpExporterOptions.SecretKey = langfuseOptions.SecretKey;
                    langfuseOtlpExporterOptions.Url = langfuseOptions.Url;
                })
                .AddOtlpExporter())
            .WithMetrics(meterProviderBuilder => meterProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter());
        return services;
    }

    public static ILoggingBuilder AddObservability(this ILoggingBuilder logging, string applicationName)
    {
        logging.AddOpenTelemetry(openTelemetryLoggerOptions => openTelemetryLoggerOptions
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName))
            .AddOtlpExporter());
        return logging;
    }
}