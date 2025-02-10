using Core.Persistance.Repository;
using Core.WebAPI.Appsettings;
using CustomerService.Domain.Entities;
using CustomerService.Persistance.Abstract.Repositories;
using CustomerService.Persistance.Context;

namespace CustomerService.Persistance.Repositories;

public class CustomerNoteRepository : EfRepositoryBase<CustomerNote, CustomerServiceDbContext, int>,
    ICustomerNoteRepository
{
    public CustomerNoteRepository(CustomerServiceDbContext context, IUserSession<int> userSession) : base(context,
        userSession)
    {
    }
}
 