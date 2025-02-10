using AutoMapper;
using Core.Application.Enums;
using Core.Application.Responses;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Users.Queries.GetList;
using IdentityService.Application.Features.Users.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Users.Commands.Delete;

public class DeleteUserCommand: IRequest<Response<DeletedUserDto>>
{ 
    
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Response<DeletedUserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IUserSession<int> _userSession;
        private readonly IBaseService _baseService;    
        public DeleteUserCommandHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules,IUserSession<int> userSession,IBaseService baseService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _userSession = userSession;
            _baseService=baseService;
        }

        public async Task<Response<DeletedUserDto>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetAsync(
                predicate: u => u.Id.Equals(_userSession.UserId),
                cancellationToken: cancellationToken
            );
            await _userBusinessRules.UserShouldBeExistsWhenSelected(user);

            await _userRepository.DeleteAsync(user!,TableDeletedParameters.DeletedAtPropertyName,
                TableDeletedParameters.DeletedByPropertyName,TableDeletedParameters.IsDeletedPropertyName);

            DeletedUserDto dto = _mapper.Map<DeletedUserDto>(user);
            return _baseService.CreateSuccessResult<DeletedUserDto>(null,
                InternalsConstants.Success);
        }
    }
}
