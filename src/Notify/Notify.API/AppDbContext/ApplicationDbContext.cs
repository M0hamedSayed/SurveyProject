
using MassTransit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Notify.API.Models;

namespace Notify.API.AppDbContext
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Ignore<DomainEvent>();
            // MassTransit Outbox Configuration
            builder.AddInboxStateEntity();
            builder.AddOutboxMessageEntity();
            builder.AddOutboxStateEntity();
        }

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<NotifyMessage> NotifyMessages => Set<NotifyMessage>();
    }
}
