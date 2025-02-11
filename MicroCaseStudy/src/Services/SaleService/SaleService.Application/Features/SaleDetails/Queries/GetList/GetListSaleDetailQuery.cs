using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleService.Application.Features.Sales.Queries.GetList;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.SaleDetails.Queries.GetList;

public class GetListSaleDetailQuery: IRequest<Response<GetListResponse<GetListSaleDetailDto>>>
{
    public int saleId { get; set; }
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; } 
    public GetListSaleDetailQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListSaleDetailQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }
    
    public class GetListSaleQueryHandler
        : IRequestHandler<GetListSaleDetailQuery,
            Response<GetListResponse<GetListSaleDetailDto>>>
    {
        private readonly ISaleDetailRepository _saleDetailRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService; 

        public GetListSaleQueryHandler(ISaleDetailRepository saleDetailRepository,
            IMapper mapper, IBaseService baseService)
        {
            _saleDetailRepository = saleDetailRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListSaleDetailDto>>> Handle(
            GetListSaleDetailQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<SaleDetail> sales = await _saleDetailRepository.GetListByDynamicAsync(
                request.DynamicQuery, 
                include: m => m.Include(b => b.SaleStatusParameter), 
                predicate:x => x.SaleId == request.saleId,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false
            );

            GetListResponse<GetListSaleDetailDto> saleDetailDto = _mapper.Map<
                GetListResponse<GetListSaleDetailDto>
            >(sales);
            return _baseService.CreateSuccessResult<GetListResponse<GetListSaleDetailDto>>(
                saleDetailDto,
                InternalsConstants.Success
            );
        }
    }
    
}