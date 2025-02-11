using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using SaleService.Application.Features.ParameterGroups.Queries.GetById;
using SaleService.Application.Features.ParameterGroups.Queries.GetList;
using SaleService.Domain.Entities;

namespace SaleService.Application.Features.ParameterGroups.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        
        CreateMap<ParameterGroup, GetByIdParameterGroupDto>().ReverseMap();
        CreateMap<ParameterGroup, GetListParameterGroupDto>().ReverseMap();
        CreateMap<Paginate<ParameterGroup>, GetListResponse<GetListParameterGroupDto>>().ReverseMap();
        
    }
    
}