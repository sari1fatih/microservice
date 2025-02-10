using Core.Persistance.Repository;
using IdentityService.Domain.Entities;

namespace IdentityService.Persistance.Abstract.Repositories;

public interface IUserRoleRepository:  IAsyncRepository<UserRole>, IRepository<UserRole>
{   
    Task<IList<Role>> GetRoleClaimsByUserIdAsync(int userId);
    
}