using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using SaleService.Application.Features.Sales.Commands.CreateSale;
using SaleService.Domain.Dtos.SaleDtos;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.Sales.Commands.UpdateSaleCustomer;

public class UpdateSaleCustomerCommand:IRequest<Response<UpdateSaleCustomerDto>>
{
    public int? CustomerId { get; set; }

    public int UserId { get; set; }
    
    public string? CustomerName { get; set; }

    public string? CustomerSurname { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerEmail { get; set; }
     
    
    public class UpdateSaleCustomerCommandHandler : IRequestHandler<UpdateSaleCustomerCommand,
        Response<UpdateSaleCustomerDto>>
    {
        private readonly ISaleRepository _saleRepository; 
        private readonly IBaseService _baseService;
        private readonly IMapper _mapper;
        public UpdateSaleCustomerCommandHandler(
            ISaleRepository saleRepository,
            IMapper mapper,
            IBaseService baseService
        )
        {
            _saleRepository = saleRepository;
            _baseService = baseService;
            _mapper = mapper;
        }

        public async Task<Response<UpdateSaleCustomerDto>> Handle(
            UpdateSaleCustomerCommand request,
            CancellationToken cancellationToken
        )
        {
            UpdateAllCustomer updateAllCustomer = _mapper.Map<UpdateAllCustomer>(request); 
            await _saleRepository.UpdateAllCustomerAsync(updateAllCustomer);
            return _baseService.CreateSuccessResult<UpdateSaleCustomerDto>(null,
                InternalsConstants.Success);
        }
 
    } 
    
    
    
}