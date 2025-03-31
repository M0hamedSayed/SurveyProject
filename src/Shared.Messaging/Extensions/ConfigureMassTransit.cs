using System.Reflection;
using MassTransit;
using MassTransit.Middleware.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messaging.Extensions
{
    public static class ConfigureMassTransit
    {
        public static IServiceCollection addMassTransitConfiguration<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? assembly = null,
            Action<IRabbitMqBusFactoryConfigurator>? configureRabbitMq = null
            ) where TDbContext : DbContext
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                    config.AddConsumers(assembly);

                config.AddEntityFrameworkOutbox<TDbContext>(o =>
                {
                    o.QueryMessageLimit = 500;
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]!);
                        host.Password(configuration["MessageBroker:Password"]!);
                    });
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                    cfg.PrefetchCount = 500; // messages per consumer
                    cfg.ConcurrentMessageLimit = 50; // parallel handle message per consumer
                    // Default endpoint configuration
                    cfg.ConfigureEndpoints(context);
                    // Additional custom configuration
                    configureRabbitMq?.Invoke(cfg);
                });

            });
            return services;
        }
    }
}

/*
 builder.Services.AddSharedMassTransit<AppDbContext>(
    rabbitMqHost: builder.Configuration["RabbitMQ:Host"],
    configureRabbitMq: cfg => 
    {
        // Service-specific RabbitMQ configuration
        cfg.PrefetchCount = 10;
        cfg.Message<NotificationCreatedEvent>(m => m.SetEntityName("notifications"));
    }
);
cfg.ReceiveEndpoint(queueName, e =>
                    {
                        e.ConfigureConsumer<TConsumer>(context);

                        e.PrefetchCount = 10; // Optimize batch processing
                        e.ConcurrentMessageLimit = 10; // Parallel execution

                        // Enable the Outbox to ensure messages are stored before publishing
                        e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10))); // Retry on failure
                    });
 */
