using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrator.MessageConsumer;
using Shared.Orchestrator;
using System.Reflection;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOrchestratorOpenTelemetry();
        services.AddLogging(builder => builder.AddSeq());
        services.RegisterLandMarkGrpcClient();

        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetExecutingAssembly());
            configure.UsingRabbitMq((context, config) =>
            {
                config.Host("rabbitmq", host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });
                config.ReceiveEndpoint("journeyQueue", endpoint =>
                {
                    endpoint.PrefetchCount = 16;
                    endpoint.ConfigureConsumer<JourneyStartConsumer>(context);
                });
            });
        });
    })
    .Build();

host.Run();
