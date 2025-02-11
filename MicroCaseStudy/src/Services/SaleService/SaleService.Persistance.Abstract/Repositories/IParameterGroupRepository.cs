using Core.Persistance.Repository;
using SaleService.Domain.Entities;

namespace SaleService.Persistance.Abstract.Repositories;

public interface IParameterGroupRepository: IAsyncRepository<ParameterGroup>, IRepository<ParameterGroup>
{
    
}