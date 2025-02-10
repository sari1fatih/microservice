using System.Text.Json.Serialization;
using System.Web;
using Core.Mailing;
using Core.Redis.Helpers;
using Core.Security.EmailAuthenticator;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Auths.Commands.Redis;
using IdentityService.Application.Features.Auths.Rules;
using IdentityService.Application.Manager.UserManager;
using IdentityService.Domain.Entities;
using MediatR;
using MimeKit;

namespace IdentityService.Application.Features.Auths.Commands.Register;

public class RegisterCommand : IRequest<Response<string>>
{
    public string Email { get; set; }
    [JsonIgnore] public string IpAddress { get; set; }

    public RegisterCommand()
    {
        Email = string.Empty;
        IpAddress = string.Empty;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response<string>>
    {
        private readonly IUserManager _userManager;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IBaseService _baseService; 
        private readonly IMailService _mailService;
        private readonly IDistributedHelper _distributedHelper;
        private readonly IEmailAuthenticatorHelper _emailAuthenticatorHelper;

        public RegisterCommandHandler(
            IUserManager userManager,
            AuthBusinessRules authBusinessRules,
            IBaseService baseService, 
            IMailService mailService,
            IDistributedHelper distributedHelper,
            IEmailAuthenticatorHelper emailAuthenticatorHelper
        )
        {
            _userManager = userManager;
            _authBusinessRules = authBusinessRules;
            _baseService = baseService; 
            _mailService = mailService;
            _distributedHelper = distributedHelper;
            _emailAuthenticatorHelper = emailAuthenticatorHelper;
        }

        public async Task<Response<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.GetAsync(
                predicate: u => u.Email == request.Email,
                cancellationToken: cancellationToken
            );
            
            await _authBusinessRules.UserEmailShouldBeNotExists(
                user,request.Email);
  
            var code = await _emailAuthenticatorHelper.CreateEmailActivationCode();

            RegisterRedisDto registerRedisDto = new RegisterRedisDto();
            registerRedisDto.Mail = request.Email;
            registerRedisDto.ActivationCode = code;
            
            var isRegisterRedisDto = await _distributedHelper.GetResponseAndAddToCache(
                RedisConstants.Register,
                request.Email,
                registerRedisDto,
                cancellationToken);


            var toEmailList = new List<MailboxAddress> { new(name: request.Email, request.Email) };
            _mailService.SendMail(
                new Mail
                {
                    ToList = toEmailList,
                    Subject = "Register Your Email - IdentityService",
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