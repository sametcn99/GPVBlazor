using GPVBlazor.Models;

namespace GPVBlazor.Services.Interfaces
{
    public interface INetworkAnalysisService
    {
        NetworkAnalysisResult BuildAnalysis(IEnumerable<User> followers, IEnumerable<User> following);
    }
}
