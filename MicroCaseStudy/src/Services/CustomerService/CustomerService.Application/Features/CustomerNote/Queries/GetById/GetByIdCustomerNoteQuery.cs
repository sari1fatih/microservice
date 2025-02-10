using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Application.Features.CustomerNote.Rules;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Features.CustomerNote.Queries.GetById;

public class GetByIdCustomerNoteQuery: IRequest<Response< GetByIdCustomerNoteDto>>
{
    public int Id { get; set; }
    
    public class GetByIdCustomerNoteQueryHandler : IRequestHandler<GetByIdCustomerNoteQuery, Response<GetByIdCustomerNoteDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerNoteRepository _customerNoteRepository;
        private readonly IBaseService _baseService; 
        private readonly CustomerNoteBusinessRules _customerNoteBusinessRules;
        public GetByIdCustomerNoteQueryHandler(IMapper mapper, ICustomerNoteRepository customerNoteRepository,
            IBaseService baseService,CustomerNoteBusinessRules customerNoteBusinessRules)
        {
            _mapper = mapper;
            _customerNoteRepository = customerNoteRepository;
            _baseService = baseService; 
            _customerNoteBusinessRules= customerNoteBusinessRules;
        }

        public async Task<Response<GetByIdCustomerNoteDto>> Handle(GetByIdCustomerNoteQuery request,
            CancellationToken cancellationToken)
        {
                  
            Domain.Entities.CustomerNote? customerNote = await _customerNoteRepository.GetAsync(predicate: b => b.Id == request.Id,
                include: m => m.Include(b => b.Customer),   
                cancellationToken: cancellationToken);
            await _customerNoteBusinessRules.CustomerNoteShouldExistWhenSelected(customerNote);
            GetByIdCustomerNoteDto dto = _mapper.Map<GetByIdCustomerNoteDto>(customerNote);
                  
            return _baseService.CreateSuccessResult<GetByIdCustomerNoteDto>(dto,
                InternalsConstants.Success);
        }
    }  
}