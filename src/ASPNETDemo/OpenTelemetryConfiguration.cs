using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Diagnostics;

namespace ASPNETDemo;

internal static class OpenTelemetryConfiguration
{
    internal static string _serviceName = "Demo.Console";
    internal static string _serviceVersion = "1.0.0";
    internal static ActivitySource GetActivitySource()
    {
        // Define some important constants to initialize tracing with
        return new ActivitySource(_serviceName, _serviceVersion);
    }

    internal static IServiceCollection AddOpenTelemetryTracingDemo(this IServiceCollection services)
    {
        // Configure important OpenTelemetry settings, the console exporter, and instrumentation library
        services.AddOpenTelemetryTracing(tracerProvider =>
        {
            tracerProvider
            .AddSource(_serviceName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(_serviceName, serviceVersion: _serviceVersion))
            .AddZipkinExporter(options =>
            {
                // not needed, it's the default
                options.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
            })
            .AddConsoleExporter()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();
        });

        return services;
    }
}
