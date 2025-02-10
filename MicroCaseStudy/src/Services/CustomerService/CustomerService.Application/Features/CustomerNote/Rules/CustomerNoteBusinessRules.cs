using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using CustomerService.Application.Features.Customer.Constants;
using CustomerService.Application.Features.CustomerNote.Constants;
using CustomerService.Persistance.Abstract.Repositories;

namespace CustomerService.Application.Features.CustomerNote.Rules;

public class CustomerNoteBusinessRules: BaseBusinessRules
{
    private readonly ICustomerNoteRepository _customerNoteepository; 
    
    public CustomerNoteBusinessRules(
        ICustomerNoteRepository customerNoteepository
    )
    {
        _customerNoteepository = customerNoteepository;
    }

    
    private async Task throwBusinessException(string message)
    { 
        throw new BusinessException(message);
    }
    
    public async Task CustomerNoteShouldExistWhenSelected(int customerNoteId)
    {
        var isCustomer = await _customerNoteepository.AnyAsync(predicate: u => u.Id == customerNoteId); 
        if (isCustomer)
            await throwBusinessException(CustomerConstants.NotExists);
    }
    
    public async Task CustomerNoteShouldExistWhenSelected(Domain.Entities.CustomerNote? customerNote)
    {
        if (customerNote == null)
            await throwBusinessException(CustomerNoteConstants.NotExists);
    } 
    
}