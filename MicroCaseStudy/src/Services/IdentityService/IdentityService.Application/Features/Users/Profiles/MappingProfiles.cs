using AutoMapper;
using Core.Application.Responses;
using Core.Persistance.Paging;
using IdentityService.Application.Features.Users.Commands.Create;
using IdentityService.Application.Features.Users.Commands.Delete;
using IdentityService.Application.Features.Users.Commands.Update;
using IdentityService.Application.Features.Users.Queries.GetById;
using IdentityService.Application.Features.Users.Queries.GetList;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Features.Users.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<User, CreateUserCommand>().ReverseMap();
        CreateMap<User, CreatedUserDto>().ReverseMap();
        CreateMap<User, UpdateUserCommand>().ReverseMap();
        CreateMap<User, UpdatedUserDto>().ReverseMap();
        CreateMap<User, DeleteUserCommand>().ReverseMap();
        CreateMap<User, DeletedUserDto>().ReverseMap();
        CreateMap<User, GetByIdUserDto>().ReverseMap();
        CreateMap<User, GetListUserDto>().ReverseMap();
        CreateMap<Paginate<User>, GetListResponse<GetListUserDto>>().ReverseMap();
    }
}
