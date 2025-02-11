using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using SaleService.Application.Features.Sales.Constants;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.Sales.Rules;

public class SaleBusinessRules: BaseBusinessRules
{
    private readonly ISaleRepository _saleRepository; 

    public SaleBusinessRules(
        ISaleRepository saleRepository
    )
    {
        _saleRepository = saleRepository;
    }

    private Task throwBusinessException(string message)
    {
        throw new BusinessException(message);
    }
    
    public async Task SaleIdShouldExistWhenSelected(int id)
    {
        bool doesExist = await _saleRepository.AnyAsync(predicate: b => b.Id == id);
        if (!doesExist)
            await throwBusinessException(SaleConstants.NotExists);
    }
}