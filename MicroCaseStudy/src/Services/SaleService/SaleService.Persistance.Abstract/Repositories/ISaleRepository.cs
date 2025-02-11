using System.Linq.Expressions;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.Persistance.Repository;
using SaleService.Domain.Dtos.SaleDtos;
using SaleService.Domain.Entities;

namespace SaleService.Persistance.Abstract.Repositories;

public interface ISaleRepository: IAsyncRepository<Sale>, IRepository<Sale>
{
    Task<GetSaleViewDtos> GetView(int saleId);
    Task<Paginate<GetSaleViewDtos>> GetListView(DynamicQuery dynamicQuery, PageRequest pageRequest);
    Task UpdateAllCustomerAsync(UpdateAllCustomer updateAllCustomer);
}