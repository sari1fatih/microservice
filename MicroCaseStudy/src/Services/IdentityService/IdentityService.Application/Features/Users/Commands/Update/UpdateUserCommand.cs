using AutoMapper;
using Core.Application.Enums;
using Core.Security.Hashing;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Users.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Users.Commands.Update;

public class UpdateUserCommand : IRequest<Response<UpdatedUserDto>>
{
    public string Name { get; set; }
    public string Surname { get; set; }

    public UpdateUserCommand()
    {
        Name = string.Empty;
        Surname = string.Empty;
    }

    public UpdateUserCommand(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Response<UpdatedUserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IUserSession<int> _userSession;
        private readonly IBaseService _baseService;    
        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper,
            UserBusinessRules userBusinessRules, IUserSession<int> userSession,IBaseService baseService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _userSession = userSession;
            _baseService = baseService;
        }

        public async Task<Response<UpdatedUserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetAsync(
                predicate: u => u.Id.Equals(_userSession.UserId),
                cancellationToken: cancellationToken
            );
            await _userBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(user!.Id, user.Email);
            user = _mapper.Map(request, user);

            await _userRepository.UpdateAsync(user, TableUpdatedParameters.UpdatedAtPropertyName,
                TableUpdatedParameters.UpdatedByPropertyName);
 
            return _baseService.CreateSuccessResult<UpdatedUserDto>(null,
                InternalsConstants.Success);
        }
    }
}