using Grpc.Core;
using Shared;
using Shared.Protos;
using System.Diagnostics;

namespace Pilgrim.Services;

public class PilgrimageService : Pilgrimage.PilgrimageBase
{
    private static readonly Random _random = new();
    private static readonly ActivitySource ActivitySource = new ActivitySource(nameof(PilgrimageService));
    private readonly IEnumerable<SuperHeroResponse> _superHeroes;

    public PilgrimageService(SuperHeroData heroData)
    {
        _superHeroes = heroData.GetEnumerableAsync().Result.Where(x => !string.IsNullOrWhiteSpace(x.Biography.PlaceOfBirth));
    }

    public override Task<PilgrimReply> EndPilgrimage(PilgrimRequest request, ServerCallContext context)
    {
        using var activity = ActivitySource.StartActivity("pilgrim service execution", ActivityKind.Server);
        var superHero = _superHeroes.RandomElement();
        activity?.AddEvent(new ActivityEvent($"{request.Name} reached the landmark {superHero.Biography.PlaceOfBirth}."));

        return Task.FromResult(new PilgrimReply
        {
            Message = "Well done " + request.Name + ". Your journey has come to an end."
        });
    }
}