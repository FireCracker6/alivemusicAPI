using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure;
using CollaborateMusicAPI.Authentication;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Controllers;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CollaborateMusicAPI.Services;

public interface ITokenService
{
    Task<UserWithTokenResponse> GetTokenAsync(string email, Guid userId, bool rememberMe);
    Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    Guid GetUserIdFromToken(string token);
    Task<string> CreateTokenAsync(string email, Guid userId);
    Task<string> GenerateTokenAsync(string email, string userId, bool rememberMe);

}

public class TokenService : ITokenService
{
    private readonly IUsersRepository _usersRepository;
    private readonly RefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenValidationService _tokenValidationService;
    private readonly string _securityKey = null!;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private string keyId = "4b623c772ff94971e1b1bb0723b2a0cb";

    public TokenService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUsersRepository usersRepository, IConfiguration configuration, ILogger<AccountController> logger, ITokenValidationService tokenValidationService, RefreshTokenRepository refreshTokenRepository)
    {

        _userManager = userManager;
        _usersRepository = usersRepository;
        _signInManager = signInManager;
        _securityKey = "4b623c772ff94971e1b1bb0723b2a0cb";
        _configuration = configuration;
        _logger = logger;
        _tokenValidationService = tokenValidationService;
        _refreshTokenRepository = refreshTokenRepository;
    }
    public async Task<UserWithTokenResponse> GetTokenAsync(string email, Guid userId, bool rememberMe)
    {
        var user = await _usersRepository.GetUserByEmailAsync(email);
        var userName = await _signInManager.UserManager.FindByEmailAsync(user!.Email!);
        var token = await GenerateAuthToken(user!.Email!, user.Id.ToString()); // Generate the JWT token

        // Check for an existing valid refresh token
        var existingRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(userId);
        if (existingRefreshToken != null && !existingRefreshToken.IsRevoked && existingRefreshToken.Expires > DateTime.UtcNow)
        {
            // Existing token is still valid, use it
            return new UserWithTokenResponse
            {
                Token = token,
                RefreshToken = existingRefreshToken.Token,
                User = user
            };
        }

        // No valid existing token, generate a new one
        var newRefreshToken = await GenerateRefreshToken(user.Email, rememberMe);
        var refreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            RememberMe = rememberMe,
            Expires = DateTime.UtcNow.AddDays(rememberMe ? 30 : 7),
            Created = DateTime.UtcNow

        };

        // Add the new refresh token entity to the database
        await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);

        return new UserWithTokenResponse
        {
            Token = token,
            RefreshToken = newRefreshToken,
            User = user
        };
    }



    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

        if (user == null)
        {
            throw new Exception("User does not exist.");
        }

        var refreshTokenEntity = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
        if (refreshTokenEntity == null || refreshTokenEntity.IsRevoked)
        {
            throw new Exception("Invalid or revoked refresh token.");
        }

        var rememberMe = refreshTokenEntity.RememberMe;

        // Generate new access token
        var newAccessToken = await CreateTokenAsync(user.Email, user.Id);

        bool shouldRegenerateRefreshToken = refreshTokenEntity.Expires < DateTime.UtcNow.AddDays(7); // Example condition

        string newRefreshTokenString = refreshToken;
        if (shouldRegenerateRefreshToken)
        {
            // Generate a new refresh token string
            newRefreshTokenString = await GenerateRefreshToken(user.Email, rememberMe);
            refreshTokenEntity.Token = newRefreshTokenString;
            refreshTokenEntity.Expires = DateTime.UtcNow.AddDays(rememberMe ? 30 : 7);
            await _refreshTokenRepository.UpdateRefreshTokenAsync(user.Id, refreshTokenEntity); // Update in database
        }

        return (newAccessToken, newRefreshTokenString);
    }


    public async Task<string> CreateTokenAsync(string email, Guid userId)
    {
        // Use await to call the async method
        return await GenerateAuthToken(email, userId.ToString());
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = _tokenValidationService.GetTokenValidationParameters();
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
    private string GenerateKeyIdentifier()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[16]; // For a 128-bit key identifier
            rng.GetBytes(randomBytes);
            var keyId = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();

            // Log the value with _logger
            _logger.LogInformation("Generated Key Identifier: {KeyId}", keyId);

            return keyId;
        }
    }
    private async Task<string> GenerateAuthToken(string email, string userId)
    {
        // Assuming _userManager is available in this context
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.NameIdentifier, userId)
    };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = Encoding.UTF8.GetBytes(_securityKey);
        var securityKey = new SymmetricSecurityKey(key) { KeyId = "4b623c772ff94971e1b1bb0723b2a0cb" };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(20),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }





    public Guid GetUserIdFromToken(string token)
    {
        // DEBUG ONLY: Show PII (Personally Identifiable Information)
        IdentityModelEventSource.ShowPII = true;
        var tokenValidationParameters = _tokenValidationService.GetTokenValidationParameters();
        // Override the ValidateLifetime to true, so it checks if the token is not expired
        tokenValidationParameters.ValidateLifetime = true;

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"Claim type: {claim.Type}, Claim value: {claim.Value}");
            }
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            else
            {
                throw new SecurityTokenException("Invalid token: UserID claim missing or invalid.");
            }
        }
        catch (SecurityTokenValidationException stEx)
        {
           

            // Log the detailed exception and rethrow or handle accordingly
            Console.WriteLine($"Security token validation error: {stEx.Message}");
            throw;
        }
        catch (ArgumentException argEx)
        {
            // This will catch issues like an invalid GUID
            Console.WriteLine($"Argument exception, check if the GUID is correct: {argEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General exception occurred: {ex}");
            throw;
        }

    }


    public async Task<string> GenerateRefreshToken(string email, bool rememberme)
    {
        var user = await _usersRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            UserId = user.Id,
            Expires = rememberme ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(7), // For example, 30 days if rememberMe is true, otherwise 7 days
            Created = DateTime.UtcNow
        };

      //  await _usersRepository.SaveRefreshToken(refreshToken);

        return refreshToken.Token;
    }

    public async Task<string> GenerateTokenAsync(string email, string userId, bool rememberMe)
    {
        var newToken = GenerateAuthToken(email, userId); // Synchronous operation

        // If a refresh token is needed
        var newRefreshToken = await GenerateRefreshToken(email, rememberMe);


        return await newToken;
    }



}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}



