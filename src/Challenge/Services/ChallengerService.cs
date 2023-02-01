using Grpc.Core;
using Shared.Protos;

namespace Challenge.Services;

public class ChallengerService : Challenger.ChallengerBase
{
    public override async Task<ChallengeReply> FaceChallenge(ChallengeRequest request, ServerCallContext context)
    {
        await Task.Delay(500);

        if (new Random().Next(1, 101) >= 75)
        {
            throw new RpcException(new Status(StatusCode.Unknown, detail: "Some detail about the cause of the failure."));
        }

        return new ChallengeReply
        {
            Message = "Well done " + request.Name + ". You have decided to face the challenge."
        };
    }
}