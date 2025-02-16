using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Manager.RefreshTokenManager;
using MediatR;

namespace IdentityService.Application.Features.Auths.Commands.RefreshToken;

public class SetActiveRefreshTokenToRedisCommand : IRequest<Response<SetActiveRefreshTokenToRedisCommandDto>>
{
    public class SetActiveRefreshTokenToRedisCommandHandler : IRequestHandler<SetActiveRefreshTokenToRedisCommand,
        Response<SetActiveRefreshTokenToRedisCommandDto>>
    {
        private readonly IBaseService _baseService;
        private readonly IRefreshTokenManager _refreshTokenManager;

        public SetActiveRefreshTokenToRedisCommandHandler(IBaseService baseService,IRefreshTokenManager refreshTokenManager)
        {
            _baseService = baseService;
            _refreshTokenManager= refreshTokenManager;
        }

        public async Task<Response<SetActiveRefreshTokenToRedisCommandDto>> Handle(
            SetActiveRefreshTokenToRedisCommand request, CancellationToken
                cancellationToken)
        {
            SetActiveRefreshTokenToRedisCommandDto refreshedTokensDto =
                new();
            await _refreshTokenManager.SetActiveRefreshTokenToRedis();
            return _baseService.CreateSuccessResult<SetActiveRefreshTokenToRedisCommandDto>(refreshedTokensDto,
                InternalsConstants.Success);
        }
    }
}