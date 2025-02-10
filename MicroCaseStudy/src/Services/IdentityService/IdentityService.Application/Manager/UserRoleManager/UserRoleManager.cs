using System.Linq.Expressions;
using Core.Application.Enums;
using Core.Persistance.Paging;
using IdentityService.Application.Features.UserRoles.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace IdentityService.Application.Manager.UserRoleManager;

public class UserRoleManager : IUserRoleManager
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly UserRoleBusinessRules _userRoleBusinessRules;

    public UserRoleManager(
        IUserRoleRepository userRoleRepository,
        UserRoleBusinessRules userRoleBusinessRules
    )
    {
        _userRoleRepository = userRoleRepository;
        _userRoleBusinessRules = userRoleBusinessRules;
    }

    public async Task<UserRole?> GetAsync(
        Expression<Func<UserRole, bool>> predicate,
        Func<IQueryable<UserRole>, IIncludableQueryable<UserRole, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        UserRole? userRole = await _userRoleRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userRole;
    }

    public async Task<Paginate<UserRole>?> GetListAsync(
        Expression<Func<UserRole, bool>>? predicate = null,
        Func<IQueryable<UserRole>, IOrderedQueryable<UserRole>>? orderBy = null,
        Func<IQueryable<UserRole>, IIncludableQueryable<UserRole, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Paginate<UserRole> userRoleList = await _userRoleRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userRoleList;
    }

    public async Task<UserRole> AddAsync(UserRole userRole)
    {
        await _userRoleBusinessRules.UserShouldNotHasRoleAlreadyWhenInsert(
            userRole.UserId,
            userRole.RoleId
        );

        UserRole addedUserRole = await _userRoleRepository.AddAsync
        (userRole, TableCreatedParameters.CreatedAtPropertyName,
            TableCreatedParameters.CreatedByPropertyName);

        return addedUserRole;
    }

    public async Task<UserRole> UpdateAsync(UserRole userRole)
    {
        await _userRoleBusinessRules.UserShouldNotHasRoleAlreadyWhenUpdated(
            userRole.Id,
            userRole.UserId,
            userRole.RoleId
        );

        UserRole updatedUserRole = await _userRoleRepository.UpdateAsync(
            userRole,
            TableUpdatedParameters.UpdatedAtPropertyName,
            TableUpdatedParameters.UpdatedByPropertyName
        );

        return updatedUserRole;
    }

    public async Task<UserRole> DeleteAsync(UserRole userRole, bool permanent = false)
    {
        UserRole deletedUserRole = await _userRoleRepository.DeleteAsync(
            userRole,
            TableDeletedParameters.DeletedAtPropertyName,
            TableDeletedParameters.DeletedByPropertyName, TableDeletedParameters.IsDeletedPropertyName
        );

        return deletedUserRole;
    }
}