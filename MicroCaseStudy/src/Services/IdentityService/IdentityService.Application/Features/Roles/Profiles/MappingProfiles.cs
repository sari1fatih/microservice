using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using IdentityService.Application.Features.Roles.Commands.Create;
using IdentityService.Application.Features.Roles.Commands.Delete;
using IdentityService.Application.Features.Roles.Commands.Update;
using IdentityService.Application.Features.Roles.Queries.GetById;
using IdentityService.Application.Features.Roles.Queries.GetList;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Features.Roles.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Role, CreateRoleCommand>().ReverseMap();
        CreateMap<Role, CreatedRoleDto>().ReverseMap();
        CreateMap<Role, UpdateRoleCommand>().ReverseMap();
        CreateMap<Role, UpdatedRoleDto>().ReverseMap();
        CreateMap<Role, DeleteRoleCommand>().ReverseMap();
        CreateMap<Role, DeletedRoleDto>().ReverseMap();
        CreateMap<Role, GetByIdRoleDto>().ReverseMap();
        CreateMap<Role, GetListRoleDto>().ReverseMap();
        CreateMap<Paginate<Role>, GetListResponse<GetListRoleDto>>().ReverseMap();
    }
}