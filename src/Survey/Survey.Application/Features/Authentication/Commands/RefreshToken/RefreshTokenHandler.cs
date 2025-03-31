using MediatR;
using Survey.Application.Base;
using Survey.Application.Interfaces;

namespace Survey.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenHandler(IAuthenticationService authenticationService) : ResponseHandler, IRequestHandler<RefreshTokenCommand, Response<RefreshTokenResult>>
    {
        public async Task<Response<RefreshTokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await authenticationService.RefreshTokenAsync();
            return Success<RefreshTokenResult>(new() { Token = token });
        }
    }
}
