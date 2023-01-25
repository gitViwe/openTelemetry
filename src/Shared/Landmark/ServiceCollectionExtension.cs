using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Landmark;

public static class ServiceCollectionExtension
{
    const string LANDMARK_SOURCE_NAME = "Landmark";

    public static void AddLandmarkOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(LANDMARK_SOURCE_NAME)
                   .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(LANDMARK_SOURCE_NAME))
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation()
                   .AddZipkinExporter(options =>
                   {
                       options.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                   })
                   .AddJaegerExporter(options =>
                   {
                       options.AgentHost = "localhost";
                       options.AgentPort = 6831;
                   });
        }).StartWithHost();
    }
}
