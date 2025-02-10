using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Roles.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Roles.Commands.Update;

public class UpdateRoleCommand: IRequest<Response< UpdatedRoleDto>>
{
    public int Id { get; set; }
    public string RoleValue { get; set; }

    public UpdateRoleCommand()
    {
        RoleValue = string.Empty;
    }

    public UpdateRoleCommand(int id, string roleValue)
    {
        Id = id;
        RoleValue = roleValue;
    }
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand,Response< 
        UpdatedRoleDto>>
    {
        
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly RoleBusinessRules _roleBusinessRules;
        private readonly IBaseService _baseService;

        public UpdateRoleCommandHandler(
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

        public async Task<Response< UpdatedRoleDto>> Handle(
            UpdateRoleCommand request,
            CancellationToken cancellationToken
        )
        {
            Role? role = await _roleRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _roleBusinessRules.RoleShouldExistWhenSelected(role);
            await _roleBusinessRules.RoleNameShouldNotExistWhenUpdating(request.Id, request.RoleValue);
            Role mappedRole = _mapper.Map(request, destination: role!);

            Role updatedRole = await _roleRepository.UpdateAsync(mappedRole,
                TableUpdatedParameters.UpdatedAtPropertyName,TableUpdatedParameters.UpdatedByPropertyName);
 
            return _baseService.CreateSuccessResult<UpdatedRoleDto>(null,
                InternalsConstants.Success);
        }
    }
}