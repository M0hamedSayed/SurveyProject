using Mapster;

namespace Survey.Application.Mapping.Authentication
{
    public partial class AuthenticationRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            LoginMapping(config);
        }
    }
}
