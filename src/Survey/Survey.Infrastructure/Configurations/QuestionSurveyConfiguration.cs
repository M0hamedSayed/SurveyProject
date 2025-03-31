using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Survey.Domain.Models.Survey;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Infrastructure.Configurations
{
    public class QuestionSurveyConfiguration : IEntityTypeConfiguration<QuestionSurvey>
    {
        public void Configure(EntityTypeBuilder<QuestionSurvey> builder)
        {
            // key
            builder.HasKey(q => q.Id);

            builder.HasMany<ChoiceSurvey>(q => q.Choices)
                .WithOne()
                .HasForeignKey(q => q.QuestionSurveyId).IsRequired();

            builder.HasOne<Surveys>()
                .WithMany(s => s.questions)
                .HasForeignKey(q => q.SurveyId).IsRequired();

            builder.HasMany<EvaluateChoiceSurvey>(q => q.EvaluateChoices)
                .WithOne()
                .HasForeignKey(q => q.QuestionSurveyId).IsRequired();

        }
    }
    }
