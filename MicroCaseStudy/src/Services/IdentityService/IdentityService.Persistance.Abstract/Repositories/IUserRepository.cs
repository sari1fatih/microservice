using Core.Persistance.Repository;
using IdentityService.Domain.Entities;

namespace IdentityService.Persistance.Abstract.Repositories;

public interface IUserRepository: IAsyncRepository<User>, IRepository<User>
{
    
}