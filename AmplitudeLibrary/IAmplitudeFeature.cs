using System.Threading.Tasks;

namespace AmplitudeFeatureLibrary
{
    public interface IAmplitudeFeature
    {
        bool FeatureIsEnabled(string flagName, string userId = "DEFAULT-USER");
        Task<bool> FeatureIsEnabledAsync(string flagName, string userId = "DEFAULT-USER");
    }
}