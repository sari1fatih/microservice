using AutoMapper;
using Core.Application.Enums;
using Core.Security.Hashing;
using Core.WebAPI.Appsettings;
using IdentityService.Application.Features.Users.Rules;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Users.Commands.Update;

public class UpdateUserCommand : IRequest<UpdatedUserDto>
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

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdatedUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;
        private readonly IUserSession<int> _userSession;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper,
            UserBusinessRules userBusinessRules, IUserSession<int> userSession)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _userSession = userSession;
        }

        public async Task<UpdatedUserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
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

            UpdatedUserDto dto = _mapper.Map<UpdatedUserDto>(user);
            return dto;
        }
    }
}