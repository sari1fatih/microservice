using AutoMapper;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Users.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Users.Queries.GetById;

public class GetByIdUserQuery: IRequest<Response<GetByIdUserDto>>
{
    public int Id { get; set; }
    
    public class GetByIdUserQueryHandler : IRequestHandler<GetByIdUserQuery, Response<GetByIdUserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IBaseService _baseService;    
        public GetByIdUserQueryHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules,IBaseService baseService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _baseService=baseService;
        }

        public async Task<Response<GetByIdUserDto>> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetAsync(
                predicate: b => b.Id==request.Id,
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            await _userBusinessRules.UserShouldBeExistsWhenSelected(user);

            var dto = _mapper.Map<GetByIdUserDto>(user);
            
            return _baseService.CreateSuccessResult(dto,
                InternalsConstants.Success);
        }
    }
}
