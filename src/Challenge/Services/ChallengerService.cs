using Grpc.Core;
using Shared;
using Shared.Protos;
using System.Diagnostics;

namespace Challenge.Services;

public class ChallengerService : Challenger.ChallengerBase
{
    private static readonly Random _random = new();
    private static readonly ActivitySource ActivitySource = new ActivitySource(nameof(ChallengerService));
    private readonly ILogger<ChallengerService> _logger;
    private readonly IEnumerable<SuperHeroResponse> _superHeroes;

    public ChallengerService(ILogger<ChallengerService> logger, SuperHeroData heroData)
    {
        _logger = logger;
        _superHeroes = heroData.GetEnumerableAsync().Result;
    }

    public override async Task<ChallengeReply> FaceChallenge(ChallengeRequest request, ServerCallContext context)
    {
        if (_random.Next(1, 101) >= 75)
        {
            var superHero = _superHeroes.RandomElement();
            throw new RpcException(new Status(StatusCode.Unknown, detail: $"{request.Name} is defeated by {superHero.Name}."));
        }
        else
        {
            using var activity = ActivitySource.StartActivity("challenge service execution", ActivityKind.Server);
            await StartChallenge(request.Name, activity);

            return new ChallengeReply
            {
                Message = "Well done " + request.Name + ". You have faced epic challenges."
            };
        }
    }

    private async Task StartChallenge(string name, Activity? activity)
    {
        ChallengeHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);
        ChallengeHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);
        ChallengeHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);

        static void ChallengeHero(SuperHeroResponse superHero, string name, Activity? activity)
        {
            activity?.AddEvent(new ActivityEvent($"{name} is victorious against {superHero.Name}."));
        }
    }
}