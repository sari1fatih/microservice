using System.Linq.Expressions;
using Core.Application.Enums;
using Core.Persistance.Paging;
using IdentityService.Application.Features.Users.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace IdentityService.Application.Manager.UserManager;

public class UserManager: IUserManager
{
    private readonly IUserRepository _userRepository;
    private readonly UserBusinessRules _userBusinessRules;

    public UserManager(IUserRepository userRepository, UserBusinessRules userBusinessRules)
    {
        _userRepository = userRepository;
        _userBusinessRules = userBusinessRules;
    }

    public async Task<User?> GetAsync(
        Expression<Func<User, bool>> predicate,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        User? user = await _userRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return user;
    }

    public async Task<Paginate<User>?> GetListAsync(
        Expression<Func<User, bool>>? predicate = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Paginate<User> userList = await _userRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userList;
    }

    public async Task<User> AddAsync(User user)
    {
        await _userBusinessRules.UserEmailShouldNotExistsWhenInsert(user.Email);

        User addedUser = await _userRepository.AddAsync(user,TableCreatedParameters.CreatedAtPropertyName,
            TableCreatedParameters.CreatedByPropertyName);

        return addedUser;
    }
    
    public async Task<User> AddAsyncForSocialMedia(User user)
    {
        User addedUser = await _userRepository.AddAsync(user,TableCreatedParameters.CreatedAtPropertyName,
            TableCreatedParameters.CreatedByPropertyName);

        return addedUser;
    }

    public async Task<User> UpdateAsync(User user)
    {
        await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(user.Id, user.Email);

        User updatedUser = await _userRepository.UpdateAsync(user,TableUpdatedParameters.UpdatedAtPropertyName,TableUpdatedParameters.UpdatedByPropertyName);

        return updatedUser;
    }

    public async Task<User> DeleteAsync(User user, bool permanent = false)
    {
        User deletedUser = await _userRepository.DeleteAsync(user,TableDeletedParameters.DeletedAtPropertyName,
            TableDeletedParameters.DeletedByPropertyName,TableDeletedParameters.IsDeletedPropertyName);

        return deletedUser;
    }
}