using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Pilgrim;

public static class ServiceCollectionExtension
{
    const string PILGRIM_SOURCE_NAME = "Pilgrim";
    const string PILGRIM_SERVICE_SOURCE_NAME = "PilgrimageService";

    public static void AddPilgrimOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(PILGRIM_SOURCE_NAME, PILGRIM_SERVICE_SOURCE_NAME)
                   .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(PILGRIM_SOURCE_NAME))
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                   .AddJaegerExporter(options =>
                   {
                       options.AgentHost = "jaeger";
                       options.AgentPort = 6831;
                   });
        }).StartWithHost();
    }

    public static IServiceCollection AddPilgrimSeqLogging(this IServiceCollection services)
    {
        return services.AddLogging(builder => builder.AddSeq("http://seq:5341"));
    }
}
