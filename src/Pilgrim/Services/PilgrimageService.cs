using Grpc.Core;
using Shared.Protos;

namespace Pilgrim.Services;

public class PilgrimageService : Pilgrimage.PilgrimageBase
{
    public override async Task<PilgrimReply> EndPilgrimage(PilgrimRequest request, ServerCallContext context)
    {
        await Task.Delay(500);

        if (new Random().Next(1, 101) >= 75)
        {
            throw new RpcException(new Status(StatusCode.Unknown, detail: "Some detail about the cause of the failure."));
        }

        return new PilgrimReply
        {
            Message = "Well done " + request.Name + ". Your journe has come to an end."
        };
    }
}