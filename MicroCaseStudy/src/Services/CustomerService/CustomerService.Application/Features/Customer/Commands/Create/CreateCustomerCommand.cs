using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.Customer.Rules;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;

namespace CustomerService.Application.Features.Customer.Commands.Create;

public class CreateCustomerCommand : IRequest<Response<CreatedCustomerDto>>
{
    public string Name { get; set; }

    public string? Surname { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string Company { get; set; }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Response<CreatedCustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService;
        private readonly CustomerBusinessRules _customerBusinessRules;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper,
            IBaseService baseService, CustomerBusinessRules customerBusinessRules)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _baseService = baseService;
            _customerBusinessRules = customerBusinessRules;
        }

        public async Task<Response<CreatedCustomerDto>> Handle(CreateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            await _customerBusinessRules.CustomerEmailOrPhoneShouldNotExistsWhenUpdate(0, request.Email, request.Phone);

            Domain.Entities.Customer mappedCustomerClaim = _mapper.Map<Domain.Entities.Customer>(request);
            mappedCustomerClaim.IsActive = true;

            await _customerRepository.AddAsync(mappedCustomerClaim,
                TableCreatedParameters.CreatedAtPropertyName, TableCreatedParameters.CreatedByPropertyName);

            return _baseService.CreateSuccessResult<CreatedCustomerDto>(null,
                InternalsConstants.Success);
        }
    }
}