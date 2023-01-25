using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Pilgrim;

public static class ServiceCollectionExtension
{
    const string PILGRIM_SOURCE_NAME = "Pilgrim";

    public static void AddPilgrimOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(PILGRIM_SOURCE_NAME)
                   .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(PILGRIM_SOURCE_NAME))
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation()
                   .AddZipkinExporter(options =>
                   {
                       options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
                   })
                   .AddJaegerExporter(options =>
                   {
                       options.AgentHost = "jaeger";
                       options.AgentPort = 6831;
                   });
        }).StartWithHost();
    }
}
