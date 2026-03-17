using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace GPVBlazor.Controllers;

/// <summary>
/// API endpoints for GitHub user operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UsersController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    /// <summary>
    /// Search for GitHub users
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>List of matching users</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(UserSearchResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserSearchResult>> SearchUsers([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query parameter is required");

        var result = await _userService.SearchUsers(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a GitHub user's profile
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <returns>User profile information</returns>
    [HttpGet("{username}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserProfile(string username, CancellationToken cancellationToken)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var user = await _userService.FetchUserProfile(username, token ?? string.Empty);
        if (user == null)
            return NotFound($"User '{username}' not found");

        return Ok(user);
    }

    /// <summary>
    /// Get a GitHub user's repositories
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <param name="count">Number of repositories per page</param>
    /// <param name="page">Page number</param>
    /// <returns>List of repositories</returns>
    [HttpGet("{username}/repositories")]
    [ProducesResponseType(typeof(List<Repository>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Repository>>> GetUserRepositories(
        string username,
        [FromQuery] int count = 30,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var repositories = await _userService.FetchUserRepositories(username, token ?? string.Empty, count, page);
        return Ok(repositories);
    }

    /// <summary>
    /// Get a GitHub user's gists
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <param name="count">Number of gists per page</param>
    /// <param name="page">Page number</param>
    /// <returns>List of gists</returns>
    [HttpGet("{username}/gists")]
    [ProducesResponseType(typeof(List<Gist>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Gist>>> GetUserGists(
        string username,
        [FromQuery] int count = 30,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var gists = await _userService.FetchUserGists(username, token ?? string.Empty, count, page);
        return Ok(gists);
    }

    /// <summary>
    /// Get a GitHub user's contribution history
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <returns>Contribution data</returns>
    [HttpGet("{username}/contributions")]
    [ProducesResponseType(typeof(ContributionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContributionResponse>> GetUserContributions(string username)
    {
        var contributions = await _userService.FetchUserContributions(username);
        if (contributions == null)
            return NotFound($"Contributions for user '{username}' not found");

        return Ok(contributions);
    }

    /// <summary>
    /// Get a GitHub user's organizations
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <returns>List of organizations</returns>
    [HttpGet("{username}/organizations")]
    [ProducesResponseType(typeof(List<Organization>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Organization>>> GetUserOrganizations(
        string username,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var organizations = await _userService.FetchUserOrganizations(username, token ?? string.Empty);
        return Ok(organizations);
    }

    /// <summary>
    /// Get a GitHub user's social accounts
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <returns>List of social accounts</returns>
    [HttpGet("{username}/social-accounts")]
    [ProducesResponseType(typeof(List<SocialAccount>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SocialAccount>>> GetUserSocialAccounts(
        string username,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var socialAccounts = await _userService.FetchUserSocialAccounts(username, token ?? string.Empty);
        return Ok(socialAccounts);
    }

    /// <summary>
    /// Get a GitHub user's followers
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Number of items per page</param>
    /// <returns>List of followers</returns>
    [HttpGet("{username}/followers")]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<User>>> GetUserFollowers(
        string username,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 100,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var followers = await _userService.FetchUserFollowers(username, token ?? string.Empty, page, perPage);
        return Ok(followers);
    }

    /// <summary>
    /// Get users followed by a GitHub user
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Number of items per page</param>
    /// <returns>List of followed users</returns>
    [HttpGet("{username}/following")]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<User>>> GetUserFollowing(
        string username,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 100,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var following = await _userService.FetchUserFollowing(username, token ?? string.Empty, page, perPage);
        return Ok(following);
    }

    /// <summary>
    /// Get a GitHub user's recent activities
    /// </summary>
    /// <param name="username">GitHub username</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Number of activities per page</param>
    /// <returns>List of activities and pagination info</returns>
    [HttpGet("{username}/activities")]
    [ProducesResponseType(typeof(UserActivitiesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserActivitiesResponse>> GetUserActivities(
        string username,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 30,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var (activities, hasNextPage) = await _userService.FetchUserActivities(username, token ?? string.Empty, page, perPage);
        return Ok(new UserActivitiesResponse { Activities = activities, HasNextPage = hasNextPage });
    }

    /// <summary>
    /// Get star history for a repository
    /// </summary>
    /// <param name="owner">Repository owner</param>
    /// <param name="repo">Repository name</param>
    /// <returns>Star history data</returns>
    [HttpGet("{owner}/{repo}/star-history")]
    [ProducesResponseType(typeof(StarHistory), StatusCodes.Status200OK)]
    public async Task<ActionResult<StarHistory>> GetStarHistory(
        string owner,
        string repo,
        CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var starHistory = await _userService.FetchStarHistory(owner, repo, token ?? string.Empty);
        return Ok(starHistory);
    }
}

/// <summary>
/// Response model for user activities
/// </summary>
public class UserActivitiesResponse
{
    public List<Activity> Activities { get; set; } = new();
    public bool HasNextPage { get; set; }
}
