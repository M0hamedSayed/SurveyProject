using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Models.Identity;
using Survey.Domain.ValueObjects.Identity;

namespace Survey.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<ApplicationRole> applicationRoles { get; set; }
        public DbSet<UserRefreshTokens> userRefreshTokens { get; set; }

    }
}
