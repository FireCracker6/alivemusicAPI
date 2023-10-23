using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;

namespace CollaborateMusicAPI.Services;

public class UserService
{
    private readonly UsersRepository _usersRepository;

    public UserService(UsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Users> CreateAccount(Users user)
    {
        return await _usersRepository.CreateAsync(user);
    }
    public Users GetUserByEmail(string email)
    {
        return _usersRepository.GetUserByEmail(email);
    }
}
