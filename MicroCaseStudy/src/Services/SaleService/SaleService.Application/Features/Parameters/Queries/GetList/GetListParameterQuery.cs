using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.Parameters.Queries.GetList;

public class GetListParameterQuery: IRequest<Response<GetListResponse<GetListParameterDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; } 
    public GetListParameterQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListParameterQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }
    
    public class GetListParameterQueryHandler
        : IRequestHandler<GetListParameterQuery,
            Response<GetListResponse<GetListParameterDto>>>
    {
        private readonly IParameterRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService; 

        public GetListParameterQueryHandler(IParameterRepository userRoleRepository,
            IMapper mapper, IBaseService baseService)
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListParameterDto>>> Handle(
            GetListParameterQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<Parameter> parameters = await _userRoleRepository.GetListByDynamicAsync(
                request.DynamicQuery,
                include: m => m
                    .Include(b => b.ParameterGroup),  
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false
            );
            
            GetListResponse<GetListParameterDto> mappedUserRoleListModel = _mapper.Map<
                GetListResponse<GetListParameterDto>
            >(parameters);
            
            return _baseService.CreateSuccessResult<GetListResponse<GetListParameterDto>>(
                mappedUserRoleListModel,
                InternalsConstants.Success
            );
        }
    }
}