using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Survey.Domain.Models.Survey;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Infrastructure.Configurations
{
    public class ChoiceSurveyConfiguration : IEntityTypeConfiguration<ChoiceSurvey>
    {
        public void Configure(EntityTypeBuilder<ChoiceSurvey> builder)
        {
            builder.HasIndex(c => c.Id);
            builder.Property(c => c.Id).HasConversion(choiceId => choiceId.Value, dbId => ChoiseSurveyId.Of(dbId));
            builder.HasOne<QuestionSurvey>()
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionSurveyId).IsRequired();
        }
    }

    public class EvaluateChoiceSurveyConfiguration : IEntityTypeConfiguration<EvaluateChoiceSurvey>
    {
        public void Configure(EntityTypeBuilder<EvaluateChoiceSurvey> builder)
        {
            builder.HasIndex(c => c.Id);
            builder.Property(c => c.Id).HasConversion(choiceId => choiceId.Value, dbId => EvaluateChoiseSurveyId.Of(dbId));
            builder.HasOne<QuestionSurvey>()
                .WithMany(q => q.EvaluateChoices)
                .HasForeignKey(c => c.QuestionSurveyId);
        }
    }
}
