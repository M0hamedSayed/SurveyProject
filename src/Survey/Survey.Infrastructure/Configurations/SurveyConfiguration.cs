using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Survey.Domain.Models.Identity;
using Survey.Domain.Models.Survey;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Infrastructure.Configurations
{
    public class SurveyConfiguration : IEntityTypeConfiguration<Surveys>
    {
        public void Configure(EntityTypeBuilder<Surveys> builder)
        {
            // key
            builder.HasKey(s => s.Id);
            builder.Property(s => s.RowVersion)
                .IsRowVersion() // For SQL Server
                .IsConcurrencyToken();
            // indexing
            builder.HasIndex(s => s.StartDate);
            builder.HasIndex(s => s.EndDate);

            builder.HasOne<ApplicationUser>(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .IsRequired();

            builder.HasOne<SurveyType>(s => s.SurveyType)
                .WithMany()
                .HasForeignKey(s => s.SurveyTypeId)
                .IsRequired();

            builder.HasMany<QuestionSurvey>( s => s.questions)
                .WithOne()
                .HasForeignKey(q => q.SurveyId).IsRequired();
        }
    }

    public class SurveyTypeConfiguration : IEntityTypeConfiguration<SurveyType>
    {
        public void Configure(EntityTypeBuilder<SurveyType> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasMany<Surveys>()
                .WithOne( s => s.SurveyType)
                .HasForeignKey(s => s.SurveyTypeId);
        }
    }
}
