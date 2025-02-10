using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using IdentityService.Application.Features.Roles.Constants;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;

namespace IdentityService.Application.Features.Roles.Rules;

public class RoleBusinessRules : BaseBusinessRules
{
    private readonly IRoleRepository _roleRepository;

    public RoleBusinessRules(
        IRoleRepository roleRepository
    )
    {
        _roleRepository = roleRepository;
    }

    private async Task throwBusinessException(string message)
    {
        throw new BusinessException(message);
    }

    public async Task RoleShouldExistWhenSelected(Role? role)
    {
        if (role == null)
            await throwBusinessException(RolesConstants.NotExists);
    } 
    public async Task RoleShouldExistWhenSelected(int roleId)
    {
        bool doesExists = await _roleRepository.AnyAsync(predicate: u => u.Id ==roleId);
        if (!doesExists)
            await throwBusinessException(RolesConstants.NotExists);
    }
    
    public async Task RoleInUse(ICollection<UserRole> role)
    {
        if (role.Any())
            throw new BusinessException(RolesConstants.InUse);
         
    }  
    public async Task RoleNameShouldNotExistWhenCreating(string roleValue)
    {
        bool doesExist = await _roleRepository.AnyAsync(predicate: b => b.RoleValue == roleValue);
        if (doesExist)
            await throwBusinessException(RolesConstants.AlreadyExists);
    }

    public async Task RoleNameShouldNotExistWhenUpdating(int id, string roleValue)
    {
        bool doesExist = await _roleRepository.AnyAsync(predicate: b => b.Id != id && b.RoleValue == roleValue);
        if (doesExist)
            await throwBusinessException(RolesConstants.AlreadyExists);
    }
}