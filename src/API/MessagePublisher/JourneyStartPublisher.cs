using API.Persistance;
using MassTransit;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Shared;
using System.Diagnostics;

namespace API.MessagePublisher;

public class JourneyStartPublisher
{
    private readonly IBus _bus;
    private static readonly ActivitySource ActivitySource = new ActivitySource(nameof(JourneyStartPublisher));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    public JourneyStartPublisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync(JourneyEntry entry, CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, object>();
        PropagateContext(headers);

        var endpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://rabbitmq/journeyQueue"));
        await endpoint.Send(new JourneyMessage(entry.Id, entry.Username, entry.CreatedAt), context =>
        {
            foreach (var item in headers)
            {
                context.Headers.Set(item.Key, item.Value, overwrite: true);
            }
        }, cancellationToken);
    }

    private static void PropagateContext(Dictionary<string, object> headers)
    {
        using var activity = ActivitySource.StartActivity("journey message publishing", ActivityKind.Producer);
        ActivityContext contextToInject = activity?.Context ?? Activity.Current?.Context ?? default;

        // Inject the ActivityContext into the message headers to propagate trace context to the receiving service.
        Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), headers, InjectTraceContext);

        static void InjectTraceContext(Dictionary<string, object> headers, string key, string value)
        {
            headers[key] = value;
        }
    }
}
