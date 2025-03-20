using Microsoft.Extensions.Hosting;
using Serilog;

namespace Survey.Infrastructure.Extensions
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseSerilogLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .UseSerilog((context, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration); // Read settings from appsettings.json
            });
        }
    }
}
