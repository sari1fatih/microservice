using System.Web;
using Core.Mailing;
using Core.Redis.Helpers;
using Core.Security.EmailAuthenticator;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Auths.Commands.Redis;
using IdentityService.Application.Features.Auths.Rules;
using IdentityService.Application.Manager.UserManager;
using IdentityService.Domain.Entities;
using MediatR;
using MimeKit;

namespace IdentityService.Application.Features.Auths.ResetPassword;

public class ResetPasswordCommand : IRequest<Response<string>>
{

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Response<string>>
    {
        private readonly IUserSession<int> _userSession;
        private readonly IUserManager _userManager;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IBaseService _baseService; 
        private readonly IMailService _mailService;
        private readonly IDistributedHelper _distributedHelper;
        private readonly IEmailAuthenticatorHelper _emailAuthenticatorHelper;

        public ResetPasswordCommandHandler(
            IUserSession<int> userSession,
            IUserManager userManager,
            AuthBusinessRules authBusinessRules,
            IBaseService baseService, 
            IMailService mailService,
            IDistributedHelper distributedHelper,
            IEmailAuthenticatorHelper emailAuthenticatorHelper
        )
        {
            _userSession = userSession;
            _userManager = userManager;
            _authBusinessRules = authBusinessRules;
            _baseService = baseService; 
            _mailService = mailService;
            _distributedHelper = distributedHelper;
            _emailAuthenticatorHelper = emailAuthenticatorHelper;
        }

        public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.GetAsync(
                predicate: u => u.Email == _userSession.Email,
                cancellationToken: cancellationToken
            );
            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);

            var code = await _emailAuthenticatorHelper.CreateEmailActivationCode();

            RegisterRedisDto registerRedisDto = new RegisterRedisDto();
            registerRedisDto.Mail = _userSession.Email;
            registerRedisDto.ActivationCode = code;

            await _distributedHelper.RemoveCache(string.Empty, _userSession.Email, cancellationToken);

            var isRegisterRedisDto = await _distributedHelper.GetResponseAndAddToCache(
                RedisConstants.ResetPassword,
                _userSession.Email,
                registerRedisDto,
                cancellationToken);


            var toEmailList = new List<MailboxAddress> { new(name: _userSession.Email, _userSession.Email) };
            _mailService.SendMail(
                new Mail
                {
                    ToList = toEmailList,
                    Subject = "Reset Password Your Email - IdentityService",
                    TextBody =
                        $"Click on the link to verify your email: ActivationKey={HttpUtility.UrlEncode(isRegisterRedisDto.ActivationCode)}",
                    HtmlBody =
                        $"<p>Click on the link to verify your email: ActivationKey={HttpUtility.UrlEncode(isRegisterRedisDto.ActivationCode)}</p>"
                }
            );

            return _baseService.CreateSuccessResult<string>(string.Empty,
                InternalsConstants.Success);
        }
    }
}