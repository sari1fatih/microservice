using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.SaleDetails.Queries.GetById;

public class GetByIdSaleDetailQuery: IRequest<Response<GetByIdSaleDetailDto>>
{
    public int Id { get; set; }
    
    public class GetByIdSaleDetailQueryHandler
        : IRequestHandler<GetByIdSaleDetailQuery,Response<GetByIdSaleDetailDto>>
    {
        private readonly ISaleDetailRepository _saleDetailRepository;
        private readonly IMapper _mapper;  
        private readonly IBaseService _baseService; 
        
        public GetByIdSaleDetailQueryHandler(
            ISaleDetailRepository saleDetailRepository,
            IMapper mapper,
            IBaseService baseService
        )
        {
            _saleDetailRepository = saleDetailRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetByIdSaleDetailDto>> Handle(
            GetByIdSaleDetailQuery request,
            CancellationToken cancellationToken
        )
        {
            SaleDetail? saleDetail = await _saleDetailRepository.GetAsync(
                predicate: b => b.Id.Equals(request.Id),
                include: m => m.Include(b => b.SaleStatusParameter),  
                enableTracking: false,
                cancellationToken: cancellationToken
            );  

            GetByIdSaleDetailDto getByIdSaleDto = _mapper.Map<GetByIdSaleDetailDto>(saleDetail);
            
            return _baseService.CreateSuccessResult<GetByIdSaleDetailDto>(getByIdSaleDto,InternalsConstants.Success);
        }
    }
}