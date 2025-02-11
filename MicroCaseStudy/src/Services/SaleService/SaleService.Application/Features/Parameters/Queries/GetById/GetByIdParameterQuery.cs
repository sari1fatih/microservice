using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleService.Domain.Entities;
using SaleService.Persistance.Abstract.Repositories;

namespace SaleService.Application.Features.Parameters.Queries.GetById;

public class GetByIdParameterQuery: IRequest<Response<GetByIdParameterDto>>
{
    public int Id { get; set; }
    
    public class GetByIdParameterQueryHandler : IRequestHandler<GetByIdParameterQuery, Response<GetByIdParameterDto>>
    {
        private readonly IParameterRepository _parameterGroupRepository;
        private readonly IMapper _mapper; 
        private readonly IBaseService _baseService;    
        public GetByIdParameterQueryHandler(IParameterRepository parameterGroupRepository, IMapper mapper, IBaseService baseService)
        {
            _parameterGroupRepository = parameterGroupRepository;
            _mapper = mapper;
            _baseService=baseService;
        }

        public async Task<Response<GetByIdParameterDto>> Handle(GetByIdParameterQuery request, CancellationToken cancellationToken)
        {
            Parameter? user = await _parameterGroupRepository.GetAsync(
                predicate: b =>  b.Id.Equals(request.Id),
                include: m => m.Include(b => b.ParameterGroup),
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            
            var dto = _mapper.Map<GetByIdParameterDto>(user);
            return _baseService.CreateSuccessResult(dto,
                InternalsConstants.Success);
        }
    }
    
}