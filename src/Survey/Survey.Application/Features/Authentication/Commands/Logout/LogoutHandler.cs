using MediatR;
using Survey.Application.Base;
using Survey.Application.Interfaces;

namespace Survey.Application.Features.Authentication.Commands.Logout
{
    public class LogoutHandler(IAuthenticationService authenticationService) : ResponseHandler, IRequestHandler<LogoutCommand, Response<string>>
    {
        public async Task<Response<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            bool isLoggedOut = await authenticationService.Logout();
            if (!isLoggedOut) return BadRequest<string>("You already Loggedout");
            return SuccessWithoutData<string>();
        }
    }
}
