using Grpc.Core;
using Shared.Protos;

namespace Pilgrim.Services
{
    public class PilgrimageService : Pilgrimage.PilgrimageBase
    {
        public override Task<PilgrimReply> EndPilgrimage(PilgrimRequest request, ServerCallContext context)
        {
            //throw new RpcException(new Status(StatusCode.Aborted, detail: "Some detail about the cause of the failure."));
            return Task.FromResult(new PilgrimReply
            {
                Message = "Well done " + request.Name + ". Your journe has come to an end."
            });
        }
    }
}