using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Notify.API.Interfaces;
using Notify.API.Services;
using Shared.Helpers;

namespace Notify.API
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Retrieve token settings using IOptions pattern
            var tokenSettings = configuration.GetSection("tokenSettings").Get<TokenSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = tokenSettings!.ValidateIssuer,
                    ValidIssuers = new[] { tokenSettings.Issuer },
                    ValidateIssuerSigningKey = tokenSettings.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSettings.Secret)),
                    ValidAudience = tokenSettings.Audience,
                    ValidateAudience = tokenSettings.ValidateAudience,
                    ValidateLifetime = tokenSettings.ValidateLifeTime,
                };
            });

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IClientService,ClientService>();
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}
