using Core.Persistance.Repository;
using Core.WebAPI.Appsettings;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using IdentityService.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Persistance.Repositories;

public class RefreshTokenRepository : EfRepositoryBase<RefreshToken, IdentityServiceDbContext, int>,
    IRefreshTokenRepository
{
    public RefreshTokenRepository(IdentityServiceDbContext context, IUserSession<int> userSession) : base(context,
        userSession)
    {
    }

    public async Task<List<RefreshToken>> GetOldRefreshTokensAsync(int userId)
    {
        List<RefreshToken> tokens = await Query()
            .AsNoTracking()
            .Where(r =>
                r.UserId == userId &&
                r.IsActive == true &&
                r.IsDeleted == false
            )
            .ToListAsync();

        return tokens;
    }
}