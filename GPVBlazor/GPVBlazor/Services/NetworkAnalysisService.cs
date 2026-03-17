using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

namespace GPVBlazor.Services
{
    public class NetworkAnalysisService : INetworkAnalysisService
    {
        public NetworkAnalysisResult BuildAnalysis(IEnumerable<User> followers, IEnumerable<User> following)
        {
            var followerList = followers.Where(user => !string.IsNullOrWhiteSpace(user.Login)).ToList();
            var followingList = following.Where(user => !string.IsNullOrWhiteSpace(user.Login)).ToList();

            var followersByLogin = followerList
                .GroupBy(user => user.Login!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            var followingByLogin = followingList
                .GroupBy(user => user.Login!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            var allLogins = followersByLogin.Keys
                .Union(followingByLogin.Keys, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var items = new List<NetworkAnalysisItem>(allLogins.Count);

            foreach (var login in allLogins)
            {
                var isFollower = followersByLogin.TryGetValue(login, out var follower);
                var isFollowing = followingByLogin.TryGetValue(login, out var followingUser);

                items.Add(new NetworkAnalysisItem
                {
                    User = follower ?? followingUser ?? new User { Login = login },
                    IsFollower = isFollower,
                    IsFollowing = isFollowing,
                    Category = GetCategory(isFollower, isFollowing)
                });
            }

            return new NetworkAnalysisResult
            {
                Followers = followerList,
                Following = followingList,
                Items = items
                    .OrderBy(item => GetCategoryPriority(item.Category))
                    .ThenBy(item => item.User.Login, StringComparer.OrdinalIgnoreCase)
                    .ToList()
            };
        }

        private static NetworkRelationshipCategory GetCategory(bool isFollower, bool isFollowing)
        {
            if (isFollower && isFollowing)
                return NetworkRelationshipCategory.Mutual;

            if (!isFollower && isFollowing)
                return NetworkRelationshipCategory.NotFollowingBack;

            return NetworkRelationshipCategory.Fan;
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
