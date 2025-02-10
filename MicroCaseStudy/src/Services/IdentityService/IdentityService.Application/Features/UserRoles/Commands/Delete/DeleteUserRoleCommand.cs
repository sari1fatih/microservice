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

namespace IdentityService.Application.Features.UserRoles.Commands.Delete;

public class DeleteUserRoleCommand : IRequest<Response<DeletedUserRoleDto>>, IJwtRemoveRedisCachableRequest
{
    public int Id { get; set; }
    [JsonIgnore]
    public string Jwt { get; set; }
    [JsonIgnore]
    public string UserId { get; set; }
    [JsonIgnore]
    public bool IsDeletedUserAll { get; set; }
    public class DeleteUserRoleCommandHandler
        : IRequestHandler<DeleteUserRoleCommand, Response<DeletedUserRoleDto>>
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly UserRoleBusinessRules _userRoleBusinessRules;
        private readonly IJwtRemoveRedisCachableRequest _jwtRemoveRedisCachableRequest;
        private readonly IBaseService _baseService; 
        private readonly IAuthManager _authManager;
        public DeleteUserRoleCommandHandler(
            IUserRoleRepository userRoleRepository,
            IMapper mapper,
            UserRoleBusinessRules userRoleBusinessRules,
            IJwtRemoveRedisCachableRequest jwtRemoveRedisCachableRequest,
            IBaseService baseService,
            IAuthManager authManager
            
        )
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _userRoleBusinessRules = userRoleBusinessRules;
            _jwtRemoveRedisCachableRequest= jwtRemoveRedisCachableRequest;
            _baseService = baseService;
            _authManager = authManager;
        }

        public async Task<Response<DeletedUserRoleDto>> Handle(
            DeleteUserRoleCommand request,
            CancellationToken cancellationToken
        )
        {
            Domain.Entities.UserRole? userRole = await _userRoleRepository.GetAsync(
                predicate: uoc => uoc.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _userRoleBusinessRules.UserRoleShouldExistWhenSelected(userRole);

            await _userRoleRepository.DeleteAsync(userRole!,TableDeletedParameters
                .DeletedAtPropertyName,TableDeletedParameters.DeletedByPropertyName,TableDeletedParameters.IsDeletedPropertyName);

            DeletedUserRoleDto dto =
                _mapper.Map<DeletedUserRoleDto>(userRole);
            
            _jwtRemoveRedisCachableRequest.Jwt = string.Empty;
            _jwtRemoveRedisCachableRequest.UserId =userRole?.UserId.ToString();
            _jwtRemoveRedisCachableRequest.IsDeletedUserAll = true;
            
            await _authManager.DeleteAllRefreshTokensByUserId(userRole.UserId);
            
            return _baseService.CreateSuccessResult<DeletedUserRoleDto>(dto,
                InternalsMessages.Success);
        }
    }
 
}