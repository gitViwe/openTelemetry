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
        // will propagate context via the message headers
        var headers = new Dictionary<string, object>();

        // return the new activity to add tags to the span
        using var activity = PropagateContext(headers);
        activity?.SetTag("api.user.id", entry.Id);
        activity?.SetTag("api.user.username", entry.Username);

        var endpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://rabbitmq/journeyQueue"));
        await endpoint.Send(new JourneyMessage(entry.Id, entry.Username, entry.CreatedAt), context =>
        {
            foreach (var item in headers)
            {
                // add headers to message so they can be extracted on the consumer
                context.Headers.Set(item.Key, item.Value, overwrite: true);
            }
        }, cancellationToken);
    }

    private static Activity? PropagateContext(Dictionary<string, object> headers)
    {
        var activity = ActivitySource.StartActivity("journey message publisher", ActivityKind.Producer);
        activity?.AddEvent(new ActivityEvent("Propagating the activity context via message headers."));
        ActivityContext contextToInject = activity?.Context ?? Activity.Current?.Context ?? default;

        // Inject the ActivityContext into the message headers to propagate trace context to the receiving service.
        Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), headers, InjectTraceContext);

        static void InjectTraceContext(Dictionary<string, object> headers, string key, string value)
        {
            headers[key] = value;
        }

        return activity;
    }
}
