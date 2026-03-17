using GPVBlazor.Models;

namespace GPVBlazor.Services.Interfaces
{
    public interface INetworkAnalysisFilterService
    {
        List<NetworkAnalysisItem> FilterItems(
            IEnumerable<NetworkAnalysisItem> items,
            string searchQuery,
            string selectedCategory,
            string selectedType,
            string selectedSort);
    }
}
