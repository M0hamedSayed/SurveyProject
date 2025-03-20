using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey.API.Base;
using Survey.API.Dto;
using Survey.Application.Features.Authentication.Commands.Login;
using Survey.Application.Features.Authentication.Commands.Logout;
using Survey.Application.Features.Authentication.Commands.RefreshToken;

namespace Survey.API.Controllers
{
    [AllowAnonymous]
    public class AuthController (IMediator mediator, IMapper mapper) : AppControllerBase (mediator)
    {
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginDto loginDto)
        {
            var command = mapper.Map<LoginCommand>(loginDto);
            var res = await _mediator.Send(command);
            return HandleResult(res);
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var res = await _mediator.Send(new RefreshTokenCommand());
            return HandleResult(res);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var res = await _mediator.Send(new LogoutCommand());
            return HandleResult(res);
        }
    }
}
