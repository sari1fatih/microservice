using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Security.Hashing;
using IdentityService.Application.Features.Auths.Constants;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Features.Auths.Rules;

public class AuthBusinessRules : BaseBusinessRules
{
    
    public AuthBusinessRules()
    {
    } 
    public async Task UserShouldBeExistsWhenSelected(User? user)
    {
        if (user == null)
            throw new BusinessException(AuthMessages.UserDontExists);
    } 

    
    public async Task RefreshTokenShouldBeExists(RefreshToken? refreshToken)
    {
        if (refreshToken == null)
            throw new BusinessException(AuthMessages.RefreshDontExists);
    }

    public async Task RefreshTokenShouldBeActive(RefreshToken refreshToken)
    {
        if (refreshToken.RevokedDate != null && DateTime.UtcNow >= refreshToken.ExpiresDate)
            throw new BusinessException(AuthMessages.InvalidRefreshToken);
    }

    public async Task UserEmailShouldBeNotExists(User? user,string email)
    {
        if (user != null && user.Email == email)
            throw new BusinessException(AuthMessages.UserMailAlreadyExists);
    }
    
    public async Task UserNameShouldBeNotExists(User? user,string userName)
    {
        if (user != null && user.Username == userName)
            throw new BusinessException(AuthMessages.UsernameAlreadyExists);
    }
 

    public async Task UserPasswordShouldBeMatch(User user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user!.PasswordHash, user.PasswordSalt))
            throw new BusinessException(AuthMessages.PasswordDontMatch);
    }

    public async Task MatchActivationCode(string activationCodeOld, string activationCodeNew)
    {
        if (activationCodeOld != activationCodeNew)
            throw new BusinessException(AuthMessages.ActivationCodeMismatch);
    }
}