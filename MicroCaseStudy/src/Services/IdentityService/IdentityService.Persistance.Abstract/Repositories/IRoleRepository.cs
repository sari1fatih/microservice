 
using Core.Persistance.Repository;
using IdentityService.Domain.Entities;

namespace IdentityService.Persistance.Abstract.Repositories;

public interface IRoleRepository: IAsyncRepository<Role>, IRepository<Role>
{
    
}