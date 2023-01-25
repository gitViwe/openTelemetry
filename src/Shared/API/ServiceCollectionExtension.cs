using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.API;

public static class ServiceCollectionExtension
{
    const string API_SOURCE_NAME = "JourneyAPI";
    const string API_PUBLISHER_SOURCE_NAME = "JourneyStartPublisher";
    const string MASS_TRANSIT_SOURCE_NAME = "MassTransit";

    public static void AddAPIOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(API_SOURCE_NAME, API_PUBLISHER_SOURCE_NAME, MASS_TRANSIT_SOURCE_NAME)
                   .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(API_SOURCE_NAME))
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation(options =>
                   {
                       options.Filter = (context) =>
                       {
                           // filter out these paths
                           string[] urls = { "/swagger/v1/swagger.json", "/_vs/browserLink", "/_framework/aspnetcore-browser-refresh.js", "/swagger/index.html" };
                           return !urls.Contains(context.Request.Path.Value);
                       };
                   })
                   .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
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

    public static IServiceCollection AddAPICors(this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        // allow requests from these URLs
                        .WithOrigins(new string[]
                        {
                            "https://localhost:7277",
                            "http://localhost:5177",
                            "https://localhost:7114",
                            "http://localhost:5043"
                        });
                });
        });
    }

    public static IServiceCollection AddAPIMassTransit(this IServiceCollection services)
    {
        return services.AddMassTransit(configure =>
        {
            configure.UsingRabbitMq((context, config) =>
            {
                config.Host("rabbitmq", host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });
            });
        });
    }
}
