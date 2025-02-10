using System.Text.Json.Serialization;
using AutoMapper;
using Core.Application.Pipelines.Transaction;
using Core.Redis.Helpers;
using Core.Redis.MediaR;
using Core.Security.Hashing;
using Core.Security.JWT.Dtos;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Auths.Commands.Redis;
using IdentityService.Application.Features.Auths.Rules;
using IdentityService.Application.Manager.AuthManager;
using IdentityService.Application.Manager.UserManager;
using IdentityService.Domain.Entities;
using MediatR;

namespace IdentityService.Application.Features.Auths.Commands.VerifyRegister;

public class VerifyRegisterCommand : IRequest<Response<VerifyRegisterDto>>, ITransactionalRequest, IJwtAddRedisCachableRequest
{
    public VerifyForRegisterDto VerifyForRegisterDto { get; set; }
    public string IpAddress { get; set; }
    [JsonIgnore]
    public string Jwt { get; set; }
    [JsonIgnore]
    public string UserId { get; set; }
    [JsonIgnore]
    public DateTime ExpiresDate { get; set; }
    public VerifyRegisterCommand()
    {
        VerifyForRegisterDto = null!;
        IpAddress = string.Empty;
    }

    public VerifyRegisterCommand(VerifyForRegisterDto verifyForRegisterDto, string ipAddress)
    {
        VerifyForRegisterDto = verifyForRegisterDto;
        IpAddress = ipAddress;
    }

    public class VerifyRegisterCommandHandler : IRequestHandler<VerifyRegisterCommand, Response<VerifyRegisterDto>>
    {
        private readonly IUserManager _userManager;
        private readonly IAuthManager _authManager;
        private readonly AuthBusinessRules _authBusinessRules; 
        private readonly IJwtAddRedisCachableRequest _jwtAddRedisCachableRequest;
        private readonly IBaseService _baseService;
        private readonly IDistributedHelper _distributedHelper; 
        private readonly IMapper _mapper;
        
        public VerifyRegisterCommandHandler(
            IUserManager userManager,
            IAuthManager authManager,
            AuthBusinessRules authBusinessRules, 
            IJwtAddRedisCachableRequest jwtAddRedisCachableRequest,
            IBaseService baseService,
            IDistributedHelper distributedHelper,
            IMapper mapper)
        {
            _userManager = userManager;
            _authManager = authManager;
            _authBusinessRules = authBusinessRules; 
            _jwtAddRedisCachableRequest= jwtAddRedisCachableRequest;
            _baseService = baseService;
            _distributedHelper = distributedHelper;
            _mapper = mapper;
        }

        public async Task<Response<VerifyRegisterDto>> Handle(VerifyRegisterCommand request,
            CancellationToken cancellationToken)
        {
            User? user = await _userManager.GetAsync(
                predicate: u =>
                    u.Email == request.VerifyForRegisterDto.Email ||
                    u.Username == request.VerifyForRegisterDto.Username,
                cancellationToken: cancellationToken
            );

            await _authBusinessRules.UserEmailShouldBeNotExists(
                user,request.VerifyForRegisterDto.Email);
 
            await _authBusinessRules.UserNameShouldBeNotExists(
                user,request.VerifyForRegisterDto.Username);
            
            RegisterRedisDto registerRedisDto = new RegisterRedisDto();
            
            registerRedisDto = await _distributedHelper.GetResponse(request.VerifyForRegisterDto.Email,
                registerRedisDto, cancellationToken);

            await _authBusinessRules.MatchActivationCode(registerRedisDto.ActivationCode, request.VerifyForRegisterDto.ActivationCode);

            HashingHelper.CreatePasswordHash(
                request.VerifyForRegisterDto.Password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
            );
            
            User newUser =
                new()
                {
                    Email = request.VerifyForRegisterDto.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Username = request.VerifyForRegisterDto.Username,
                    Name = request.VerifyForRegisterDto.Name,
                    Surname = request.VerifyForRegisterDto.Surname,
                    IsActive = true 
                };
            
            User createdUser = await _userManager.AddAsync(newUser);

            AccessToken createdAccessToken = await _authManager.CreateAccessToken(createdUser);

            Domain.Entities.RefreshToken createdRefreshToken;
            
            var newRefreshToken  = await _authManager.CreateRefreshToken(
                    createdUser,
                    request.IpAddress,
                    createdAccessToken.Jti
                );


            createdRefreshToken = _mapper.Map<Domain.Entities.RefreshToken>(newRefreshToken);
            
            createdRefreshToken.CreatedBy = createdUser.Id; 
            Domain.Entities.RefreshToken addedRefreshToken = await _authManager.AddRefreshToken
                (createdRefreshToken);
            
            VerifyRegisterDto registeredDto = new()
            {
                AccessToken = createdAccessToken,
                RefreshToken = newRefreshToken.Token
            };
            
            _jwtAddRedisCachableRequest.Jwt = createdRefreshToken.Jti;
            _jwtAddRedisCachableRequest.UserId = createdUser.Id.ToString();
            _jwtAddRedisCachableRequest.ExpiresDate = addedRefreshToken.ExpiresDate;
            
            await _distributedHelper.RemoveCache(string.Empty,request.VerifyForRegisterDto.Email,cancellationToken);
            return _baseService.CreateSuccessResult<VerifyRegisterDto>(registeredDto,
                InternalsConstants.Success);
        }
    } 
}