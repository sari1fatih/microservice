using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Roles.Queries.GetList;

public class GetListRoleQuery : IRequest<Response<GetListResponse<GetListRoleDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; } 
    public GetListRoleQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListRoleQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListRoleQueryHandler
        : IRequestHandler<GetListRoleQuery, Response<GetListResponse<GetListRoleDto>>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService;

        public GetListRoleQueryHandler(IRoleRepository roleRepository, IMapper mapper,
            IBaseService baseService)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListRoleDto>>> Handle(
            GetListRoleQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<Role> role = await _roleRepository.GetListByDynamicAsync(
                request.DynamicQuery,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false
            );

            GetListResponse<GetListRoleDto> response = _mapper.Map<
                GetListResponse<GetListRoleDto>
            >(role);
            return _baseService.CreateSuccessResult<GetListResponse<GetListRoleDto>>(response,
                InternalsConstants.Success);
        }
    }
}