using AutoMapper;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.Customer.Rules;
using CustomerService.Application.IntegrationEvents.Events;
using CustomerService.Persistance.Abstract.Repositories;
using EventBus.Base.Abstraction;
using MediatR;

namespace CustomerService.Application.Features.Customer.Commands.SaleCreated;

public class SaleCreatedCommand : IRequest<Response<SaleCreatedDto>>
{
    public int? CustomerId { get; set; }
    public string? SaleName { get; set; }
    public string? Note { get; set; }

    public class SaleCreatedCommandHandler : IRequestHandler<SaleCreatedCommand,
        Response<SaleCreatedDto>>

    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IUserSession<int> _userSession;
        private readonly CustomerBusinessRules _customerBusinessRules;
        private readonly IEventBus _eventBus;
        private readonly IBaseService _baseService;
        public SaleCreatedCommandHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            IUserSession<int> userSession,
            IEventBus eventBus,
            IBaseService baseService,
            CustomerBusinessRules customerBusinessRules
        )
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _userSession = userSession;
            _eventBus = eventBus;
            _baseService = baseService;
            _customerBusinessRules = customerBusinessRules;
        }

        public async Task<Response<SaleCreatedDto>> Handle(
            SaleCreatedCommand request,
            CancellationToken cancellationToken
        )
        {
            Domain.Entities.Customer? user = await _customerRepository.GetAsync(
                predicate: u => u.Id.Equals(request.CustomerId),
                cancellationToken: cancellationToken
            );

            await _customerBusinessRules.CustomerShouldExistWhenSelected(user);

            var eventMessage = new SaleCreatedIntegrationEvent();
            eventMessage.CustomerId = request.CustomerId;
            eventMessage.CustomerName = user.Name;
            eventMessage.CustomerSurname = user.Surname;
            eventMessage.CustomerPhone = user.Phone;
            eventMessage.CustomerEmail = user.Email;
            eventMessage.SaleName = request.SaleName;
            eventMessage.CreatedBy = _userSession.UserId;
            eventMessage.Note = request.Note;
            _eventBus.Publish(eventMessage);
             return _baseService.CreateSuccessResult<SaleCreatedDto>(null,
                InternalsConstants.Success);;
        }
    }
}