using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using SaleService.Application.Features.Constants;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.Sales.Commands.CreateSale;

public class CreateSaleCommand:IRequest<Response<CreateSaleDto>>
{
    public int? CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerSurname { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerEmail { get; set; }

    public string? SaleName { get; set; }
    public int? CreatedBy { get; set; }
    public string? Note { get; set; }
    
    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand,
        Response<CreateSaleDto>>
        
    {
        private readonly ISaleRepository _saleRepository; 
        private readonly IBaseService _baseService;
        private readonly IMapper _mapper;
        public CreateSaleCommandHandler(
            ISaleRepository saleRepository,
            IMapper mapper,
            IBaseService baseService
        )
        {
            _saleRepository = saleRepository;
            _baseService = baseService;
            _mapper = mapper;
        }

        public async Task<Response<CreateSaleDto>> Handle(
            CreateSaleCommand request,
            CancellationToken cancellationToken
        )
        {
            Sale sale = _mapper.Map<Sale>(request);
            sale.SaleDetails = new List<SaleDetail>()
            {
                new SaleDetail()
                {
                    Note = request.Note,
                    CreatedBy = request.CreatedBy,
                    IsActive = true,
                    SaleStatusParameterId = (int)SaleStatusParameterEnums.NewSaleStatus
                }
            };
            
            sale.IsActive = true;
            await _saleRepository.AddAsync(sale,
                TableCreatedParameters.CreatedAtPropertyName,TableCreatedParameters.CreatedByPropertyName);
            return _baseService.CreateSuccessResult<CreateSaleDto>(null,
                InternalsConstants.Success);;
        }
    } 
        
}