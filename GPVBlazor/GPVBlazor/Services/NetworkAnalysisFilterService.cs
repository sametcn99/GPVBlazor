using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

namespace GPVBlazor.Services
{
    public class NetworkAnalysisFilterService : INetworkAnalysisFilterService
    {
        public List<NetworkAnalysisItem> FilterItems(
            IEnumerable<NetworkAnalysisItem> items,
            string searchQuery,
            string selectedCategory,
            string selectedType,
            string selectedSort)
        {
            var query = items;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(item =>
                    item.User.Login != null &&
                    item.User.Login.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (Enum.TryParse<NetworkRelationshipCategory>(selectedCategory, out var categoryFilter))
            {
                query = query.Where(item => item.Category == categoryFilter);
            }

            if (!string.IsNullOrWhiteSpace(selectedType))
            {
                query = query.Where(item => string.Equals(item.User.Type, selectedType, StringComparison.OrdinalIgnoreCase));
            }

            query = selectedSort switch
            {
                "Z-A" => query.OrderByDescending(item => item.User.Login, StringComparer.OrdinalIgnoreCase),
                "Category" => query
                    .OrderBy(item => GetCategoryPriority(item.Category))
                    .ThenBy(item => item.User.Login, StringComparer.OrdinalIgnoreCase),
                _ => query.OrderBy(item => item.User.Login, StringComparer.OrdinalIgnoreCase)
            };

            return query.ToList();
        }

        private static int GetCategoryPriority(NetworkRelationshipCategory category)
        {
            return category switch
            {
                NetworkRelationshipCategory.Mutual => 0,
                NetworkRelationshipCategory.NotFollowingBack => 1,
                _ => 2
            };
        }
    }
}
