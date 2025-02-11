using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using SaleService.Application.Features.Parameters.Rules;
using SaleService.Application.Features.Sales.Rules;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.SaleDetails.Commands.Create;

public class CreateSaleDetailCommand: IRequest<Response<CreatedSaleDetailDto>>
{
    public int SaleId { get; set; }

    public int SaleStatusParameterId { get; set; }

    public string Note { get; set; }
  
    
    public class CreateRoleCommandHandler : IRequestHandler<CreateSaleDetailCommand,
        Response<CreatedSaleDetailDto>>
    {
        private readonly ISaleDetailRepository _saleDetailRepository;
        private readonly IMapper _mapper;
        private readonly SaleBusinessRules _saleBusinessRules;
        private readonly ParameterBusinessRules _parameterBusinessRules;
        private readonly IBaseService _baseService;

        public CreateRoleCommandHandler(
            ISaleDetailRepository saleDetailRepository,
            IMapper mapper,
            SaleBusinessRules saleBusinessRules,
            ParameterBusinessRules parameterBusinessRules,
            IBaseService baseService
        )
        {
            _saleDetailRepository = saleDetailRepository;
            _mapper = mapper;
            _saleBusinessRules = saleBusinessRules;
            _parameterBusinessRules= parameterBusinessRules;
            _baseService = baseService;
        }

        public async Task<Response< CreatedSaleDetailDto>> Handle(
            CreateSaleDetailCommand request,
            CancellationToken cancellationToken
        )
        {
            
            await _saleBusinessRules.SaleIdShouldExistWhenSelected(request.SaleId);
            await _parameterBusinessRules.ParameterIdShouldExistWhenSelected(request.SaleStatusParameterId);
            
            SaleDetail mappedRoleClaim = _mapper.Map<SaleDetail>(request);
            mappedRoleClaim.IsActive = true;
            
            await _saleDetailRepository.AddAsync(mappedRoleClaim,
                TableCreatedParameters.CreatedAtPropertyName,TableCreatedParameters.CreatedByPropertyName);
            return _baseService.CreateSuccessResult<CreatedSaleDetailDto>(null,
                InternalsConstants.Success);
        }
    } 
}