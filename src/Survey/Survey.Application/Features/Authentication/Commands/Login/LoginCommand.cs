using MediatR;
using Survey.Application.Base;

namespace Survey.Application.Features.Authentication.Commands.Login
{
    public record LoginCommand : IRequest<Response<LoginResult>>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
