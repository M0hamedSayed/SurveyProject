using System.Reflection;
using System.Reflection.Emit;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Shared.Events;
using Survey.Domain.Events.Dispatcher;
using Survey.Domain.Interfaces.Models;
using Survey.Domain.Models.Identity;
using Survey.Domain.Models.Survey;
using Survey.Infrastructure.Interceptors;

namespace Survey.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        //private readonly DispatchDomainEventInterceptor _domainEventInterceptor;
        //private readonly EntityChangesInterceptor _entityChangesInterceptor;
        public ApplicationDbContext
            (
            DbContextOptions<ApplicationDbContext> options
            //DispatchDomainEventInterceptor domainEventInterceptor,
            //EntityChangesInterceptor entityChangesInterceptor
            )
            : base(options) 
        {
            //_domainEventInterceptor = domainEventInterceptor;
            //_entityChangesInterceptor = entityChangesInterceptor;
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

            builder.Entity<SurveyDetails>(entity =>
            {
                entity.HasNoKey(); // Mark as keyless
                entity.ToView(null); // Not mapped to a database view
            });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.AddInterceptors(_domainEventInterceptor, _domainEventInterceptor);
        }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<ApplicationRole> applicationRoles { get; set; }
        public DbSet<UserRefreshTokens> userRefreshTokens { get; set; }

        public DbSet<Surveys> Surveys { get; set; }
        public DbSet<QuestionSurvey> QuestionSurveys { get; set; }
        public DbSet<ChoiceSurvey> Choices { get; set; }
        public DbSet<EvaluateChoiceSurvey> EvaluateChoices { get; set; }

        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<ResponseAnswer> ResponseAnswers { get; set; }
    }
}
