using Grpc.Core;
using Shared;
using Shared.Protos;
using System.Diagnostics;
using System.Xml.Linq;

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
        _superHeroes = heroData.GetEnumerableAsync().Result.Where(x => !string.IsNullOrWhiteSpace(x.Connections.GroupAffiliation));
    }

    public override async Task<ChallengeReply> FaceChallenge(ChallengeRequest request, ServerCallContext context)
    {
        using var activity = ActivitySource.StartActivity("challenge service execution", ActivityKind.Server);
        var winCount = await StartChallenge(request.Name, activity);

        if (winCount < 2)
        {
            string message = $"{request.Name}'s team lost the challenge with only {winCount} win(s).";
            _logger.LogError("Challenge lost. {message}", message);
            throw new RpcException(new Status(StatusCode.Unknown, detail: message));
        }

        return new ChallengeReply
        {
            Message = "Well done " + request.Name + ". You have faced epic challenges."
        };
    }

    private async Task<int> StartChallenge(string name, Activity? activity)
    {
        int winCount = 0;

        ChallengeHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);
        ChallengeHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);
        ChallengeHero(_superHeroes.RandomElement(), name, activity);
        await Task.Delay(500);

        void ChallengeHero(SuperHeroResponse superHero, string name, Activity? activity)
        {
            bool victory = new Random().Next(150) > superHero.Powerstats.Power;
            string result = victory
                ? " was victorious against "
                : " was defeated by ";

            activity?.AddEvent(new ActivityEvent($"{name}'s team, {result} {superHero.Name} from {superHero.Connections.GroupAffiliation}."));

            if (victory)
            {
                winCount ++;
            }
        }

        return winCount;
    }
}