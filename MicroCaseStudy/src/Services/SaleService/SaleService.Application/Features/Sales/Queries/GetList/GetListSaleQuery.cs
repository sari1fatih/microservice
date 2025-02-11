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

namespace SaleService.Application.Features.Sales.Queries.GetList;

public class GetListSaleQuery: IRequest<Response<GetListResponse<GetListSaleDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; } 
    public GetListSaleQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListSaleQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }
    
    public class GetListSaleQueryHandler
        : IRequestHandler<GetListSaleQuery,
            Response<GetListResponse<GetListSaleDto>>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService; 

        public GetListSaleQueryHandler(ISaleRepository saleRepository,
            IMapper mapper, IBaseService baseService)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListSaleDto>>> Handle(
            GetListSaleQuery request,
            CancellationToken cancellationToken
        )
        { 
            GetListResponse<GetListSaleDto> mappedUserRoleListModel = _mapper.Map<
                GetListResponse<GetListSaleDto>
            >(await _saleRepository.GetListView(request.DynamicQuery, request.PageRequest));
            return _baseService.CreateSuccessResult<GetListResponse<GetListSaleDto>>(
                mappedUserRoleListModel,
                InternalsConstants.Success
            );
        }
    }
    
}