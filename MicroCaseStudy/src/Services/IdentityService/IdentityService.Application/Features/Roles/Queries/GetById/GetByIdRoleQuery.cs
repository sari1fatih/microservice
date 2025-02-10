using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Roles.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Roles.Queries.GetById;

public class GetByIdRoleQuery: IRequest<Response< GetByIdRoleDto>>
{
    public int Id { get; set; }
 
    public class GetByIdRoleQueryHandler : IRequestHandler<GetByIdRoleQuery, Response< 
        GetByIdRoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly RoleBusinessRules _roleBusinessRules;
        private readonly IBaseService _baseService;
        public GetByIdRoleQueryHandler(
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

        public async Task<Response<GetByIdRoleDto>> Handle(
            GetByIdRoleQuery request,
            CancellationToken cancellationToken
        )
        {
            Role? role = await _roleRepository.GetAsync(
                predicate: b => b.Id == request.Id,
                cancellationToken: cancellationToken,
                enableTracking: false
            );
            await _roleBusinessRules.RoleShouldExistWhenSelected(role);

            GetByIdRoleDto dto = _mapper.Map<GetByIdRoleDto>(role);
            return _baseService.CreateSuccessResult<GetByIdRoleDto>(dto,
                InternalsConstants.Success);
        }
    }
}
