using System.Diagnostics;
using System;
using System.Net.Http.Headers;
using Azure.Core;
using CollaborateMusicAPI.Authentication;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using CollaborateMusicAPI.Contexts;

namespace CollaborateMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    private readonly IGoogleTokenService _googleTokenService;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDBContext _context;

    public AuthController(IConfiguration configuration, IGoogleTokenService googleTokenService, IUserService userService, ITokenService tokenService, ApplicationDBContext context)
    {
        _configuration = configuration;

        _googleTokenService = googleTokenService;
        _userService = userService;
        _tokenService = tokenService;
        _context = context;
    }







    [HttpPost("google-auth")]
    public async Task<IActionResult> GoogleAuth([FromBody] AuthRequest request)
    {
      try
        {
            Console.WriteLine($"Received request: {JsonConvert.SerializeObject(request)}");

            // Use the GoogleTokenService to exchange the code for a token
            var tokenResponse = await _googleTokenService.ExchangeCodeForTokenAsync(new TokenRequest { Code = request.Code });

            if (tokenResponse.StatusCode != Enums.StatusCode.Ok)
            {
                Console.WriteLine(tokenResponse.Message);
                return BadRequest(tokenResponse.Message);
            }

            var tokenObject = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse.Message);
            var accessToken = tokenObject!.Access_token;

            // Fetch user info using the token
            var googleUserResponse = await _googleTokenService.GetGoogleUserFromTokenAsync(accessToken.ToString());


            if (googleUserResponse.StatusCode != Enums.StatusCode.Ok || googleUserResponse.Content == null)
            {
                return BadRequest(new { error = "Invalid Google Token." });
            }



            var googleUser = googleUserResponse.Content;

            var existingUserResponse = await _userService.GetUserByEmailAsync(googleUser.Email);

            // If the user already exists, generate a JWT and return it with their details
            // If the user already exists, generate a JWT and return it with their details
            // If the user already exists, generate a JWT and return it with their details
            if (existingUserResponse.StatusCode == Enums.StatusCode.Ok && existingUserResponse.Content != null)
            {
                // Assuming that existingUserResponse.Content has an Id property which is the userId.
                var token = await _tokenService.CreateTokenAsync(googleUser.Email, existingUserResponse.Content.Id);
                var userId = _tokenService.GetUserIdFromToken(token!);
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found."); // This should not happen, just in case
                }

                // Now return the token along with user details
                return Ok(new
                {
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.Email,
                        FullName = user.Email // Use the actual property for the user's full name
                    }
                });
            }
           



            // If user does not exist, create a new user.
            var newUserResponse = await _googleTokenService.CreateGoogleUserAsync(googleUser);

            Console.WriteLine($"Create User Status Code: {newUserResponse.StatusCode}");
            Console.WriteLine($"Create User Message: {newUserResponse.Message}");

            

            if (newUserResponse.StatusCode == Enums.StatusCode.Created)
            {
                return CreatedAtAction(nameof(GoogleAuth), newUserResponse.Content);
            }

            return BadRequest("Could not create user!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during token verification: {ex.Message}");
            Console.WriteLine(ex.ToString());
            return BadRequest(new { error = "Error occurred during user creation." });

        }
    }




}
