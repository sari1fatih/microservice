using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using SaleService.Application.Features.Parameters.Queries.GetById;
using SaleService.Application.Features.Parameters.Queries.GetList;
using SaleService.Domain.Entities;

namespace SaleService.Application.Features.Parameters.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    { 
        CreateMap<Parameter, GetByIdParameterDto>()
            .ForMember(dest => dest.ParameterGroupId, opt => opt.MapFrom(src => src.ParameterGroup.Id))
            
            .ForMember(dest => dest.ParameterGroupValue, opt => opt.MapFrom(src => src.ParameterGroup.ParameterGroupValue))
            .ReverseMap();

        CreateMap<Parameter, GetListParameterDto>()
            .ForMember(dest => dest.ParameterGroupId, opt => opt.MapFrom(src => src.ParameterGroup.Id))
            
            .ForMember(dest => dest.ParameterGroupValue, opt => opt.MapFrom(src => src.ParameterGroup.ParameterGroupValue))
            .ReverseMap();
        
        CreateMap<Paginate<Parameter>, GetListResponse<GetListParameterDto>>().ReverseMap();
        
        
    }
}