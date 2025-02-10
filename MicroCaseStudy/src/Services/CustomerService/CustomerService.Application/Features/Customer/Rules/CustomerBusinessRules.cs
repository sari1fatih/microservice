using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using CustomerService.Application.Features.Customer.Constants;
using CustomerService.Persistance.Abstract.Repositories;

namespace CustomerService.Application.Features.Customer.Rules;

public class CustomerBusinessRules: BaseBusinessRules
{
    private readonly ICustomerRepository _customerepository; 
    
    public CustomerBusinessRules(
        ICustomerRepository customerepository
    )
    {
        _customerepository = customerepository;
    }

    
    private async Task throwBusinessException(string message)
    { 
        throw new BusinessException(message);
    }
    
    public async Task CustomerShouldExistWhenSelected(Domain.Entities.Customer? customer)
    {
        if (customer == null)
            await throwBusinessException(CustomerConstants.NotExists);
    }
    
    public async Task CustomerShouldExistWhenSelected(int customerId)
    {
        var isCustomer = await _customerepository.AnyAsync(predicate: u => u.Id == customerId); 
        if (!isCustomer)
            await throwBusinessException(CustomerConstants.NotExists);
    }
    
    public async Task CustomerEmailOrPhoneShouldNotExistsWhenUpdate(int id, string email,string phone)
    {
        var isCustomer = await _customerepository.GetAsync(predicate: u => u.Id != id && (u.Email == email|| u.Phone == phone));
        
        if (isCustomer?.Email == email)
            await throwBusinessException(CustomerConstants.CustomerMailAlreadyExists);
        
        if (isCustomer?.Phone == phone)
            await throwBusinessException(CustomerConstants.CustomerPhoneAlreadyExists);
    }
}