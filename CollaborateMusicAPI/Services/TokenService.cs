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
using CollaborateMusicAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CollaborateMusicAPI.Services;

public interface ITokenService
{
    Task<string> GetTokenAsync(string email, string password, bool isRememberMe);
    Task<string> RefreshTokenAsync(string accessToken, string refreshToken);
    Guid GetUserIdFromToken(string token); // Add this line
  
}

public class TokenService : ITokenService
{
    private readonly IUsersRepository _usersRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenValidationService _tokenValidationService;
    private readonly string _securityKey = null!;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private string keyId = "4b623c772ff94971e1b1bb0723b2a0cb";

    public TokenService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUsersRepository usersRepository, IConfiguration configuration, ILogger<AccountController> logger, ITokenValidationService tokenValidationService)
    {

        _userManager = userManager;
        _usersRepository = usersRepository;
        _signInManager = signInManager;
        _securityKey = "4b623c772ff94971e1b1bb0723b2a0cb";
        _configuration = configuration;
        _logger = logger;
        _tokenValidationService = tokenValidationService;
    }

    public async Task<string> GetTokenAsync(string email,  string password, bool isRememberMe)
    {
      


        var response = new ServiceResponse<UserWithTokenResponse>();
        try
        {
            var user = await _usersRepository.GetUserByEmailAsync(email);
         
            var userName = await _signInManager.UserManager.FindByEmailAsync(user.Email);   
          

            if (userName != null)
            {
                // Depending on your setup, you might need to use user.UserName instead of user.Email
                var result = await _signInManager.PasswordSignInAsync(userName, password, isRememberMe, false);

                if (result.Succeeded)
                {
                    var token = GenerateAuthToken(user!.Email!, user.Id); // Generate the token

                    // Immediately validate the token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = _tokenValidationService.GetTokenValidationParameters(); // Your existing validation parameters

                    try
                    {
                        // This will throw an exception if the token is invalid
                        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                    }
                    catch (SecurityTokenException ex)
                    {
                        // Handle the case where the token is invalid
                        Console.WriteLine($"Token validation error: {ex.Message}");
                        // Depending on your error handling, either throw, log the error, or return a failure response
                        throw;
                    }

                    // If the token is valid, then you can proceed to issue it to the client
                    var refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    await _userManager.UpdateAsync(user);

                    return token;
                }
                else
{
    var errorMessage = result.IsLockedOut ? "User account is locked out."
                    : result.IsNotAllowed ? "User is not allowed to login."
                    : result.RequiresTwoFactor ? "Login requires two-factor authentication."
                    : "Invalid login attempt.";
    Debug.WriteLine(errorMessage);
    response.Message = errorMessage;
}

            }
            else
            {
                response.Message = "User not found";
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = Enums.StatusCode.InternalServerError;
            response.Message = ex.Message;
            // Log the exception details here
        }
        return null; // Consider returning an appropriate error message or throw an exception
    }


    public async Task<string> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
      

       

        var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

        if (emailClaim == null || userIdClaim == null)
        {
            throw new Exception("Invalid token: email or user ID claim missing.");
        }

        var email = emailClaim.Value;
        var userIdString = userIdClaim.Value;
        Guid userId;

        if (!Guid.TryParse(userIdString, out userId))
        {
            throw new Exception("Invalid token: user ID claim is not a valid GUID.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null || user.RefreshToken != refreshToken)
        {
            throw new Exception("Invalid refresh token.");
        }

        var newToken = GenerateAuthToken(email, userId); // Pass userId here
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        return newToken;
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

    private string GenerateAuthToken(string email, Guid userId)
    {
         // Generates a new KeyId
        _logger.LogInformation($"Key ID generated: {keyId}"); // Logs the KeyId

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_securityKey); // This should be your secret key

        if (key.Length < 16)
        {
            throw new InvalidOperationException("The key is too short.");
        }

        // Here you assign the KeyId to the securityKey
        var securityKey = new SymmetricSecurityKey(key) { KeyId = "4b623c772ff94971e1b1bb0723b2a0cb" };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

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


    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}



