using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

namespace GPVBlazor.Services
{
    public class RepositoryFilterService : IRepositoryFilterService
    {
        public List<Repository> FilterRepositories(
       IEnumerable<Repository> repositories,
       IEnumerable<Repository> _repositories,
       string searchQuery,
       bool includeArchived,
       bool includeForked,
       bool includeTemplates,
       string selectedSort,
       string selectedTopic,
       string selectedLanguage,
       string selectedLicense)
        {
            if (repositories == null) return new List<Repository>();


            var query = _repositories.Where(r =>
                (r.Name != null && r.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                (r.Description != null && r.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));

            if (!string.IsNullOrWhiteSpace(selectedTopic))
                query = query.Where(r => r.Topics != null && r.Topics.Contains(selectedTopic));
            if (!string.IsNullOrWhiteSpace(selectedLanguage))
                query = query.Where(r => r.Language == selectedLanguage);
            if (!string.IsNullOrWhiteSpace(selectedLicense))
                query = query.Where(r => r.License != null && r.License.Name == selectedLicense);
            if (!includeArchived)
                query = query.Where(r => !r.Archived);
            if (!includeForked)
                query = query.Where(r => !r.Fork);
            if (!includeTemplates)
                query = query.Where(r => !r.IsTemplate);

            switch (selectedSort)
            {
                case "Updated Descending":
                    query = query.OrderByDescending(r => r.UpdatedAt);
                    break;
                case "Updated Ascending":
                    query = query.OrderBy(r => r.UpdatedAt);
                    break;
                case "Created Descending":
                    query = query.OrderByDescending(r => r.CreatedAt);
                    break;
                case "Created Ascending":
                    query = query.OrderBy(r => r.CreatedAt);
                    break;
                case "Stars Descending":
                    query = query.OrderByDescending(r => r.StargazersCount);
                    break;
                case "Stars Ascending":
                    query = query.OrderBy(r => r.StargazersCount);
                    break;
            }

            return query.ToList();
        }
    }
}
