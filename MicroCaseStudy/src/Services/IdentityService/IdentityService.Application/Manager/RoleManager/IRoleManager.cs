using System.Linq.Expressions;
using Core.Persistance.Paging;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IdentityService.Application.Manager.RoleManager;

public interface IRoleManager
{
    Task<Role?> GetAsync(
        Expression<Func<Role, bool>> predicate,
        Func<IQueryable<Role>, IIncludableQueryable<Role, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Paginate<Role>?> GetListAsync(
        Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        Func<IQueryable<Role>, IIncludableQueryable<Role, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Role> AddAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task<Role> DeleteAsync(Role role, bool permanent = false);
}