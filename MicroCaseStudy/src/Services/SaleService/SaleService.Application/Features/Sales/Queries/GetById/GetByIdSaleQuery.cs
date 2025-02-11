using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.Sales.Queries.GetById;

public class GetByIdSaleQuery: IRequest<Response<GetByIdSaleDto>>
{
    public int Id { get; set; }
    
    public class GetListSaleQueryHandler
        : IRequestHandler<GetByIdSaleQuery,Response<GetByIdSaleDto>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;  
        private readonly IBaseService _baseService; 
        
        public GetListSaleQueryHandler(
            ISaleRepository saleRepository,
            IMapper mapper,
            IBaseService baseService
        )
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetByIdSaleDto>> Handle(
            GetByIdSaleQuery request,
            CancellationToken cancellationToken
        )
        {
             
            var data = await _saleRepository.GetView(request.Id);  

            GetByIdSaleDto getByIdSaleDto = _mapper.Map<GetByIdSaleDto>(data);
            
            return _baseService.CreateSuccessResult<GetByIdSaleDto>(getByIdSaleDto,InternalsConstants.Success);
        }
    }
}