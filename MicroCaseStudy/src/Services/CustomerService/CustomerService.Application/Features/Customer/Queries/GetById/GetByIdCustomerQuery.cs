using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.Customer.Rules;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;

namespace CustomerService.Application.Features.Customer.Queries.GetById;

public class GetByIdCustomerQuery: IRequest<Response< GetByIdCustomerDto>>
{
    public int Id { get; set; }
     
    public class GetByIdICustomerQueryHandler : IRequestHandler<GetByIdCustomerQuery, Response<GetByIdCustomerDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly CustomerBusinessRules _customerBusinessRules;
        private readonly IBaseService _baseService; 

        public GetByIdICustomerQueryHandler(IMapper mapper, ICustomerRepository customerRepository,
            IBaseService baseService,CustomerBusinessRules customerBusinessRules)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
            _baseService = baseService; 
            _customerBusinessRules= customerBusinessRules;
        }

        public async Task<Response<GetByIdCustomerDto>> Handle(GetByIdCustomerQuery request,
            CancellationToken cancellationToken)
        {
            
            Domain.Entities.Customer? customer = await _customerRepository.GetAsync(predicate: b => b.Id == request.Id,
                cancellationToken: cancellationToken);
            await _customerBusinessRules.CustomerShouldExistWhenSelected(customer);
            
            GetByIdCustomerDto dto = _mapper.Map<GetByIdCustomerDto>(customer);
            
            return _baseService.CreateSuccessResult<GetByIdCustomerDto>(dto,
                InternalsConstants.Success);
        }
    }
    
    
    
    
}