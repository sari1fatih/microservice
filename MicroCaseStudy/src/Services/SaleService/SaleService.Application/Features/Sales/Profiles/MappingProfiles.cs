using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using SaleService.Application.Features.Sales.Commands.CreateSale;
using SaleService.Application.Features.Sales.Commands.UpdateSaleCustomer;
using SaleService.Application.Features.Sales.Queries.GetById;
using SaleService.Application.Features.Sales.Queries.GetList;
using SaleService.Domain.Dtos.SaleDtos;
using SaleService.Domain.Entities;

namespace SaleService.Application.Features.Sales.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<UpdateSaleCustomerCommand, UpdateAllCustomer>()
            .ReverseMap();
        
        
        CreateMap<Sale, CreateSaleCommand>()
            .ReverseMap();
        
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