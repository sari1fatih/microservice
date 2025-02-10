using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.CustomerNote.Rules;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;

namespace CustomerService.Application.Features.CustomerNote.Commands.Update;

public class UpdateCustomerNoteCommand: IRequest<Response<UpdatedCustomerNoteDto>>
{
    public int Id { get; set; }
    public string? Note { get; set; }
    
    
    public class UpdateCustomerNoteCommandHandler : IRequestHandler<UpdateCustomerNoteCommand, Response<UpdatedCustomerNoteDto>>
    {
        private readonly ICustomerNoteRepository _customerNoteRepository;
        private readonly IMapper _mapper; 
        private readonly IBaseService _baseService; 
        private readonly CustomerNoteBusinessRules _customerNoteBusinessRules;
       
        public UpdateCustomerNoteCommandHandler(ICustomerNoteRepository customerNoteRepository, IMapper mapper,IBaseService baseService, CustomerNoteBusinessRules customerNoteBusinessRules)
        {
            _customerNoteRepository = customerNoteRepository;
            _mapper = mapper;
            _baseService = baseService; 
            _customerNoteBusinessRules= customerNoteBusinessRules;
        }

        public async Task<Response<UpdatedCustomerNoteDto>> Handle(UpdateCustomerNoteCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.CustomerNote? customerNote = await _customerNoteRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            
            await _customerNoteBusinessRules.CustomerNoteShouldExistWhenSelected(customerNote);
            Domain.Entities.CustomerNote mappedRole = _mapper.Map(request, destination: customerNote!);
          
            mappedRole.IsActive = true;
           
            await _customerNoteRepository.UpdateAsync(mappedRole,
                TableUpdatedParameters.UpdatedAtPropertyName,TableUpdatedParameters.UpdatedByPropertyName);
           
            return _baseService.CreateSuccessResult<UpdatedCustomerNoteDto>(null,
                InternalsConstants.Success);
           
        }
    }
}
 