using System.Linq.Expressions;
using Core.Persistance.Paging;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IdentityService.Application.Manager.UserRoleManager;

public interface IUserRoleManager
{
    Task<UserRole?> GetAsync(
        Expression<Func<UserRole, bool>> predicate,
        Func<IQueryable<UserRole>, IIncludableQueryable<UserRole, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Paginate<UserRole>?> GetListAsync(
        Expression<Func<UserRole, bool>>? predicate = null,
        Func<IQueryable<UserRole>, IOrderedQueryable<UserRole>>? orderBy = null,
        Func<IQueryable<UserRole>, IIncludableQueryable<UserRole, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<UserRole> AddAsync(UserRole userRole);
    Task<UserRole> UpdateAsync(UserRole userRole);
    Task<UserRole> DeleteAsync(UserRole userRole, bool permanent = false);
}