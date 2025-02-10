using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Roles.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Roles.Commands.Create;

public class CreateRoleCommand: IRequest<Response< CreatedRoleDto>>
{
    public string RoleValue { get; set; }

    public CreateRoleCommand()
    {
        RoleValue = string.Empty;
    }

    public CreateRoleCommand(string roleValue)
    {
        RoleValue = roleValue;
    }
    
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand,
        Response<CreatedRoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly RoleBusinessRules _roleBusinessRules;
        private readonly IBaseService _baseService;

        public CreateRoleCommandHandler(
            IRoleRepository roleRepository,
            IMapper mapper,
            RoleBusinessRules roleBusinessRules,
            IBaseService baseService
        )
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _roleBusinessRules = roleBusinessRules;
            _baseService = baseService;
        }

        public async Task<Response< CreatedRoleDto>> Handle(
            CreateRoleCommand request,
            CancellationToken cancellationToken
        )
        {
            await _roleBusinessRules.RoleNameShouldNotExistWhenCreating(request.RoleValue);
            Role mappedRoleClaim = _mapper.Map<Role>(request);
            mappedRoleClaim.IsActive = true;
            Role createdRole = await _roleRepository.AddAsync(mappedRoleClaim,
                TableCreatedParameters.CreatedAtPropertyName,TableCreatedParameters.CreatedByPropertyName);

            CreatedRoleDto dto = _mapper.Map<CreatedRoleDto>(createdRole);
            return _baseService.CreateSuccessResult<CreatedRoleDto>(dto,
                InternalsConstants.Success);
        }
    }
}