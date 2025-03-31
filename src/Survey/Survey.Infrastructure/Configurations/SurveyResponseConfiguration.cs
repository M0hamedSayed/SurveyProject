using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Survey.Domain.Models.Identity;
using Survey.Domain.Models.Survey;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Infrastructure.Configurations
{
    public class SurveyResponseConfiguration : IEntityTypeConfiguration<SurveyResponse>
    {
        public void Configure(EntityTypeBuilder<SurveyResponse> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne<ApplicationUser>(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            builder.HasOne<Surveys>()
                .WithMany(s => s.surveyResponses)
                .HasForeignKey(r => r.SurveyId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany<ResponseAnswer>( r => r.Answers)
                .WithOne()
                .HasForeignKey(a => a.ResponseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class ResponseAnswerConfiguration : IEntityTypeConfiguration<ResponseAnswer>
    {
        public void Configure(EntityTypeBuilder<ResponseAnswer> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.ChoiceId).HasConversion<Guid?>(c =>  c != null ? c.Value : null, dbId => !dbId.HasValue || dbId == Guid.Empty ? null : ChoiseSurveyId.Of((Guid) dbId));
            builder.Property(r => r.Evaluation)
                .HasConversion<Guid?>(
                    c => c != null ? c.Value : null,  // Simply return null if `c` is null
                    dbId => dbId.HasValue ? EvaluateChoiseSurveyId.Of(dbId.Value) : null // Handle null safely
                );
            builder.HasOne<QuestionSurvey>()
                .WithMany()
                .HasForeignKey(r => r.QuestionId);
            builder.HasOne<SurveyResponse>()
                .WithMany(r => r.Answers)
                .HasForeignKey(r => r.ResponseId);

            builder.HasOne<ChoiceSurvey>()
                .WithMany()
                .HasForeignKey(r => r.ChoiceId);

            builder.Property(s => s.MultipleChoices)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    v => JsonSerializer.Deserialize<List<ChoiseSurveyId>>(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new List<ChoiseSurveyId>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(new ValueComparer<List<ChoiseSurveyId>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2), // Check if both lists contain the same elements
                    c => c.Aggregate(0, (hash, id) => HashCode.Combine(hash, id.Value.GetHashCode())), // Hash code generation
                    c => c.Select(id => ChoiseSurveyId.Of(id.Value)).ToList() // Deep copy
                ));

            builder.HasOne<EvaluateChoiceSurvey>()
                .WithMany()
                .HasForeignKey(a => a.Evaluation);
        }
    }
}
