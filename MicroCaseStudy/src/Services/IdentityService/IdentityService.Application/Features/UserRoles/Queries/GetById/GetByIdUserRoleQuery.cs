using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.UserRoles.Rules;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Features.UserRoles.Queries.GetById;

public class GetByIdUserRoleQuery : IRequest<Response<GetByIdUserRoleDto>>
{
    public int Id { get; set; }

    public class GetByIdUserRoleQueryHandler
        : IRequestHandler<GetByIdUserRoleQuery, Response<GetByIdUserRoleDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserRoleBusinessRules _userRoleBusinessRules;
        private readonly IBaseService _baseService;

        public GetByIdUserRoleQueryHandler(
            IMapper mapper,
            UserRoleBusinessRules userRoleBusinessRules,
            IBaseService baseService,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRoleBusinessRules = userRoleBusinessRules;
            _baseService = baseService;
            _userRepository = userRepository;
        }

        public async Task<Response<GetByIdUserRoleDto>> Handle(
            GetByIdUserRoleQuery request,
            CancellationToken cancellationToken
        )
        {
            GetByIdUserRoleDto getByIdUserRoleDto = await _userRepository
                .Query()
                .AsNoTracking()
                .Include(x => x.UserRoleUsers)
                .ThenInclude(x => x.Role)
                .Where(x => x.Id.Equals(request.Id))
                .Select(x => new GetByIdUserRoleDto()
                {
                    Id = x.Id,
                    Username = x.Username,
                    Name = x.Name,
                    Surname = x.Surname,
                    Roles = x.UserRoleUsers.Select(a => new GetByIdUserRoleDetailDto
                    {
                        RoleId = a.RoleId,
                        RoleValue = a.Role.RoleValue
                    }).ToList()
                }).FirstOrDefaultAsync();
            
            GetByIdUserRoleDto userRoleDto = _mapper.Map<GetByIdUserRoleDto>(
                getByIdUserRoleDto
            );
            return _baseService.CreateSuccessResult<GetByIdUserRoleDto>(userRoleDto, InternalsConstants.Success);
        }
    }
}