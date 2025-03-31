using System;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Helpers;
using Shared.Messaging.Extensions;
using Survey.Domain.Events.Dispatcher;
using Survey.Domain.Interfaces;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Identity;
using Survey.Infrastructure.DatabaseContext;
using Survey.Infrastructure.Events;
using Survey.Infrastructure.Identity;
using Survey.Infrastructure.Interceptors;
using Survey.Infrastructure.Repositories;

namespace Survey.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // DI for interceptors
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<EntityChangesInterceptor>();
            services.AddScoped<DispatchDomainEventInterceptor>(sp =>
            {
                var bus = sp.GetRequiredService<IBus>();
                return new DispatchDomainEventInterceptor(bus);
            });
            // connect to sql server
            services.AddDbContext<ApplicationDbContext>((sp ,options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Database"));
                options.AddInterceptors
                (
                    sp.GetRequiredService<EntityChangesInterceptor>(),
                    sp.GetRequiredService<DispatchDomainEventInterceptor>()
                );
            });

            // configure mass transit
            services.addMassTransitConfiguration<ApplicationDbContext>(configuration);


            // Bind configuration settings using IOptions pattern
            services.Configure<TokenSettings>(configuration.GetSection("tokenSettings"));

            // Add Identity configuration
            services.AddIdentity<ApplicationUser, ApplicationRole>(IdentityOptionsConfig)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            // Add JWT Authentication
            services.AddJwtAuthentication(configuration);
            // DI
            // repositories and UOW
            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericrepositoryAsync<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<ISurveyTypeRepository, SurveyTypeRepository>();
            services.AddScoped<IQuestionSurveyRepository, SurveyQuestionRepository>();
            services.AddScoped<IChoiceRepository, ChoiceSurveynRepository>();
            services.AddScoped<IEvaluateChoiceRepository, EvaluateChoiceSurveynRepository>();
            services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // domain services
            services.AddScoped<IUserDomainService, UserDomainService>();

            

            return services;
        }

        private static void IdentityOptionsConfig(IdentityOptions options)
        {
            // User settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
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
    }
}
