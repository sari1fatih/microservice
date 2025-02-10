using Core.Persistance.Repository;
using Core.WebAPI.Appsettings;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using IdentityService.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Persistance.Repositories;

public class UserRoleRepository: EfRepositoryBase<UserRole, IdentityServiceDbContext,int>, 
    IUserRoleRepository
{
    public UserRoleRepository(IdentityServiceDbContext context,IUserSession<int> userSession) : base(context,userSession)
    {
    }
    public async Task<IList<Role>> GetRoleClaimsByUserIdAsync(int userId)
    {
        List<Role> roles = await Query()
            .AsNoTracking()
            .Where(p => p.UserId.Equals(userId))
            .Select(p => new Role { Id = p.RoleId, RoleValue = p.Role.RoleValue })
            .ToListAsync();
        return roles;
    }
}