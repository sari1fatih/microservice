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
        CreateMap<UserRole, CreateUserRoleCommand>().ReverseMap();
        CreateMap<UserRole, CreatedUserRoleDto>().ReverseMap();
        CreateMap<UserRole, DeleteUserRoleCommand>().ReverseMap();
        CreateMap<UserRole, DeletedUserRoleDto>().ReverseMap();
        CreateMap<UserRole, GetByIdUserRoleDto>().ReverseMap();
        CreateMap<UserRole, GetListUserRoleDto>().ReverseMap();
        CreateMap<Paginate<UserRole>, GetListResponse<GetListUserRoleDto>>().ReverseMap();
    }
}
