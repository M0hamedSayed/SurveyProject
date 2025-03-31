using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Survey.Domain.Abstractions;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Domain.Models.Survey
{
    [Table("survey_types", Schema = "Survey")]
    public class SurveyType : Entity<Guid>
    {
        [Column("name_en")]
        [MaxLength(100)]
        [Required]
        public string NameEn { get; private set; }

        [Column("name_ar")]
        [MaxLength(100)]
        [Required]
        public string NameAr { get; private set; }

        private SurveyType() { }

        public static SurveyType Create(Guid id, string nameEn, string nameAr)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
            ArgumentException.ThrowIfNullOrWhiteSpace(nameAr);
            return new SurveyType
            {
                Id = id,
                NameEn = nameEn,
                NameAr = nameAr
            };
        }
    }
}
