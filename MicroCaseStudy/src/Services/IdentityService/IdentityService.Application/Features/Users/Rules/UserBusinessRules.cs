using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Security.Hashing;
using IdentityService.Application.Features.Users.Constants;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;

namespace IdentityService.Application.Features.Users.Rules;

public class UserBusinessRules: BaseBusinessRules
{
    private readonly IUserRepository _userRepository; 

    public UserBusinessRules(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    private Task throwBusinessException(string message)
    {
        throw new BusinessException(message);
    }

    public async Task UserShouldBeExistsWhenSelected(User? user)
    {
        if (user == null)
            await throwBusinessException(UsersConstants.UserDontExists);
    }
    
    public async Task UserShouldBeExistsWhenSelected(int userId)
    {
        bool doesExists = await _userRepository.AnyAsync(predicate: u => u.Id ==userId);
        if (!doesExists)
            await throwBusinessException(UsersConstants.UserDontExists);
    }
 

    public async Task UserPasswordShouldBeMatched(User user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            await throwBusinessException(UsersConstants.PasswordDontMatch);
    }

    public async Task UserEmailShouldNotExistsWhenInsert(string email)
    {
        bool doesExists = await _userRepository.AnyAsync(predicate: u => u.Email == email);
        if (doesExists)
            await throwBusinessException(UsersConstants.UserMailAlreadyExists);
    }

    public async Task UserEmailShouldNotExistsWhenUpdate(int id, string email)
    {
        bool doesExists = await _userRepository.AnyAsync(predicate: u => u.Id != id && u.Email == email);
        if (doesExists)
            await throwBusinessException(UsersConstants.UserMailAlreadyExists);
    }
}
