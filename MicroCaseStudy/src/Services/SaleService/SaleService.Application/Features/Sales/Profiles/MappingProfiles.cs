using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using SaleService.Application.Features.Sales.Queries.GetById;
using SaleService.Application.Features.Sales.Queries.GetList;
using SaleService.Domain.Dtos.SaleDtos;
using SaleService.Domain.Entities;

namespace SaleService.Application.Features.Sales.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<GetSaleViewDtos, GetByIdSaleDto>()
            .ReverseMap();
        
        CreateMap<Sale, GetByIdSaleDto>()
            .ReverseMap();
        
        CreateMap<GetSaleViewDtos,GetListSaleDto>()
            .ReverseMap();
        CreateMap<Paginate<GetSaleViewDtos>, GetListResponse<GetListSaleDto>>().ReverseMap();
        
        CreateMap<Sale, GetListSaleDto>()
            .ReverseMap();
        CreateMap<Paginate<Sale>, GetListResponse<GetListSaleDto>>().ReverseMap();
    }
}