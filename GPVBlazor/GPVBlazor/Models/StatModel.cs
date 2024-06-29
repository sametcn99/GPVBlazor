namespace GPVBlazor.Models
{
    public class RepositoryStats
    {
        private readonly List<Repository> _repositories;

        public RepositoryStats(List<Repository> repositories) => _repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));

        public Repository? OldestRepository => _repositories.MinBy(repo => repo.CreatedAt);

        public Repository? LongestUpdatePeriod => _repositories.MaxBy(repo => repo.UpdatedAt - repo.CreatedAt);

        public Repository? MostStarred => _repositories.MaxBy(repo => repo.StargazersCount);

        public Repository? MostForked => _repositories.MaxBy(repo => repo.ForksCount);

        public Repository? LatestUpdatedRepository => _repositories.MaxBy(repo => repo.PushedAt);

        public Repository? LastCreatedRepository => _repositories.MaxBy(repo => repo.CreatedAt);

        public int TotalTopics => _repositories.Sum(repo => repo.Topics?.Count ?? 0);

        public int TotalLanguages => UsedLanguages.Count;

        public int TotalStars => _repositories.Sum(repo => repo.StargazersCount);

        public int TotalForks => _repositories.Sum(repo => repo.ForksCount);

        public int TotalWatchers => _repositories.Sum(repo => repo.WatchersCount);

        public int TotalOpenIssues => _repositories.Sum(repo => repo.OpenIssuesCount);

        public double AverageStarsPerRepository => Math.Round(_repositories.Average(repo => repo.StargazersCount), 2);

        public double AverageForksPerRepository => Math.Round(_repositories.Average(repo => repo.ForksCount), 2);

        public List<string> TopTopics => _repositories
            .SelectMany(repo => repo.Topics ?? new List<string>())
            .GroupBy(topic => topic)
            .OrderByDescending(group => group.Count())
            .Take(10)
            .Select(group => group.Key)
            .ToList();

        public Dictionary<int, int> RepositoriesByYear => _repositories
            .Where(repo => repo.CreatedAt.HasValue)
            .GroupBy(repo => repo.CreatedAt!.Value.Year)
            .ToDictionary(group => group.Key, group => group.Count());

        public Dictionary<int, int> RepositoriesUpdatedByYear => _repositories
            .Where(repo => repo.UpdatedAt.HasValue)
            .GroupBy(repo => repo.UpdatedAt!.Value.Year)
            .ToDictionary(group => group.Key, group => group.Count());

        public Dictionary<string, int> UsedLanguages => _repositories
            .Where(repo => repo.Language != null)
            .GroupBy(repo => repo.Language!)
            .ToDictionary(group => group.Key!, group => group.Count());

        public Dictionary<string, int> UsedLicenses => _repositories
            .Where(repo => repo.License != null)
            .GroupBy(repo => repo.License!.Name)
            .ToDictionary(group => group.Key!, group => group.Count());

        public Dictionary<string, int> StarsPerRepository => _repositories
            .ToDictionary(repo => repo.Name!, repo => repo.StargazersCount);
    }
}