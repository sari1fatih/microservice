using AutoMapper;
using Core.Application.Enums;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.CustomerNote.Rules;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;

namespace CustomerService.Application.Features.CustomerNote.Commands.Delete;

public class DeleteCustomerNoteCommand: IRequest<Response<DeletedCustomerNoteDto>>
{
    public int Id { get; set; }
    public class DeleteCustomerNoteCommandHandler : IRequestHandler<DeleteCustomerNoteCommand, Response<DeletedCustomerNoteDto>>
    {
        private readonly ICustomerNoteRepository _customerNoteRepository;
        private readonly IMapper _mapper; 
        private readonly IBaseService _baseService;
        private readonly CustomerNoteBusinessRules _customerNoteBusinessRules;
        
        public DeleteCustomerNoteCommandHandler(ICustomerNoteRepository customerNoteRepository, IMapper mapper,IBaseService baseService, CustomerNoteBusinessRules customerNoteBusinessRules)
        {
            _customerNoteRepository = customerNoteRepository;
            _mapper = mapper;
            _baseService = baseService;
            _customerNoteBusinessRules = customerNoteBusinessRules;
        }

        public async Task<Response<DeletedCustomerNoteDto>> Handle(DeleteCustomerNoteCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.CustomerNote? customer = await _customerNoteRepository.GetAsync(
                predicate: uoc => uoc.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _customerNoteBusinessRules.CustomerNoteShouldExistWhenSelected(customer);
            await _customerNoteRepository.DeleteAsync(customer!,TableDeletedParameters
                .DeletedAtPropertyName,TableDeletedParameters.DeletedByPropertyName,TableDeletedParameters.IsDeletedPropertyName);

            return _baseService.CreateSuccessResult<DeletedCustomerNoteDto>(null,
                InternalsConstants.Success);
        }
    }
}