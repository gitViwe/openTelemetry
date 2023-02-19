using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Challenge;

public static class ServiceCollectionExtension
{
    const string CHALLANGE_SOURCE_NAME = "Challenge";
    const string CHALLANGE_SERVICE_SOURCE_NAME = "ChallengerService";

    public static void AddChallengeOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(CHALLANGE_SOURCE_NAME, CHALLANGE_SERVICE_SOURCE_NAME)
                   .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(CHALLANGE_SOURCE_NAME))
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                   .AddJaegerExporter(options =>
                   {
                       options.AgentHost = "jaeger";
                       options.AgentPort = 6831;
                   });
        }).StartWithHost();
    }

    public static IServiceCollection AddChallengeSeqLogging(this IServiceCollection services)
    {
        return services.AddLogging(builder => builder.AddSeq("http://seq:5341"));
    }
}
