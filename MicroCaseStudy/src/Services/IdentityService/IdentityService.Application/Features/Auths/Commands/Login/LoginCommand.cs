using System.Text.Json.Serialization;
using AutoMapper;
using Core.Application.Pipelines.Transaction;
using Core.Redis.Helpers;
using Core.Redis.MediaR;
using Core.Security.JWT.Dtos;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Auths.Rules;
using IdentityService.Application.Manager.AuthManager;
using IdentityService.Application.Manager.UserManager;
using IdentityService.Domain.Entities;
using MediatR;

namespace IdentityService.Application.Features.Auths.Commands.Login;

public class LoginCommand : IRequest<Response<LoginDto>>, ITransactionalRequest, IJwtAddRedisCachableRequest
{
    public UserForLoginDto UserForLoginDto { get; set; }
    public string IpAddress { get; set; }
    [JsonIgnore]
    public string Jwt { get; set; }
    [JsonIgnore]
    public string UserId { get; set; }
    [JsonIgnore]
    public DateTime ExpiresDate { get; set; }

    public LoginCommand()
    {
        UserForLoginDto = null!;
        IpAddress = string.Empty;
    }

    public LoginCommand(UserForLoginDto userForLoginDto, string ipAddress)
    {
        UserForLoginDto = userForLoginDto;
        IpAddress = ipAddress;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<LoginDto>>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IJwtAddRedisCachableRequest _jwtAddRedisCachableRequest;
        private readonly IAuthManager _authManager;
        private readonly IUserManager _userManager;
        private readonly IBaseService _baseService;
        private readonly IMapper _mapper;
        private readonly IDistributedHelper _distributedHelper;

        public LoginCommandHandler(
            IUserManager userManager,
            IJwtAddRedisCachableRequest jwtAddRedisCachableRequest,
            IAuthManager authManager,
            AuthBusinessRules authBusinessRules,
            IBaseService baseService,
            IDistributedHelper distributedHelper,
            IMapper mapper
        )
        {
            _jwtAddRedisCachableRequest=jwtAddRedisCachableRequest;
            _userManager = userManager;
            _authManager = authManager;
            _authBusinessRules = authBusinessRules;
            _baseService = baseService;
            _distributedHelper = distributedHelper;
            _mapper = mapper;
        }

        public async Task<Response<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.GetAsync(
                predicate: u => u.Email == request.UserForLoginDto.Email,
                cancellationToken: cancellationToken
            );

            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _authBusinessRules.UserPasswordShouldBeMatch(user!, request.UserForLoginDto.Password);
            LoginDto loginDto = new();

            AccessToken createdAccessToken = await _authManager.CreateAccessToken(user);

            Domain.Entities.RefreshToken createdRefreshToken;
            var newRefreshToken =
                await _authManager.CreateRefreshToken(user, request.IpAddress, createdAccessToken.Jti);


            createdRefreshToken = _mapper.Map<Domain.Entities.RefreshToken>(newRefreshToken);

            createdRefreshToken.CreatedBy = user.Id; 
            Domain.Entities.RefreshToken addedRefreshToken = await _authManager.AddRefreshToken(createdRefreshToken);

            loginDto.AccessToken = createdAccessToken;
            loginDto.RefreshToken = newRefreshToken.Token;
            _jwtAddRedisCachableRequest.Jwt = createdRefreshToken.Jti;
            _jwtAddRedisCachableRequest.UserId = user.Id.ToString();
            _jwtAddRedisCachableRequest.ExpiresDate = addedRefreshToken.ExpiresDate;
         
            return _baseService.CreateSuccessResult<LoginDto>(loginDto,
                @"Successfully logged in.");
        }
    }
}