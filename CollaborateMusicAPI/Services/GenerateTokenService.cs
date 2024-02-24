using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CollaborateMusicAPI.Authentication;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CollaborateMusicAPI.Services;

public class GenerateTokenService
{
    private readonly IOptions<JwtSettings> _jwtSettings;

    public GenerateTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public string GenerateJWTForUser(string userId)
    {
        var secret = _jwtSettings.Value.Key;
        // If your secret is base64 encoded in the configuration, decode it
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)) { KeyId = "Your Key Id" };
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
       
    };

        var tokenOptions = new JwtSecurityToken(
                       issuer: "https://localhost:7286",
                       audience: "https://localhost:7286",
                       claims: claims,
                       expires: DateTime.Now.AddMinutes(5),
                       signingCredentials: signinCredentials
                   );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }


    public Task<ServiceResponse<string>> CreateUserAndReturnToken(ApplicationUser newUser)
    {
        var token = GenerateJWTForUser(newUser.Id.ToString());
        return Task.FromResult(new ServiceResponse<string>
        {
            StatusCode = Enums.StatusCode.Ok,
            Content = token
        });
    }

}
