using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace GPVBlazor.Controllers;

/// <summary>
/// API endpoints for GitHub authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AutoValidateAntiforgeryToken]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Get the GitHub OAuth login URL
    /// </summary>
    /// <returns>GitHub OAuth authorization URL</returns>
    [HttpGet("login-url")]
    [ProducesResponseType(typeof(LoginUrlResponse), StatusCodes.Status200OK)]
    public ActionResult<LoginUrlResponse> GetLoginUrl()
    {
        var url = _authService.GetGitHubLoginUrl(HttpContext);
        return Ok(new LoginUrlResponse { Url = url });
    }

    /// <summary>
    /// Start the GitHub OAuth flow with a validated state cookie
    /// </summary>
    [HttpGet("login")]
    public IActionResult Login()
    {
        var url = _authService.GetGitHubLoginUrl(HttpContext);
        return Redirect(url);
    }

    /// <summary>
    /// Validate the current authenticated session
    /// </summary>
    /// <returns>Whether the current auth session is valid</returns>
    [HttpGet("validate-token")]
    [ProducesResponseType(typeof(TokenValidationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TokenValidationResponse>> ValidateToken(CancellationToken cancellationToken)
    {
        var token = await _authService.GetActiveAccessTokenAsync(User, cancellationToken);
        var isValid = !string.IsNullOrWhiteSpace(token);
        return Ok(new TokenValidationResponse { IsValid = isValid });
    }

    /// <summary>
    /// Get current user information using the active auth session
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("current-user")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<User>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUserAsync(User, cancellationToken);
        if (user == null)
            return Unauthorized("Not authenticated");

        return Ok(user);
    }

    /// <summary>
    /// Get GitHub API rate limit information
    /// </summary>
    /// <returns>Rate limit information</returns>
    [HttpGet("rate-limit")]
    [ProducesResponseType(typeof(RateLimitInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RateLimitInfo>> GetRateLimit(CancellationToken cancellationToken)
    {
        var rateLimit = await _authService.GetRateLimitAsync(principal: User, cancellationToken: cancellationToken);
        if (rateLimit == null)
            return StatusCode(500, "Failed to fetch rate limit information");

        return Ok(rateLimit);
    }

    /// <summary>
    /// GitHub OAuth callback endpoint
    /// </summary>
    /// <param name="code">OAuth code</param>
    /// <param name="state">OAuth state</param>
    /// <returns>Redirect to home page after sign-in</returns>
    [HttpGet("github-callback")]
    [HttpGet("/github-callback")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GitHubCallback([FromQuery] string code, [FromQuery] string? state, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest("Code is required");

        var signedIn = await _authService.SignInWithGitHubCodeAsync(HttpContext, code, state, cancellationToken);
        if (!signedIn)
            return BadRequest("Failed to validate the GitHub login flow or exchange code for token");

        return Redirect("/");
    }

    /// <summary>
    /// Sign in with a personal access token
    /// </summary>
    /// <param name="request">Personal access token request</param>
    /// <returns>Authentication status</returns>
    [HttpPost("personal-access-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignInWithPersonalAccessToken([FromBody] PersonalAccessTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            return BadRequest("Token is required");

        var signedIn = await _authService.SignInWithPersonalAccessTokenAsync(HttpContext, request.Token, request.AuthSource, cancellationToken);
        if (!signedIn)
            return Unauthorized("Invalid token");

        return Ok(new TokenValidationResponse { IsValid = true });
    }

    /// <summary>
    /// Sign out and clear the current auth session
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _authService.SignOutAsync(HttpContext, User, cancellationToken);
        return Ok();
    }
}

/// <summary>
/// Response model for login URL
/// </summary>
public class LoginUrlResponse
{
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Response model for token validation
/// </summary>
public class TokenValidationResponse
{
    public bool IsValid { get; set; }
}

/// <summary>
/// Request model for personal access token sign-in
/// </summary>
public class PersonalAccessTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string AuthSource { get; set; } = "token";
}
