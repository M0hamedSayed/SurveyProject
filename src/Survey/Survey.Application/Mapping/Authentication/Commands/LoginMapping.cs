using Mapster;
using Survey.Application.Features.Authentication.Commands.Login;
using Survey.Domain.Models.Identity;

namespace Survey.Application.Mapping.Authentication
{
    public partial class AuthenticationRegister
    {
        public void LoginMapping(TypeAdapterConfig config)
        {
            config.NewConfig<ApplicationUser, LoginResult>()
                .Map(dest => dest.activeSession, src => src.RefreshTokens.FirstOrDefault().Adapt<ActiveSession>());

            config.NewConfig<UserRefreshTokens, ActiveSession>()
            .Map(dest => dest.AccessToken, src => src.AccessToken)
            .Map(dest => dest.UserAgent, src => src.UserMetaData.UserAgent)
            .Map(dest => dest.IP, src => src.UserMetaData.IP)
            .Map(dest => dest.Country, src => src.UserMetaData.Country)
            .Map(dest => dest.CountryCode, src => src.UserMetaData.CountryCode)
            .Map(dest => dest.City, src => src.UserMetaData.City)
            .Map(dest => dest.TimeZone, src => src.UserMetaData.TimeZone)
            .Map(dest => dest.LocationLat, src => src.UserMetaData.LocationLat)
            .Map(dest => dest.LocationLng, src => src.UserMetaData.LocationLng);
        }
    }
}
