using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Features.UserRoles.Queries.GetList;

public class GetListUserRoleQuery : IRequest<Response<GetListResponse<GetListUserRoleDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; }

    public GetListUserRoleQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListUserRoleQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListUserRoleQueryHandler
        : IRequestHandler<GetListUserRoleQuery,
            Response<GetListResponse<GetListUserRoleDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService;

        public GetListUserRoleQueryHandler(
            IMapper mapper, IBaseService baseService, IUserRepository userRepository)
        {
            _mapper = mapper;
            _baseService = baseService;
            _userRepository = userRepository;
        }

        public async Task<Response<GetListResponse<GetListUserRoleDto>>> Handle(
            GetListUserRoleQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<GetListUserRoleDto> data = await _userRepository
                .Query()
                .AsNoTracking()
                .ToDynamic(request.DynamicQuery)
                .Include(x => x.UserRoleUsers)
                .ThenInclude(x => x.Role)
                .Select(x => new GetListUserRoleDto()
                {
                    Id = x.Id,
                    Username = x.Username,
                    Name = x.Name,
                    Surname = x.Surname,
                    Roles = x.UserRoleUsers.Select(x => new GetListUserRoleDetailDto()
                    {
                        RoleId = x.RoleId,
                        RoleValue = x.Role.RoleValue
                    }).ToList()
                })
                .ToPaginateAsync(request.PageRequest.PageIndex, request.PageRequest.PageSize, cancellationToken);

            GetListResponse<GetListUserRoleDto> mappedUserRoleListModel = _mapper.Map<
                GetListResponse<GetListUserRoleDto>
            >(data);
            return _baseService.CreateSuccessResult<GetListResponse<GetListUserRoleDto>>(
                mappedUserRoleListModel,
                InternalsConstants.Success
            );
        }
    }
}