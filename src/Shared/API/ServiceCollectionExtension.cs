﻿using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                           string[] urls =
                           {
                               "/swagger/v1/swagger.json",
                               "/_vs/browserLink",
                               "/_framework/aspnetcore-browser-refresh.js",
                               "/swagger/index.html",
                               "/swagger/favicon-32x32.png",
                               "/favicon.ico",
                               "/swagger/swagger-ui-bundle.js",
                               "/swagger/swagger-ui-bundle.js",
                               "/swagger/swagger-ui.css",
                               "/"
                           };
                           return !urls.Contains(context.Request.Path.Value);
                       };

                       options.RecordException = true;
                   })
                   .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
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

    public static IServiceCollection AddAPISeqLogging(this IServiceCollection services)
    {
        return services.AddLogging(builder => builder.AddSeq("http://seq:5341", null));
    }
}
