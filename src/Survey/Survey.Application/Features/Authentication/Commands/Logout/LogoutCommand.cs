using MediatR;
using Survey.Application.Base;

namespace Survey.Application.Features.Authentication.Commands.Logout
{
    public record LogoutCommand : IRequest<Response<string>>
    {
    }
}
