using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Internals.Constants;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

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
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService; 

        public GetListUserRoleQueryHandler(IUserRoleRepository userRoleRepository,
            IMapper mapper, IBaseService baseService)
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListUserRoleDto>>> Handle(
            GetListUserRoleQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<UserRole> userRoles = await _userRoleRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false
            );

            GetListResponse<GetListUserRoleDto> mappedUserRoleListModel = _mapper.Map<
                GetListResponse<GetListUserRoleDto>
            >(userRoles);
            return _baseService.CreateSuccessResult<GetListResponse<GetListUserRoleDto>>(
                mappedUserRoleListModel,
                InternalsMessages.Success
                );
        }
    }
}