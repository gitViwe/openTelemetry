using Grpc.Core;
using MassTransit;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Shared;
using Shared.Protos;
using System.Diagnostics;

namespace Orchestrator.MessageConsumer;

public class JourneyStartConsumer : IConsumer<JourneyMessage>
{
    private static readonly ActivitySource ActivitySource = new ActivitySource(nameof(JourneyStartConsumer));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    private readonly ILogger<JourneyStartConsumer> _logger;
    private readonly Landmarker.LandmarkerClient _landmarkerClient;
    private readonly Challenger.ChallengerClient _challengerClient;
    private readonly Pilgrimage.PilgrimageClient _pilgrimageClient;

    public JourneyStartConsumer(
        ILogger<JourneyStartConsumer> logger,
        Landmarker.LandmarkerClient landmarkerClient,
        Challenger.ChallengerClient challengerClient,
        Pilgrimage.PilgrimageClient pilgrimageClient)
    {
        _logger = logger;
        _landmarkerClient = landmarkerClient;
        _challengerClient = challengerClient;
        _pilgrimageClient = pilgrimageClient;
    }

    public async Task Consume(ConsumeContext<JourneyMessage> context)
    {
        try
        {
            using var activity = PropagateContext(context.Headers.GetAll());
            activity?.AddEvent(new ActivityEvent("Propagating the activity context via message headers."));

            // call service 01
            activity?.AddEvent(new ActivityEvent("Starting the request to the landmark service."));
            var landmarkResponse = await _landmarkerClient.ReachLandmarkAsync(new LandmarkRequest { Name = context.Message.Username });
            activity?.SetTag("orchestrator.landmark.response.message", landmarkResponse.Message);

            // call service 02
            activity?.AddEvent(new ActivityEvent("Starting the request to the challenge service."));
            var challengeResponse = await _challengerClient.FaceChallengeAsync(new ChallengeRequest { Name = context.Message.Username });
            activity?.SetTag("orchestrator.challenge.response.message", challengeResponse.Message);

            // call service 03
            activity?.AddEvent(new ActivityEvent("Starting the request to the pilgrim service."));
            var pilgrimResponse = await _pilgrimageClient.EndPilgrimageAsync(new PilgrimRequest { Name = context.Message.Username });
            activity?.SetTag("orchestrator.pilgrim.response.message", pilgrimResponse.Message);

            activity?.AddEvent(new ActivityEvent("Orchestration complete!"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to orchestrate journey message");
            throw;
        }
    }

    private Activity? PropagateContext(IEnumerable<KeyValuePair<string, object>> headers)
    {
        // Extract the PropagationContext of the upstream parent from the message headers
        var parentContext = Propagator.Extract(default, headers, ExtractTraceContext);

        // Inject extracted info into current context
        Baggage.Current = parentContext.Baggage;

        return ActivitySource.StartActivity("journey message orchestration", ActivityKind.Consumer, parentContext.ActivityContext);
    }

    private IEnumerable<string> ExtractTraceContext(IEnumerable<KeyValuePair<string, object>> headers, string key)
    {
        try
        {
            return headers.Select(x => x.Value.ToString()).ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract trace context");
        }

        return Enumerable.Empty<string>();
    }
}
