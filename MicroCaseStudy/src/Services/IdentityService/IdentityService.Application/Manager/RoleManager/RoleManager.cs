using System.Linq.Expressions;
using Core.Application.Enums;
using Core.Persistance.Paging;
using IdentityService.Application.Features.Roles.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace IdentityService.Application.Manager.RoleManager;

public class RoleManager: IRoleManager
{
    private readonly IRoleRepository _roleRepository;
    private readonly RoleBusinessRules _roleBusinessRules;

    public RoleManager(
        IRoleRepository roleRepository,
        RoleBusinessRules roleBusinessRules
    )
    {
        _roleRepository = roleRepository;
        _roleBusinessRules = roleBusinessRules;
    }

    public async Task<Role?> GetAsync(
        Expression<Func<Role, bool>> predicate,
        Func<IQueryable<Role>, IIncludableQueryable<Role, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        /*
        Role? role = await _roleRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return role;*/
        return null;
    }

    public async Task<Paginate<Role>?> GetListAsync(
        Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        Func<IQueryable<Role>, IIncludableQueryable<Role, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        /*
        Paginate<Role> roleList = await _roleRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return roleList;*/
        return null;
    }

    public async Task<Role> AddAsync(Role role)
    {
        /*
        await _roleBusinessRules.RoleNameShouldNotExistWhenCreating(role.RoleValue);

        Role addedRole = await _roleRepository.AddAsync(role,
            TableCreatedParameters.CreatedAtPropertyName,TableCreatedParameters.CreatedByPropertyName);

        return addedRole;*/
        return null;
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        /*
        await _roleBusinessRules.RoleNameShouldNotExistWhenUpdating(role.Id, 
            role.RoleValue);

        Role updatedRole = await _roleRepository.UpdateAsync(role,
            TableUpdatedParameters.UpdatedAtPropertyName,TableUpdatedParameters.UpdatedByPropertyName);

        return updatedRole;*/
        return null;
    }

    public async Task<Role> DeleteAsync(Role role, bool permanent = false)
    {
        /*
        Role deletedRole = await _roleRepository.DeleteAsync(role,
            TableDeletedParameters.DeletedAtPropertyName,TableDeletedParameters.DeletedByPropertyName,TableDeletedParameters.IsDeletedPropertyName);

        return deletedRole;*/
        return null;
    }
}
