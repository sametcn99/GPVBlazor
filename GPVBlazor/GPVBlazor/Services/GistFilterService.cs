using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

namespace GPVBlazor.Services
{
    public class GistFilterService : IGistFilterService
    {
        public List<Gist> FilterGists(
            IEnumerable<Gist> gists,
            IEnumerable<Gist> _gists,
            string searchQuery,
            string selectedSort,
            string selectedLanguage)
        {
            if (gists == null) return new List<Gist>();

            var query = _gists.Where(g =>
                (g.Description != null && g.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                (g.Files != null && g.Files.Any(f => f.Key.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))));

            if (!string.IsNullOrWhiteSpace(selectedLanguage))
            {
                query = query.Where(g => g.Files != null && g.Files.Any(f => f.Value.Language == selectedLanguage));
            }

            switch (selectedSort)
            {
                case "Updated Descending":
                    query = query.OrderByDescending(g => g.UpdatedAt);
                    break;
                case "Updated Ascending":
                    query = query.OrderBy(g => g.UpdatedAt);
                    break;
                case "Created Descending":
                    query = query.OrderByDescending(g => g.CreatedAt);
                    break;
                case "Created Ascending":
                    query = query.OrderBy(g => g.CreatedAt);
                    break;
                case "Stars Descending":
                    query = query.OrderByDescending(g => g.StargazersCount);
                    break;
                case "Stars Ascending":
                    query = query.OrderBy(g => g.StargazersCount);
                    break;
            }

            return query.ToList();
        }
    }
}
