using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphUserManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a JWT token for testing purposes
    /// </summary>
    /// <param name="request">Login request</param>
    /// <returns>JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public ActionResult<TokenResponse> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Simple validation for demo purposes
            // In production, validate against your identity provider
            if (request.Username == "admin" && request.Password == "password")
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
                var issuer = jwtSettings["Issuer"] ?? "GraphUserManagementAPI";
                var audience = jwtSettings["Audience"] ?? "GraphUserManagementAPI";
                var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");

                var token = GenerateJwtToken(request.Username, secretKey, issuer, audience, expiryInMinutes);

                _logger.LogInformation("JWT token generated for user: {Username}", request.Username);

                return Ok(new TokenResponse
                {
                    Token = token,
                    TokenType = "Bearer",
                    ExpiresIn = expiryInMinutes * 60, // Convert to seconds
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expiryInMinutes)
                });
            }

            _logger.LogWarning("Invalid login attempt for username: {Username}", request.Username);
            return Unauthorized(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "Invalid username or password",
                Status = StatusCodes.Status401Unauthorized
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred during login",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    private string GenerateJwtToken(string username, string secretKey, string issuer, string audience, int expiryInMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim("scope", "users:read users:write"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record TokenResponse
{
    public string Token { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
    public DateTime ExpiresAt { get; init; }
}