using MapsterMapper;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Interfaces;
using Survey.Domain.Interfaces.Repositories;

namespace Survey.Application.Features.Authentication.Commands.Login
{
    public class LoginHandler (IAuthenticationService authenticationService, IUnitOfWork unitOfWork, IMapper mapper) : ResponseHandler, IRequestHandler<LoginCommand, Response<LoginResult>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        public async Task<Response<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // check if user exist
            var user = await _unitOfWork.UserRepository.UserManager.FindByEmailAsync(request.Email);

            if (user is null) return BadRequest<LoginResult>("Email or passwrod wrong");
            if (user.IsDeleted) return BadRequest<LoginResult>("Your account is deactivated please contact with support team");

            // check email confirmation
            if (!user.EmailConfirmed) return BadRequest<LoginResult>("Please confirm your email");
            // validate password
            var userSignIn = await _unitOfWork.UserRepository.SignInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!userSignIn.Succeeded) return BadRequest<LoginResult>("Email or passwrod wrong");
            // generate token and refresh token
            user = await _authenticationService.GenerateUserTokensAsync(user);

            var loginResult = _mapper.Map<LoginResult>(user);

            return Success<LoginResult>(loginResult);
        }
    }
}
