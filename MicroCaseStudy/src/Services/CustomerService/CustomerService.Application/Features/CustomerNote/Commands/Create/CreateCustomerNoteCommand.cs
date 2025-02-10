using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.Customer.Rules;
using CustomerService.Application.Features.CustomerNote.Rules;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;

namespace CustomerService.Application.Features.CustomerNote.Commands.Create;

public class CreateCustomerNoteCommand: IRequest<Response<CreatedCustomerNoteDto>>
{
    public int CustomerId { get; set; }

    public string? Note { get; set; }
    
    public class CreateCustomerNoteCommandHandler : IRequestHandler<CreateCustomerNoteCommand, Response<CreatedCustomerNoteDto>>
    {
        private readonly ICustomerNoteRepository _customerNoteRepository;
        private readonly IMapper _mapper; 
        private readonly IBaseService _baseService;
        private readonly CustomerBusinessRules _customerBusinessRules;
        private readonly CustomerNoteBusinessRules _customerNoteBusinessRules;
        
        public CreateCustomerNoteCommandHandler(ICustomerNoteRepository customerNoteRepository, IMapper mapper,IBaseService baseService, CustomerBusinessRules customerBusinessRules,CustomerNoteBusinessRules customerNoteBusinessRules)
        {
            _customerNoteRepository = customerNoteRepository;
            _mapper = mapper;
            _baseService = baseService;
            _customerBusinessRules = customerBusinessRules;
            _customerNoteBusinessRules= customerNoteBusinessRules;
        }

        public async Task<Response<CreatedCustomerNoteDto>> Handle(CreateCustomerNoteCommand request, CancellationToken cancellationToken)
        {
            await _customerBusinessRules.CustomerShouldExistWhenSelected(request.CustomerId);
            
            Domain.Entities.CustomerNote mappedCustomerNote = _mapper.Map<Domain.Entities.CustomerNote>(request);
            mappedCustomerNote.IsActive = true;
            
            await _customerNoteRepository.AddAsync(mappedCustomerNote,
                TableCreatedParameters.CreatedAtPropertyName,TableCreatedParameters.CreatedByPropertyName);
            
            return _baseService.CreateSuccessResult<CreatedCustomerNoteDto>(null,
                InternalsConstants.Success);
            
        }
    }
}