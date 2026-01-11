using GPVBlazor.Models;

namespace GPVBlazor.Services.Interfaces
{
    public interface IGistFilterService
    {
        List<Gist> FilterGists(
            IEnumerable<Gist> gists,
            IEnumerable<Gist> _gists,
            string searchQuery,
            string selectedSort,
            string selectedLanguage);
    }
}
