using Core.Persistance.Repository;
using Core.Redis.Dtos;
using IdentityService.Domain.Entities;

namespace IdentityService.Persistance.Abstract.Repositories;

public interface IRefreshTokenRepository: IAsyncRepository<RefreshToken>, IRepository<RefreshToken>
{
    Task DeleteOldRefreshTokensAsync(bool all, int userId);
    Task<List<JwtRedisDto>> GetListForRedis();
}