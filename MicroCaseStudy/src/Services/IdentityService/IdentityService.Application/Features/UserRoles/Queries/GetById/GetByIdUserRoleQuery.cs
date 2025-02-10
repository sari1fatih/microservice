using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.UserRoles.Rules;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Features.UserRoles.Queries.GetById;

public class GetByIdUserRoleQuery: IRequest<Response<GetByIdUserRoleDto>>
{
    public int Id { get; set; }

    public class GetByIdUserRoleQueryHandler
        : IRequestHandler<GetByIdUserRoleQuery,Response<GetByIdUserRoleDto>>
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly UserRoleBusinessRules _userRoleBusinessRules;
        private readonly IBaseService _baseService; 
        public GetByIdUserRoleQueryHandler(
            IUserRoleRepository userRoleRepository,
            IMapper mapper,
            UserRoleBusinessRules userRoleBusinessRules,
            IBaseService baseService
        )
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _userRoleBusinessRules = userRoleBusinessRules;
            _baseService = baseService; 
        }

        public async Task<Response<GetByIdUserRoleDto>> Handle(
            GetByIdUserRoleQuery request,
            CancellationToken cancellationToken
        )
        {
            Domain.Entities.UserRole? userRole = await _userRoleRepository.GetAsync(
                predicate: b => b.Id.Equals(request.Id),
                include: m => m.Include(b => b.User).Include(b => b.Role),   
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            await _userRoleBusinessRules.UserRoleShouldExistWhenSelected(userRole);

            GetByIdUserRoleDto userRoleDto = _mapper.Map<GetByIdUserRoleDto>(
                userRole
            );
            return _baseService.CreateSuccessResult<GetByIdUserRoleDto>(userRoleDto,InternalsConstants.Success);
        }
    }
}
