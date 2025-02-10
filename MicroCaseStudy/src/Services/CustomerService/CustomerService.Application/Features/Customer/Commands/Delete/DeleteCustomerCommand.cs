using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.Customer.Rules;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;

namespace CustomerService.Application.Features.Customer.Commands.Delete;

public class DeleteCustomerCommand: IRequest<Response<DeletedCustomerDto>>
{
    public int Id { get; set; }
    
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Response<DeletedCustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper; 
        private readonly IBaseService _baseService;
        private readonly CustomerBusinessRules _customerBusinessRules;
        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper,IBaseService baseService, CustomerBusinessRules customerBusinessRules)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _baseService = baseService;
            _customerBusinessRules = customerBusinessRules;
        }

        public async Task<Response<DeletedCustomerDto>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Customer? customer = await _customerRepository.GetAsync(
                predicate: uoc => uoc.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _customerBusinessRules.CustomerShouldExistWhenSelected(customer);
            await _customerRepository.DeleteAsync(customer!,TableDeletedParameters
                .DeletedAtPropertyName,TableDeletedParameters.DeletedByPropertyName,TableDeletedParameters.IsDeletedPropertyName);

            return _baseService.CreateSuccessResult<DeletedCustomerDto>(null,
                InternalsConstants.Success);

        }
    }
    
}