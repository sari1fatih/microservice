using Core.Persistance.Repository;
using CustomerService.Domain.Entities;

namespace CustomerService.Persistance.Abstract.Repositories;

public interface ICustomerRepository: IAsyncRepository<Customer>, IRepository<Customer>
{
    
}