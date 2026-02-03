using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace GPVBlazor.Controllers;

/// <summary>
/// API endpoints for GitHub authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
        var url = _authService.GetGitHubLoginUrl();
        return Ok(new LoginUrlResponse { Url = url });
    }

    /// <summary>
    /// Validate a GitHub access token
    /// </summary>
    /// <param name="token">GitHub access token to validate</param>
    /// <returns>Whether the token is valid</returns>
    [HttpGet("validate-token")]
    [ProducesResponseType(typeof(TokenValidationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TokenValidationResponse>> ValidateToken([FromHeader(Name = "X-GitHub-Token")] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Ok(new TokenValidationResponse { IsValid = false });

        var isValid = await _authService.IsTokenValidAsync(token);
        return Ok(new TokenValidationResponse { IsValid = isValid });
    }

    /// <summary>
    /// Get current user information using access token
    /// </summary>
    /// <param name="token">GitHub access token</param>
    /// <returns>Current user information</returns>
    [HttpGet("current-user")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<User>> GetCurrentUser([FromHeader(Name = "X-GitHub-Token")] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized("Token is required");

        var user = await _authService.FetchCurrentUserAsync(token);
        if (user == null)
            return Unauthorized("Invalid token");

        return Ok(user);
    }

    /// <summary>
    /// Get GitHub API rate limit information
    /// </summary>
    /// <param name="token">Optional GitHub access token for authenticated rate limits</param>
    /// <returns>Rate limit information</returns>
    [HttpGet("rate-limit")]
    [ProducesResponseType(typeof(RateLimitInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RateLimitInfo>> GetRateLimit([FromHeader(Name = "X-GitHub-Token")] string? token = null)
    {
        var rateLimit = await _authService.GetRateLimitAsync(token);
        if (rateLimit == null)
            return StatusCode(500, "Failed to fetch rate limit information");

        return Ok(rateLimit);
    }

    /// <summary>
    /// Exchange OAuth code for access token
    /// </summary>
    /// <param name="request">OAuth code exchange request</param>
    /// <returns>Access token response</returns>
    [HttpPost("token")]
    [ProducesResponseType(typeof(AuthTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthTokenResponse>> ExchangeCodeForToken([FromBody] CodeExchangeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return BadRequest("Code is required");

        var tokenResponse = await _authService.GetAccessTokenFromCodeAsync(request.Code);
        if (tokenResponse == null)
            return BadRequest("Failed to exchange code for token");

        return Ok(tokenResponse);
    }

    /// <summary>
    /// Refresh an access token using a refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New access token response</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("Refresh token is required");

        var tokenResponse = await _authService.RefreshAccessTokenAsync(request.RefreshToken);
        if (tokenResponse == null)
            return BadRequest("Failed to refresh token");

        return Ok(tokenResponse);
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
/// Request model for OAuth code exchange
/// </summary>
public class CodeExchangeRequest
{
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// Request model for token refresh
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
