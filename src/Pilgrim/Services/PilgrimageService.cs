using Grpc.Core;
using Shared.Protos;

namespace Pilgrim.Services
{
    public class PilgrimageService : Pilgrimage.PilgrimageBase
    {
        public override Task<PilgrimReply> EndPilgrimage(PilgrimRequest request, ServerCallContext context)
        {
            return Task.FromResult(new PilgrimReply
            {
                Message = "Well done " + request.Name + ". Your journe has come to an end."
            });
        }
    }
}