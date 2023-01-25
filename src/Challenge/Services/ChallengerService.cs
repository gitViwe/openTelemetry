using Grpc.Core;
using Shared.Protos;

namespace Challenge.Services;

public class ChallengerService : Challenger.ChallengerBase
{
    public override Task<ChallengeReply> FaceChallenge(ChallengeRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ChallengeReply
        {
            Message = "Well done " + request.Name + ". You have decided to face the challenge."
        });
    }
}