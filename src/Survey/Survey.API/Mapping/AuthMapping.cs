using Mapster;
using Survey.API.Dto;
using Survey.Application.Features.Authentication.Commands.Login;

namespace Survey.API.Mapping
{
    public class AuthMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<LoginDto, LoginCommand>()
                .RequireDestinationMemberSource(true);
        }
    }
}
