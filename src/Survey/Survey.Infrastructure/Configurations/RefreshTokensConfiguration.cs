using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Survey.Domain.Models.Identity;
using Survey.Domain.ValueObjects.Identity;

namespace Survey.Infrastructure.Configurations
{
    public class RefreshTokensConfiguration : IEntityTypeConfiguration<UserRefreshTokens>
    {
        public void Configure(EntityTypeBuilder<UserRefreshTokens> builder)
        {
            builder.HasKey(t => t.Id).HasName("rt_id");
            builder.Property(rt => rt.Id)
                .HasConversion(
                    rtId => rtId.Value,
                    dbId => RefreshTokenId.Of(dbId)
                );

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .IsRequired();

            builder.ComplexProperty(
                rt => rt.UserMetaData,
                metaBuilder =>
                {
                    metaBuilder.Property(m => m.CountryCode);
                    metaBuilder.Property(m => m.Country);
                    metaBuilder.Property(m => m.City);
                    metaBuilder.Property(m => m.UserAgent);
                    metaBuilder.Property(m => m.IP);
                    metaBuilder.Property(m => m.LocationLat);
                    metaBuilder.Property(m => m.LocationLng);
                    metaBuilder.Property(m => m.TimeZone);
                }

            );
        }
    }
}
