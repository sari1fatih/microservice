using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using SaleService.Application.Features.SaleDetails.Commands.Create;
using SaleService.Application.Features.SaleDetails.Queries.GetById;
using SaleService.Application.Features.SaleDetails.Queries.GetList;
using SaleService.Domain.Entities;

namespace SaleService.Application.Features.SaleDetails.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<SaleDetail, CreateSaleDetailCommand>().ReverseMap();
        CreateMap<SaleDetail, GetByIdSaleDetailDto>()
            .ForMember(dest =>
                dest.SaleStatusParameterValue, opt => opt.MapFrom(src => src.SaleStatusParameter.ParameterValue))
            
            .ForMember(dest =>
                dest.SaleStatusParameterId, opt => opt.MapFrom(src => src.SaleStatusParameterId))
            .ReverseMap();
        
        CreateMap<SaleDetail, GetListSaleDetailDto>()
            .ForMember(
                dest => dest.SaleStatusParameterValue, 
                opt => opt.MapFrom(src => src.SaleStatusParameter.ParameterValue)
            )
            .ReverseMap();
        
        CreateMap<Paginate<SaleDetail>, GetListResponse<GetListSaleDetailDto>>().ReverseMap();
        
    }
}