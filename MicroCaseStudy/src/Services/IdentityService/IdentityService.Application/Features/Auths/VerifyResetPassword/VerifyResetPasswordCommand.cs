using Core.Redis.Helpers;
using Core.Security.Hashing;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Auths.Commands.Redis;
using IdentityService.Application.Features.Auths.Rules;
using IdentityService.Application.Manager.UserManager;
using IdentityService.Domain.Entities;
using MediatR;

namespace IdentityService.Application.Features.Auths.VerifyResetPassword;

public class VerifyResetPasswordCommand : IRequest<Response<string>>
{
    public string Password { get; set; }
    public string ActivationCode { get; set; }

    public VerifyResetPasswordCommand()
    {
        this.Password = string.Empty;
        this.ActivationCode = string.Empty;
    }

    public class RegisterCommandHandler : IRequestHandler<VerifyResetPasswordCommand, Response<string>>
    {
        private readonly IUserSession<int> _userSession;
        private readonly IUserManager _userManager;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IBaseService _baseService; 
        private readonly IDistributedHelper _distributedHelper;

        public RegisterCommandHandler(
            IUserSession<int> userSession,
            IUserManager userManager,
            AuthBusinessRules authBusinessRules,
            IBaseService baseService, 
            IDistributedHelper distributedHelper)
        {
            _userSession = userSession;
            _userManager = userManager;
            _authBusinessRules = authBusinessRules;
            _baseService = baseService; 
            _distributedHelper = distributedHelper;
        }

        public async Task<Response<string>> Handle(VerifyResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            User? user = await _userManager.GetAsync(
                predicate: u =>
                    u.Email == _userSession.Email,
                cancellationToken: cancellationToken
            );

            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);

            RegisterRedisDto registerRedisDto = new RegisterRedisDto();
            registerRedisDto = await _distributedHelper.GetResponse(_userSession.Email,
                registerRedisDto, cancellationToken);

            await _authBusinessRules.MatchActivationCode(registerRedisDto.ActivationCode, request.ActivationCode);

            HashingHelper.CreatePasswordHash(
                request.Password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
            );

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userManager.UpdateAsync(user);
            await _distributedHelper.RemoveCache(string.Empty, _userSession.Email, cancellationToken);
            return _baseService.CreateSuccessResult<string>(string.Empty,
                InternalsConstants.Success);
        }
    }
}