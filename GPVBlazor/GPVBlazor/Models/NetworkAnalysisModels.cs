using System.Globalization;

namespace GPVBlazor.Models
{
    public enum NetworkRelationshipCategory
    {
        Mutual,
        NotFollowingBack,
        Fan
    }

    public class NetworkAnalysisItem
    {
        public User User { get; set; } = new();
        public NetworkRelationshipCategory Category { get; set; }
        public bool IsFollower { get; set; }
        public bool IsFollowing { get; set; }
        public string NormalizedLogin => User.Login?.ToUpper(CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public class NetworkAnalysisResult
    {
        public List<User> Followers { get; set; } = new();
        public List<User> Following { get; set; } = new();
        public List<NetworkAnalysisItem> Items { get; set; } = new();
        public int MutualCount => Items.Count(item => item.Category == NetworkRelationshipCategory.Mutual);
        public int NotFollowingBackCount => Items.Count(item => item.Category == NetworkRelationshipCategory.NotFollowingBack);
        public int FanCount => Items.Count(item => item.Category == NetworkRelationshipCategory.Fan);
    }
}
