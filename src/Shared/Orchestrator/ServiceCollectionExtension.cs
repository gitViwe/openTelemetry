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
                   .AddAspNetCoreInstrumentation(options =>
                   {
                       options.Filter = (context) =>
                       {
                           // filter out these paths
                           string[] urls = { "" };
                           return !urls.Contains(context.Request.Path.Value);
                       };
                   })
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

    public static void RegisterLandMarkGrpcClient(this IServiceCollection services)
    {
        services.AddGrpcClient<Landmarker.LandmarkerClient>(options =>
        {
            options.Address = new Uri("http://localhost:5251");
        });

        services.AddGrpcClient<Challenger.ChallengerClient>(options =>
        {
            options.Address = new Uri("http://localhost:5249");
        });

        services.AddGrpcClient<Pilgrimage.PilgrimageClient>(options =>
        {
            options.Address = new Uri("http://localhost:5235");
        });
    }
}
