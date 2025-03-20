using MediatR;
using Survey.Application.Base;

namespace Survey.Application.Features.Authentication.Commands.RefreshToken
{
    public record RefreshTokenCommand : IRequest<Response<RefreshTokenResult>>
    {
    }
}
