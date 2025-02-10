using AutoMapper;
using Core.Application.Enums;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Roles.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Features.Roles.Commands.Delete;

public class DeleteRoleCommand : IRequest<Response<DeletedRoleDto>>
{
    public int Id { get; set; }

    public class
        DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand,
        Response<DeletedRoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly RoleBusinessRules _roleBusinessRules;
        private readonly IBaseService _baseService;

        public DeleteRoleCommandHandler(
            IRoleRepository roleRepository,
            IMapper mapper,
            RoleBusinessRules roleBusinessRules,
            IUserRoleRepository userRoleRepository,
            IBaseService baseService
        )
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _roleBusinessRules = roleBusinessRules;
            _userRoleRepository = userRoleRepository;
            _baseService = baseService;
        }

        public async Task<Response<DeletedRoleDto>> Handle(
            DeleteRoleCommand request,
            CancellationToken cancellationToken
        )
        {
            Role? role = await _roleRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                include: m=> m.Include(b=> b.UserRoles),
                cancellationToken: cancellationToken
            );
            
            await _roleBusinessRules.RoleShouldExistWhenSelected(role);
            await _roleBusinessRules.RoleInUse(role.UserRoles);

           
                
            
            await _roleRepository.DeleteAsync(entity: role!,TableDeletedParameters
                .DeletedAtPropertyName,TableDeletedParameters.DeletedByPropertyName,TableDeletedParameters
                .IsDeletedPropertyName);

            DeletedRoleDto dto = _mapper.Map<DeletedRoleDto>(role);

            return _baseService.CreateSuccessResult<DeletedRoleDto>(dto,
                InternalsConstants.Success);
        }
    }
}