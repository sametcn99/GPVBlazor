using GPVBlazor.Models;

namespace GPVBlazor.Services.Interfaces
{
    public interface IRepositoryFilterService
    {
        List<Repository> FilterRepositories(
            IEnumerable<Repository> repositories,
            IEnumerable<Repository> _repositories,
            string searchQuery,
            bool includeArchived,
            bool includeForked,
            bool includeTemplates,
            string selectedSort,
            string selectedTopic,
            string selectedLanguage,
            string selectedLicense);
    }
}
