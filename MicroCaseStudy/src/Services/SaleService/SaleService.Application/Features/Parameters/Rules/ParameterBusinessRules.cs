using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using SaleService.Application.Features.Parameters.Constants;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.Parameters.Rules;

public class ParameterBusinessRules: BaseBusinessRules
{
    private readonly IParameterRepository _parameterRepository; 

    public ParameterBusinessRules(
        IParameterRepository parameterRepository
    )
    {
        _parameterRepository = parameterRepository;
    }

    private Task throwBusinessException(string message)
    {
        throw new BusinessException(message);
    }
    
    public async Task ParameterIdShouldExistWhenSelected(int id)
    {
        bool doesExist = await _parameterRepository.AnyAsync(predicate: b => b.Id == id);
        if (!doesExist)
            await throwBusinessException(ParameterConstants.NotExists);
    }
}