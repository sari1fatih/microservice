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

 
        CreateMap<Paginate<GetListUserRoleDto>, GetListResponse<GetListUserRoleDto>>().ReverseMap();
    }
}
