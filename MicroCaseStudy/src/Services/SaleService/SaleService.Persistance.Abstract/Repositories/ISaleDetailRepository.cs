using Core.Persistance.Repository;
using SaleService.Domain.Entities;

namespace SaleService.Persistance.Abstract.Repositories;

public interface ISaleDetailRepository: IAsyncRepository<SaleDetail>, IRepository<SaleDetail>
{
    
}