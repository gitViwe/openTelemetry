using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Landmark;

public static class ServiceCollectionExtension
{
    const string LANDMARK_SOURCE_NAME = "Landmark";
    const string LANDMARK_SERVICE_SOURCE_NAME = "LandmarkerService";

    public static void AddLandmarkOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(LANDMARK_SOURCE_NAME, LANDMARK_SERVICE_SOURCE_NAME)
                   .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(LANDMARK_SOURCE_NAME))
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                   .AddJaegerExporter(options =>
                   {
                       options.AgentHost = "jaeger";
                       options.AgentPort = 6831;
                   });
        }).StartWithHost();
    }

    public static IServiceCollection AddLandmarkSeqLogging(this IServiceCollection services)
    {
        return services.AddLogging(builder => builder.AddSeq("http://seq:5341"));
    }
}
