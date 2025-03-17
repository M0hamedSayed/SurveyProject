using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Survey.Domain.Models.Identity;
using Survey.Domain.ValueObjects.Identity;

namespace Survey.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(user => user.Id);

            builder.HasMany(user => user.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasPrincipalKey(user => user.Id)
                .HasForeignKey(rt => rt.UserId);
        }
    }
}
