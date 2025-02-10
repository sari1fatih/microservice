using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using CustomerService.Application.Features.Customer.Commands.Create;
using CustomerService.Application.Features.Customer.Commands.Delete;
using CustomerService.Application.Features.Customer.Commands.Update;
using CustomerService.Application.Features.Customer.Queries.GetById;
using CustomerService.Application.Features.Customer.Queries.GetList;

namespace CustomerService.Application.Features.Customer.Profiles;

public class MappingProfiles: Profile
{
    
    public MappingProfiles()
    {
        CreateMap<Domain.Entities.Customer, GetByIdCustomerDto>().ReverseMap();
        CreateMap<Domain.Entities.Customer, CreateCustomerCommand>().ReverseMap();
        CreateMap<Domain.Entities.Customer, DeleteCustomerCommand>().ReverseMap();
        CreateMap<Domain.Entities.Customer, UpdateCustomerCommand>().ReverseMap();
        
        CreateMap<Domain.Entities.Customer, GetListCustomerDto>().ReverseMap();
        CreateMap<Paginate<Domain.Entities.Customer>, GetListResponse<GetListCustomerDto>>().ReverseMap();
    }
}