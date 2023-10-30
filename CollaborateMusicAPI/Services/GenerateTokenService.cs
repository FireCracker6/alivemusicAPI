﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CollaborateMusicAPI.Authentication;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
        var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));


        var signinCredentials = new SigningCredentials(secretKey, "HS256");


        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
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

    public Task<ServiceResponse<string>> CreateUserAndReturnToken(Users newUser)
    {
        var token = GenerateJWTForUser(newUser.Id.ToString());
        return Task.FromResult(new ServiceResponse<string>
        {
            StatusCode = Enums.StatusCode.Ok,
            Content = token
        });
    }

}