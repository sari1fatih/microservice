using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.ParameterGroups.Queries.GetById;

public class GetByIdParameterGroupQuery: IRequest<Response<GetByIdParameterGroupDto>>
{
    public int Id { get; set; }
    
    public class GetByIdParameterGroupQueryHandler : IRequestHandler<GetByIdParameterGroupQuery, Response<GetByIdParameterGroupDto>>
    {
        private readonly IParameterGroupRepository _parameterGroupRepository;
        private readonly IMapper _mapper; 
        private readonly IBaseService _baseService;    
        public GetByIdParameterGroupQueryHandler(IParameterGroupRepository parameterGroupRepository, IMapper mapper, IBaseService baseService)
        {
            _parameterGroupRepository = parameterGroupRepository;
            _mapper = mapper;
            _baseService=baseService;
        }

        public async Task<Response<GetByIdParameterGroupDto>> Handle(GetByIdParameterGroupQuery request, CancellationToken cancellationToken)
        {
            ParameterGroup? user = await _parameterGroupRepository.GetAsync(
                predicate: b => b.Id==request.Id,
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            
            var dto = _mapper.Map<GetByIdParameterGroupDto>(user);
            return _baseService.CreateSuccessResult(dto,
                InternalsConstants.Success);
        }
    }
}