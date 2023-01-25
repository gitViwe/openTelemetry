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
        using var activity = PropagateContext(context.Headers.GetAll());

        // call service 01
        var landmarkResponse = await _landmarkerClient.ReachLandmarkAsync(new LandmarkRequest { Name = context.Message.Username });
        activity?.SetTag("orchestrator.landmark.response.message", landmarkResponse.Message);

        // call service 02
        var challengeResponse = await _challengerClient.FaceChallengeAsync(new ChallengeRequest { Name = context.Message.Username });
        activity?.SetTag("orchestrator.challenge.response.message", challengeResponse.Message);

        // call service 03
        var pilgrimResponse = await _pilgrimageClient.EndPilgrimageAsync(new PilgrimRequest { Name = context.Message.Username });
        activity?.SetTag("orchestrator.pilgrim.response.message", pilgrimResponse.Message);

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
