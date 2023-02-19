using Grpc.Core;
using Shared;
using Shared.Protos;
using System.Diagnostics;

namespace Landmark.Services;

public class LandmarkerService : Landmarker.LandmarkerBase
{
    private static readonly Random _random = new();
    private static readonly ActivitySource ActivitySource = new ActivitySource(nameof(LandmarkerService));
    private readonly IEnumerable<SuperHeroResponse> _superHeroes;

    public LandmarkerService(SuperHeroData heroData)
    {
        _superHeroes = heroData.GetEnumerableAsync().Result;
    }

    public override async Task<LandmarkReply> ReachLandmark(LandmarkRequest request, ServerCallContext context)
    {
        using var activity = ActivitySource.StartActivity("landmark service execution", ActivityKind.Server);
        await StartLandmark(request.Name, activity);

        return new LandmarkReply
        {
            Message = "Well done " + request.Name + ". You have reached a landmark."
        };
    }

    private async Task StartLandmark(string name, Activity? activity)
    {
        VisitHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);
        VisitHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);
        VisitHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);

        static void VisitHero(SuperHeroResponse superHero, string name, Activity? activity)
        {
            activity?.AddEvent(new ActivityEvent($"{name} visited {superHero.Name} at {superHero.Work.Base}."));
        }
    }
}