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
    private readonly IUserSession<int> _userSession;
    public RefreshTokenRepository(IdentityServiceDbContext context, IUserSession<int> userSession) : base(context,
        userSession)
    {
        _userSession = userSession;
    }
 
    public async Task DeleteOldRefreshTokensAsync(bool all,int userId)
    {
        await Query()
            .AsNoTracking()
            .Where(r =>
                r.UserId == userId &&
                r.IsActive == true &&
                r.IsDeleted == false &&
                (
                    all || 
                    (
                        !all &&
                        r.ExpiresDate < DateTime.UtcNow
                    )
                )
            )
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.IsActive, false)
                .SetProperty(u => u.IsDeleted, true)
                .SetProperty(u => u.UpdatedAt, DateTime.UtcNow)
                .SetProperty(u => u.UpdatedBy, _userSession.UserId)
            );
 
    }
}