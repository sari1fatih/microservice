 
using System.Text.Json.Serialization;
using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.Redis.MediaR;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Users.Queries.GetList;

public class GetListUserQuery : IRequest<Response<GetListResponse<GetListUserDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; } 
    public class GetListUserQueryHandler : IRequestHandler<GetListUserQuery, Response<GetListResponse<GetListUserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService;

        public GetListUserQueryHandler(IUserRepository userRepository, IMapper mapper, IBaseService baseService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListUserDto>>> Handle(
            GetListUserQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<User> users = await _userRepository.GetListByDynamicAsync(
                request.DynamicQuery,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize ,
                enableTracking: false
            );

            GetListResponse<GetListUserDto> response = _mapper.Map<GetListResponse<GetListUserDto>>(users);
           
            return _baseService.CreateSuccessResult<GetListResponse<GetListUserDto>>(response,
             InternalsConstants.Success);
        }
    }
 
}