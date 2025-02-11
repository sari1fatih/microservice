using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.ParameterGroups.Queries.GetList;

public class GetListParameterGroupQuery: IRequest<Response<GetListResponse<GetListParameterGroupDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; } 
    public GetListParameterGroupQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListParameterGroupQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }
    
    public class GetListParameterGroupQueryHandler
        : IRequestHandler<GetListParameterGroupQuery,
            Response<GetListResponse<GetListParameterGroupDto>>>
    {
        private readonly IParameterGroupRepository _parameterGroupRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService; 

        public GetListParameterGroupQueryHandler(IParameterGroupRepository parameterGroupRepository,
            IMapper mapper, IBaseService baseService)
        {
            _parameterGroupRepository = parameterGroupRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListParameterGroupDto>>> Handle(
            GetListParameterGroupQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<ParameterGroup> parameterGroups = await _parameterGroupRepository.GetListByDynamicAsync(
                request.DynamicQuery,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false
            );
            
            GetListResponse<GetListParameterGroupDto> mappedParameterGroups = _mapper.Map<
                GetListResponse<GetListParameterGroupDto>
            >(parameterGroups);
            return _baseService.CreateSuccessResult<GetListResponse<GetListParameterGroupDto>>(
                mappedParameterGroups,
                InternalsConstants.Success
            );
            
        }
    }
    
}