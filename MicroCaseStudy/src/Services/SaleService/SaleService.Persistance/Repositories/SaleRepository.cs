using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Core.Application.Requests;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.Persistance.Repository;
using Core.WebAPI.Appsettings;
using Microsoft.EntityFrameworkCore;
using SaleService.Domain.Dtos.SaleDtos;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;
using SaleService.Persistance.Context;

namespace SaleService.Persistance.Repositories;

public class SaleRepository: EfRepositoryBase<Sale, SaleServiceDbContext,int>,
    ISaleRepository
{
   private readonly SaleServiceDbContext _context;
    public SaleRepository(SaleServiceDbContext context,IUserSession<int> userSession) : base(context,userSession)
    {
        _context = context;
    }
    public async Task<GetSaleViewDtos> GetView(int saleId)
    {
        var queryable = Query().AsNoTracking().Where(x => x.Id == saleId);
          
        var data = from sale in queryable
            select new GetSaleViewDtos
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                CustomerName = sale.CustomerName,
                CustomerSurname = sale.CustomerSurname,
                CustomerPhone = sale.CustomerPhone,
                CustomerEmail = sale.CustomerEmail,
                SaleName = sale.SaleName,
                SaleStatusParameterValue = (
                    from saleDetail in _context.SaleDetails
                    join parameter in _context.Parameters
                        on saleDetail.SaleStatusParameterId equals parameter.Id
                    where saleDetail.SaleId == sale.Id
                    orderby saleDetail.Id descending 
                    select parameter.ParameterValue
                ).FirstOrDefault() 
            };
        return await data.FirstOrDefaultAsync();
        
    }
    
    public async Task<Paginate<GetSaleViewDtos>> GetListView(DynamicQuery dynamicQuery,PageRequest pageRequest)
    {
        var queryable = Query().AsNoTracking().ToDynamic(dynamicQuery);
          
        var data = from sale in queryable
                select new GetSaleViewDtos
                {
                   Id = sale.Id,
                    CustomerId = sale.CustomerId,
                    CustomerName = sale.CustomerName,
                    CustomerSurname = sale.CustomerSurname,
                    CustomerPhone = sale.CustomerPhone,
                    CustomerEmail = sale.CustomerEmail,
                    SaleName = sale.SaleName,
                    SaleStatusParameterValue = (
                        from saleDetail in _context.SaleDetails
                        join parameter in _context.Parameters
                            on saleDetail.SaleStatusParameterId equals parameter.Id
                        where saleDetail.SaleId == sale.Id
                        orderby saleDetail.Id descending 
                        select parameter.ParameterValue
                    ).FirstOrDefault() 
                };
        return await data.ToPaginateAsync(pageRequest.PageIndex, pageRequest.PageSize,CancellationToken.None);
        
    }
    
    public async Task UpdateAllCustomerAsync(UpdateAllCustomer updateAllCustomer)
    {
        await Query()
            .AsNoTracking()
            .Where(r =>
                r.CustomerId == updateAllCustomer.CustomerId
            )
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.CustomerName, updateAllCustomer.CustomerName)
                .SetProperty(u => u.CustomerSurname, updateAllCustomer.CustomerSurname)
                .SetProperty(u => u.CustomerPhone, updateAllCustomer.CustomerPhone)
                .SetProperty(u => u.CustomerEmail, updateAllCustomer.CustomerEmail)
                .SetProperty(u => u.IsActive, true)
                .SetProperty(u => u.IsDeleted, false)
                .SetProperty(u => u.UpdatedAt, DateTime.UtcNow)
                .SetProperty(u => u.UpdatedBy, updateAllCustomer.UserId)
            );
 
    }
}
