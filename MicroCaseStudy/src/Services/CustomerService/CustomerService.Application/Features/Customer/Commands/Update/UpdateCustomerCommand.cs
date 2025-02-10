using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.Customer.Commands.Create;
using CustomerService.Application.Features.Customer.Rules;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;

namespace CustomerService.Application.Features.Customer.Commands.Update;

public class UpdateCustomerCommand: IRequest<Response<UpdatedCustomerDto>>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Surname { get; set; }
    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Company { get; set; } = null!;
    
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Response<UpdatedCustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper; 
        private readonly IBaseService _baseService;
        private readonly CustomerBusinessRules _customerBusinessRules;
        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper,IBaseService baseService,
            CustomerBusinessRules customerBusinessRules)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _baseService = baseService;
            _customerBusinessRules=customerBusinessRules;
            
        }

        public async Task<Response<UpdatedCustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Customer? user = await _customerRepository.GetAsync(
                predicate: u => u.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            
            await _customerBusinessRules.CustomerShouldExistWhenSelected(user);
            await _customerBusinessRules.CustomerEmailOrPhoneShouldNotExistsWhenUpdate(user.Id,request.Email, request.Phone);
            
            user = _mapper.Map(request, user);
            user.IsActive = true;
            await _customerRepository.UpdateAsync(user, TableUpdatedParameters.UpdatedAtPropertyName,
                TableUpdatedParameters.UpdatedByPropertyName);
            
            return _baseService.CreateSuccessResult<UpdatedCustomerDto>(null,
                InternalsConstants.Success);
            
        }
    }
    
}