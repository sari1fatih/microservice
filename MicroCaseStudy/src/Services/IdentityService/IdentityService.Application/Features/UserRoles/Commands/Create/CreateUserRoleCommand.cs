using System.Text.Json.Serialization;
using AutoMapper;
using Core.Application.Enums;
using Core.Redis.MediaR;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Internals.Constants;
using IdentityService.Application.Features.UserRoles.Rules;
using IdentityService.Application.Manager.AuthManager;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.UserRoles.Commands.Create;

public class CreateUserRoleCommand : IRequest<Response<CreatedUserRoleDto>>, IJwtRemoveRedisCachableRequest
{   
    [JsonIgnore]
    public string Jwt { get; set; }
    [JsonIgnore]
    public string UserId { get; set; }

   
    [JsonIgnore]
    public bool IsDeletedUserAll { get; set; }
    public int ReqUserId { get; set; }
    public int RoleId { get; set; }


    public class CreateUserRoleCommandHandler
        : IRequestHandler<CreateUserRoleCommand, Response<CreatedUserRoleDto>>
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly UserRoleBusinessRules _userRoleBusinessRules;
        private readonly IJwtRemoveRedisCachableRequest _jwtRemoveRedisCachableRequest;
        private readonly IBaseService _baseService;
        private readonly IAuthManager _authManager;

        public CreateUserRoleCommandHandler(
            IUserRoleRepository userRoleRepository,
            IMapper mapper,
            UserRoleBusinessRules userRoleBusinessRules,
            IJwtRemoveRedisCachableRequest jwtRemoveRedisCachableRequest,
            IAuthManager authManager,
            IBaseService baseService
        )
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _authManager = authManager;
            _userRoleBusinessRules = userRoleBusinessRules;
            _jwtRemoveRedisCachableRequest = jwtRemoveRedisCachableRequest;
            _baseService = baseService;
        }

        public async Task<Response<CreatedUserRoleDto>> Handle(
            CreateUserRoleCommand request,
            CancellationToken cancellationToken
        )
        {
            await _userRoleBusinessRules.UserShouldNotHasRoleAlreadyWhenInsert(
                request.ReqUserId,
                request.RoleId
            );
            Domain.Entities.UserRole mappedUserRole = _mapper.Map<Domain.Entities.UserRole>(request);

            Domain.Entities.UserRole createdUserRole = await _userRoleRepository.AddAsync
            (mappedUserRole, TableCreatedParameters.CreatedAtPropertyName,
                TableCreatedParameters.CreatedByPropertyName);

            CreatedUserRoleDto createdUserRoleDto = _mapper.Map<CreatedUserRoleDto>(
                createdUserRole
            );

            _jwtRemoveRedisCachableRequest.Jwt = string.Empty;
            _jwtRemoveRedisCachableRequest.UserId = request.ReqUserId.ToString();
            _jwtRemoveRedisCachableRequest.IsDeletedUserAll = true;
            await _authManager.DeleteAllRefreshTokensByUserId(request.ReqUserId);
            return _baseService.CreateSuccessResult<CreatedUserRoleDto>(createdUserRoleDto,
                InternalsMessages.Success);
        }
    }
}