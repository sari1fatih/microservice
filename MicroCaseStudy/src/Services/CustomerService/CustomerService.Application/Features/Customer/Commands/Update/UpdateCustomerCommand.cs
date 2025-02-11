using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.Customer.Rules;
using CustomerService.Application.IntegrationEvents.Events;
using CustomerService.Persistance.Abstract.Repositories;
using EventBus.Base.Abstraction;
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
        private readonly IEventBus _eventBus;
        private readonly IUserSession<int> _userSession;
        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper,IBaseService baseService,
            CustomerBusinessRules customerBusinessRules,IUserSession<int> userSession, IEventBus eventBus)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _baseService = baseService;
            _customerBusinessRules=customerBusinessRules;
            _eventBus = eventBus;
            _userSession = userSession;
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
             
            var eventMessage = new CustomerUpdatedIntegrationEvent();
            eventMessage.CustomerId = user.Id;
            eventMessage.CustomerName = user.Name;
            eventMessage.CustomerSurname = user.Surname;
            eventMessage.CustomerPhone = user.Phone;
            eventMessage.CustomerEmail = user.Email; 
            eventMessage.UserId = _userSession.UserId; 
            _eventBus.Publish(eventMessage);
             
            return _baseService.CreateSuccessResult<UpdatedCustomerDto>(null,
                InternalsConstants.Success);
            
        }
    }
    
}