using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Challenge;

public static class ServiceCollectionExtension
{
    const string CHALLANGE_SOURCE_NAME = "Challenge";

    public static void AddChallengeOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(CHALLANGE_SOURCE_NAME)
                   .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(CHALLANGE_SOURCE_NAME))
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
