using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Survey.Infrastructure.DatabaseContext;

namespace Survey.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // connect to sql server
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.AddInterceptors()
                .UseSqlServer(configuration.GetConnectionString("Database"));
            });
            return services;
        }
    }
}
