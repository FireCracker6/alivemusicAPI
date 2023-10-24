using System.Diagnostics;
using System.Linq.Expressions;
using Azure;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;

namespace CollaborateMusicAPI.Services;

public interface IUserService
{
    Task<ServiceResponse<Users>> CreateAsync(ServiceRequest<UserRegistrationDto> request);
    Task<ServiceResponse<Users>> GetUserByEmailAsync(string email);
    Task<ServiceResponse<IEnumerable<Users>>> GetAllAsync();
   



    //Task<ServiceResponse<Users>> UpdateUserAsync(int id, ServiceRequest<UserUpdateDto> request);
    //Task<ServiceResponse<Users>> DeleteUserAsync(int id);
    //Task<ServiceResponse<Users>> UpdatePasswordAsync(int id, ServiceRequest<UserUpdatePasswordDto> request);
    //Task<ServiceResponse<Users>> UpdateEmailAsync(int id, ServiceRequest<UserUpdateEmailDto> request);
    //Task<ServiceResponse<Users>> UpdateTwoFactorAsync(int id, ServiceRequest<UserUpdateTwoFactorDto> request);
    //Task<ServiceResponse<Users>> UpdateOAuthAsync(int id, ServiceRequest<UserUpdateOAuthDto> request);
    //Task<ServiceResponse<Users>> UpdateProfileAsync(int id, ServiceRequest<UserUpdateProfileDto> request);
    //Task<ServiceResponse<Users>> UpdateProfileImageAsync(int id, ServiceRequest<UserUpdateProfileImageDto> request);
    //Task<ServiceResponse<Users>> UpdateProfileBackgroundImageAsync(int id, ServiceRequest<UserUpdateProfileBackgroundImageDto> request);
}

public class UserService : IUserService
{
    private readonly IUsersRepository _usersRepository;

    public UserService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Users> CreateAccount(Users user)
    {
        return await _usersRepository.CreateAsync(user);
    }

    public async Task<ServiceResponse<Users>> CreateAsync(ServiceRequest<UserRegistrationDto> request)
    {
        var response = new ServiceResponse<Users>();

        try
        {
            if (request.Content == null)
            {
                response.StatusCode = Enums.StatusCode.BadRequest;
                response.Message = "Content cannot be null.";
                return response;
            }

            if (!await _usersRepository.ExistsAsync(x => x.Email == request.Content.Email))

            {
                // Convert DTO to Entity
                Users newUser = new Users
                {
                   
                    Email = request.Content.Email,
                    PasswordHash = request.Content.Password,
                    
                };

                response.Content = await _usersRepository.CreateAsync(newUser);
                response.StatusCode = Enums.StatusCode.Created;
            }
            else
            {
                response.StatusCode = Enums.StatusCode.Conflict;
                response.Content = null;
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = Enums.StatusCode.InternalServerError;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<IEnumerable<Users>>> GetAllAsync()
    {
        var response = new ServiceResponse<IEnumerable<Users>>();

        try
        {
            response.Content = await _usersRepository.GetAllAsync();
            response.StatusCode = Enums.StatusCode.Success;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.StatusCode = Enums.StatusCode.InternalServerError;
            response.Message = ex.Message;
        }

        return response;
    }


    public async Task<ServiceResponse<Users>> GetUserByEmailAsync(string email)
    {
        var response = new ServiceResponse<Users>();

        try
        {
            if (string.IsNullOrEmpty(email))
            {
                response.StatusCode = Enums.StatusCode.BadRequest;
                response.Message = "Email cannot be null or empty.";
                return response;
            }

          
            response.Content = await _usersRepository.GetUserByEmailAsync(email);

            if (response.Content == null)
            {
                response.StatusCode = Enums.StatusCode.NotFound;
                response.Message = "User not found.";
            }
            else
            {
                response.StatusCode = Enums.StatusCode.Success;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.StatusCode = Enums.StatusCode.InternalServerError;
            response.Message = ex.Message;
        }

        return response;
    }

}
