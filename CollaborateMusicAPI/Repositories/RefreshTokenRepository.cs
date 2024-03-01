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

    public async Task<RefreshToken> GetRefreshTokenAsync(Guid userId)
    {
        return await this.GetAsync(x => x.UserId == userId && !x.IsRevoked);
    }

    public async Task<RefreshToken> UpdateRefreshTokenAsync(Guid userId, RefreshToken refreshToken)
    {
        // Update the refresh token based on the userId
        return await this.UpdateAsync(x => x.UserId == userId && x.Token == refreshToken.Token, refreshToken);
    }
}
