using Core.Persistance.Repository;
using Core.WebAPI.Appsettings;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using IdentityService.Persistance.Context;

namespace IdentityService.Persistance.Repositories;

public class UserRepository: EfRepositoryBase<User, IdentityServiceDbContext,int>, 
    IUserRepository
{
    public UserRepository(IdentityServiceDbContext context,IUserSession<int> userSession) : base(context,userSession)
    {
    }
}