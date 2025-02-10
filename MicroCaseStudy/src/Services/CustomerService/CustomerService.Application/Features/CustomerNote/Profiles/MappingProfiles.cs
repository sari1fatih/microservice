using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using CustomerService.Application.Features.CustomerNote.Commands.Create;
using CustomerService.Application.Features.CustomerNote.Commands.Delete;
using CustomerService.Application.Features.CustomerNote.Commands.Update;
using CustomerService.Application.Features.CustomerNote.Queries.GetById;
using CustomerService.Application.Features.CustomerNote.Queries.GetList;

namespace CustomerService.Application.Features.CustomerNote.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    { 
        CreateMap<Domain.Entities.CustomerNote, CreateCustomerNoteCommand>().ReverseMap();
        CreateMap<Domain.Entities.CustomerNote, DeleteCustomerNoteCommand>().ReverseMap();
        CreateMap<Domain.Entities.CustomerNote, UpdateCustomerNoteCommand>().ReverseMap();
        
        
        
        
        CreateMap<Domain.Entities.CustomerNote, GetByIdCustomerNoteDto>()
            .ForMember(dest => dest.Email, 
                opt => 
                    opt.MapFrom(src => src.Customer.Email))
            
            .ForMember(dest => dest.Phone,
                opt => 
                    opt.MapFrom(src => src.Customer.Phone))
            
            .ForMember(dest => dest.CustomerId, 
                opt => 
                    opt.MapFrom(src => src.Customer.Id))
            
            .ForMember(dest => dest.CustomerName, 
                opt => 
                    opt.MapFrom(src => src.Customer.Name))
            
            .ForMember(dest => dest.CustomerSurname, 
                opt => 
                    opt.MapFrom(src => src.Customer.Surname))
            .ReverseMap();
        
        
        
        CreateMap<Domain.Entities.CustomerNote, GetListCustomerNoteDto>()
            .ForMember(dest => dest.Email, 
                opt => 
                    opt.MapFrom(src => src.Customer.Email))
            
            .ForMember(dest => dest.Phone,
                opt => 
                    opt.MapFrom(src => src.Customer.Phone))
            
            .ForMember(dest => dest.CustomerId, 
                opt => 
                    opt.MapFrom(src => src.Customer.Id))
            
            .ForMember(dest => dest.CustomerName, 
                opt => 
                    opt.MapFrom(src => src.Customer.Name))
            
            .ForMember(dest => dest.CustomerSurname, 
                opt => 
                    opt.MapFrom(src => src.Customer.Surname))
            
            .ReverseMap();
        CreateMap<Paginate<Domain.Entities.CustomerNote>, GetListResponse<GetListCustomerNoteDto>>().ReverseMap();
        
        
    }
} 