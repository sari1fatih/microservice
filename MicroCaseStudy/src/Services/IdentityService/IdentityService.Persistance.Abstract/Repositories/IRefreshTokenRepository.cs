using Core.Persistance.Repository;
using IdentityService.Domain.Entities;

namespace IdentityService.Persistance.Abstract.Repositories;

public interface IRefreshTokenRepository: IAsyncRepository<RefreshToken>, IRepository<RefreshToken>
{
    Task<List<RefreshToken>> GetOldRefreshTokensAsync(int userId);
}