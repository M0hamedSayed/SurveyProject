using System.Reflection;
using Asp.Versioning;
using AspNetCoreRateLimit;
using Mapster;
using MapsterMapper;
using Survey.API.Middlewares;

namespace Survey.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // add rate limiting
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Period = "1m",  // Limit to 30 request per 1 minutes
                        Limit = 30
                    }
                };
            });
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Add API versioning
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true; // Include API versions in response headers
                options.ApiVersionReader = new UrlSegmentApiVersionReader(); //Reads version number from request url at "apiVersion" constraint
                options.AssumeDefaultVersionWhenUnspecified = true; // Assume default version if none specified
                options.DefaultApiVersion = new ApiVersion(1, 0); // Set default API version
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; //v1
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddCors(options => {

                options.AddDefaultPolicy(policyBuilder =>
                {
                    string[] defaultOrigins = ["*"];
                    policyBuilder
                    .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? defaultOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .AllowCredentials();
                });
            });

            return services;
        }
    }
}
