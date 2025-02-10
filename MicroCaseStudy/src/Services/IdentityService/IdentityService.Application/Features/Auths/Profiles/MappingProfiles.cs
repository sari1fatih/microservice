using AutoMapper;
using Core.Security.JWT.Dtos;

namespace IdentityService.Application.Features.Auths.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<RefreshToken<int, int>, Domain.Entities.RefreshToken>().ReverseMap();
    }
}