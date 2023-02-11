using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Protos;

namespace Shared.Orchestrator;

public static class ServiceCollectionExtension
{
    const string ORCHESTRATOR_CONSUMER_SOURCE_NAME = "JourneyStartConsumer";
    const string ORCHESTRATOR_SOURCE_NAME = "Orchestrator";
    const string MASS_TRANSIT_SOURCE_NAME = "MassTransit";

    public static void AddOrchestratorOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(ORCHESTRATOR_SOURCE_NAME, ORCHESTRATOR_CONSUMER_SOURCE_NAME, MASS_TRANSIT_SOURCE_NAME)
                   .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ORCHESTRATOR_SOURCE_NAME))
                   .AddHttpClientInstrumentation()
                   .AddGrpcClientInstrumentation(options => options.SuppressDownstreamInstrumentation = true)
                   .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                   .AddJaegerExporter(options =>
                   {
                       options.AgentHost = "jaeger";
                       options.AgentPort = 6831;
                   });
        }).StartWithHost();
    }

    public static void RegisterOrchestratorGrpcClient(this IServiceCollection services)
    {
        services.AddGrpcClient<Landmarker.LandmarkerClient>(options =>
        {
            options.Address = new Uri("http://landmark:5251");
        }).ConfigureChannel(options => options.ServiceConfig = ConfigureClient());

        services.AddGrpcClient<Challenger.ChallengerClient>(options =>
        {
            options.Address = new Uri("http://challenge:5249");
        }).ConfigureChannel(options => options.ServiceConfig = ConfigureClient());

        services.AddGrpcClient<Pilgrimage.PilgrimageClient>(options =>
        {
            options.Address = new Uri("http://pilgrim:5235");
        }).ConfigureChannel(options => options.ServiceConfig = ConfigureClient());

        static ServiceConfig ConfigureClient()
        {
            return new ServiceConfig()
            {
                MethodConfigs =
                {
                    new MethodConfig()
                    {
                        Names = { MethodName.Default },
                        RetryPolicy= new RetryPolicy()
                        {
                            MaxAttempts = 5,
                            InitialBackoff= TimeSpan.FromSeconds(2),
                            MaxBackoff = TimeSpan.FromSeconds(5),
                            BackoffMultiplier = 1.5,
                            RetryableStatusCodes = { Grpc.Core.StatusCode.Unknown, Grpc.Core.StatusCode.Unavailable }
                        }
                    }
                }

            };
        }
    }
}
