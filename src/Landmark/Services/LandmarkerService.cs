using Grpc.Core;
using Shared.Protos;

namespace Landmark.Services;

public class LandmarkerService : Landmarker.LandmarkerBase
{
    public override async Task<LandmarkReply> ReachLandmark(LandmarkRequest request, ServerCallContext context)
    {
        await Task.Delay(500);

        if (new Random().Next(1, 101) >= 75)
        {
            throw new RpcException(new Status(StatusCode.Unknown, detail: "Some detail about the cause of the failure."));
        }

        return new LandmarkReply
        {
            Message = "Well done " + request.Name + ". You have reached a landmark."
        };
    }
}