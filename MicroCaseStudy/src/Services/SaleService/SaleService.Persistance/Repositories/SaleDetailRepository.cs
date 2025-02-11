using Core.Persistance.Repository;
using Core.WebAPI.Appsettings;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;
using SaleService.Persistance.Context;

namespace SaleService.Persistance.Repositories;

public class SaleDetailRepository: EfRepositoryBase<SaleDetail, SaleServiceDbContext,int>,
    ISaleDetailRepository
{
    public SaleDetailRepository(SaleServiceDbContext context,IUserSession<int> userSession) : base(context,userSession)
    {
    }
}

