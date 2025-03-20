using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Survey.Application.Behaviors;
using Survey.Application.Features.Authentication.Commands.Login;
using Survey.Application.Interfaces;
using Survey.Application.Services;

namespace Survey.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            // add mediators
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            // Get Validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // mapping
            var config = TypeAdapterConfig.GlobalSettings;
            // Scan all loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                config.Scan(assembly);
            }

            services.AddSingleton(config);

            services.AddScoped<IMapper, ServiceMapper>();

            // Validate mappings (Ensures all DTOs and Entities are properly mapped)
            config.Compile();

            // DI
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            return services;
        }

    }
}
