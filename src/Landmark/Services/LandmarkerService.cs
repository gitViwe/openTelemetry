using Grpc.Core;
using Shared.Protos;

namespace Landmark.Services;

public class LandmarkerService : Landmarker.LandmarkerBase
{
    public override Task<LandmarkReply> ReachLandmark(LandmarkRequest request, ServerCallContext context)
    {
        return Task.FromResult(new LandmarkReply
        {
            Message = "Well done " + request.Name + ". You have reached a landmark."
        });
    }
}