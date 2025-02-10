using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using IdentityService.Application.Features.UserRoles.Constants;
using IdentityService.Persistance.Abstract.Repositories;

namespace IdentityService.Application.Features.UserRoles.Rules;

public class UserRoleBusinessRules: BaseBusinessRules
{
    private readonly IUserRoleRepository _userRoleRepository; 

    public UserRoleBusinessRules(
        IUserRoleRepository userRoleRepository
    )
    {
        _userRoleRepository = userRoleRepository;
    }

    private Task throwBusinessException(string message)
    {
        throw new BusinessException(message);
    }

    public async Task UserRoleShouldExistWhenSelected(Domain.Entities.UserRole? userRole)
    {
        if (userRole == null)
            await throwBusinessException(UserRolesConstants.UserRoleNotExists);
    }

    public async Task UserRoleIdShouldExistWhenSelected(int id)
    {
        bool doesExist = await _userRoleRepository.AnyAsync(predicate: b => b.Id == id);
        if (doesExist)
            await throwBusinessException(UserRolesConstants.UserRoleNotExists);
    }

    public async Task UserRoleShouldNotExistWhenSelected(Domain.Entities.UserRole? userRole)
    {
        if (userRole != null)
            await throwBusinessException(UserRolesConstants.UserRoleAlreadyExists);
    }

    public async Task UserShouldNotHasRoleAlreadyWhenInsert(int userId, int roleId)
    {
        bool doesExist = await _userRoleRepository.AnyAsync(u =>
            u.UserId == userId && u.RoleId == roleId
        );
        if (doesExist)
            await throwBusinessException(UserRolesConstants.UserRoleAlreadyExists);
    }

    public async Task UserShouldNotHasRoleAlreadyWhenUpdated(int id, int userId, int roleId)
    {
        bool doesExist = await _userRoleRepository.AnyAsync(predicate: uoc =>
            uoc.Id == id && uoc.UserId == userId && uoc.RoleId == roleId
        );
        if (doesExist)
            await throwBusinessException(UserRolesConstants.UserRoleAlreadyExists);
    }
}
