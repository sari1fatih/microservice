using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using IdentityService.Application.Features.UserRoles.Commands.Create;
using IdentityService.Application.Features.UserRoles.Commands.Delete;
using IdentityService.Application.Features.UserRoles.Queries.GetById;
using IdentityService.Application.Features.UserRoles.Queries.GetList;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Features.UserRoles.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<UserRole, CreateUserRoleCommand>()
            .ForMember(dest => dest.ReqUserId, opt => opt.MapFrom(src => src.UserId))
            .ReverseMap();
        CreateMap<UserRole, CreatedUserRoleDto>().ReverseMap();
        CreateMap<UserRole, DeleteUserRoleCommand>().ReverseMap();
        CreateMap<UserRole, DeletedUserRoleDto>().ReverseMap();


        CreateMap<UserRole, GetByIdUserRoleDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
            .ForMember(dest => dest.RoleValue, opt => opt.MapFrom(src =>  src.Role.RoleValue))
            .ReverseMap();
        
        
        
        CreateMap<UserRole, GetListUserRoleDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname)) 
            .ForMember(dest => dest.RoleValue, opt => opt.MapFrom(src => src.Role.RoleValue ))
            
            .ReverseMap();
        CreateMap<Paginate<UserRole>, GetListResponse<GetListUserRoleDto>>().ReverseMap();
    }
}
