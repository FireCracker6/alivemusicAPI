using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;

namespace CollaborateMusicAPI.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken, ApplicationDBContext>
{
    public RefreshTokenRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        // Assuming your base repository has an AddAsync method as shown earlier.
        return await this.CreateAsync(refreshToken);
    }
}
