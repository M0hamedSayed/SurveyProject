using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Events;
using Survey.Domain.Abstractions;
using Survey.Domain.Enums;
using Survey.Domain.Exceptions;
using Survey.Domain.Models.Identity;
using Survey.Domain.ValueObjects.Identity;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Domain.Models.Survey
{
    [Table("surveys", Schema = "Survey")]
    public class Surveys : Aggregate<Guid>
    {
        public byte[] RowVersion { get; private set; } // Concurrency token
        [Required]
        [Column("survey_type_id")]
        public Guid SurveyTypeId { get; private set; }
        [Required]
        [Column("user_id")]
        public Guid UserId { get; private set; }
        [Required]
        [MaxLength(100)]
        [Column("name_en")]
        public string NameEn { get; private set; }
        [Required]
        [MaxLength(100)]
        [Column("name_ar")]
        public string NameAr { get; private set; }
        [MaxLength(500)]
        [Column("description_en")]
        public string? DescriptionEn { get; private set; }
        [MaxLength(500)]
        [Column("description_ar")]
        public string? DescriptionAr { get; private set; }
        [MaxLength(100)]
        [Column("closing_address_en")]
        public string? ClosingAddressEn { get; private set; }
        [MaxLength(100)]
        [Column("closing_address_ar")]
        public string? ClosingAddressAr { get; private set; }
        [MaxLength(500)]
        [Column("closing_statement_en")]
        public string? ClosingStatementEn { get; private set; }
        [MaxLength(500)]
        [Column("closing_statement_ar")]
        public string? ClosingStatementAr { get; private set; }
        [Column("start_date")]
        public DateTime StartDate { get; private set; }
        [Column("end_date")]
        public DateTime EndDate { get; private set; }
        [Column("timezone")]
        [MaxLength(100)]
        public string Timezone { get; private set; }
        [MaxLength(500)]
        [Column("image_url")]
        public string? ImageUrl { get; private set; }
        [Column("is_required")]
        public bool IsRequired { get; private set; }
        [Column("is_active")]
        public bool IsActive { get; private set; }
        [Column("reminder_sent")]
        public bool ReminderSent { get; private set; }

        private readonly List<QuestionSurvey> _questions = new();
        public IReadOnlyCollection<QuestionSurvey> questions => _questions.AsReadOnly();
        public virtual SurveyType SurveyType { get; private set; }
        public virtual ApplicationUser User { get; private set; }

        private readonly List<SurveyResponse> _surveyResponses = new();
        public IReadOnlyCollection<SurveyResponse> surveyResponses => _surveyResponses.AsReadOnly();

        private Surveys() { }

        public static Surveys Create(
            Guid id,
            Guid userId,
            Guid typeId,
            string nameEn,
            string nameAr,
            string? descriptionEn,
            string? descriptionAr,
            string? closingAddresAr,
            string? closingAddressEn,
            string? closingStatementAr,
            string? closingStatementEn,
            DateTime startDate,
            DateTime endDate,
            string? timezone,
            string? imageUrl = null,
            bool? isRequired = false
            )
        {
            // validate survey
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(startDate);
            ArgumentNullException.ThrowIfNull(endDate);
            ArgumentNullException.ThrowIfNull(userId);
            ArgumentNullException.ThrowIfNull(typeId);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameEn);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameAr);
            if (startDate >= endDate)
            {
                throw new DomainException("Invalid Date");
            }
            var survey = new Surveys
            {
                Id = id,
                UserId = userId,
                ClosingAddressAr = closingAddressEn,
                ClosingStatementAr = closingStatementAr,
                ClosingAddressEn = closingAddressEn,
                ClosingStatementEn = closingStatementEn,
                StartDate = startDate,
                EndDate = endDate,
                ImageUrl = imageUrl,
                DescriptionAr = descriptionAr,
                DescriptionEn = descriptionEn,
                IsRequired = isRequired ?? false,
                SurveyTypeId = typeId,
                NameEn = nameEn,
                NameAr = nameAr,
                IsActive = false,
                Timezone = timezone ?? "GMT",
                ReminderSent = false,
            };
            return survey;
        }

        public void Update( 
            Guid typeId,
            string nameEn,
            string nameAr,
            string? descriptionEn,
            string? descriptionAr,
            string? closingAddresAr,
            string? closingAddressEn,
            string? closingStatementAr,
            string? closingStatementEn,
            DateTime startDate,
            DateTime endDate,
            string? timezone,
            string? imageUrl = null,
            bool? isRequired = false
            )
        {
            // validate survey
            ArgumentNullException.ThrowIfNull(typeId);
            ArgumentNullException.ThrowIfNull(startDate);
            ArgumentNullException.ThrowIfNull(endDate);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameEn);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameAr);
            if (startDate >= endDate)
            {
                throw new DomainException("Invalid Date");
            }

            ClosingAddressAr = closingAddressEn;
            ClosingStatementAr = closingStatementAr;
            ClosingAddressEn = closingAddressEn;
            ClosingStatementEn = closingStatementEn;
            StartDate = startDate;
            EndDate = endDate;
            ImageUrl = imageUrl;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            IsRequired = isRequired ?? false;
            SurveyTypeId = typeId;
            NameEn = nameEn;
            NameAr = nameAr;
            Timezone = timezone ?? "GMT";
        }


        public void Activate()
        {
            IsActive = true;
            // add event to send mails to users
            AddDomainEvent(new SurveyActivatedEvent(Id, NameEn, NameAr, UserId));
        }

        public void SuveyReminder()
        {
            ReminderSent = true;
            AddDomainEvent(new SurveyReminderEvent(Id, NameEn, NameAr, UserId));
        }

        public void AddQuestion(QuestionSurvey questionSurvey)
        {
            _questions.Add(questionSurvey);
        }

        public QuestionSurvey AddNewQuestions(Guid id, string questionEn, string questionAr, QuestionType? questionType)
        {
            
            var question =  QuestionSurvey.Create(id,Id, questionEn, questionAr, questionType);
            _questions.Add(question);
            return question;
        }

        public QuestionSurvey UpdateQuestion(Guid id, string questionEn, string questionAr, QuestionType? questionType)
        {
            var question = ValidateQuestion(id);
            question = question.Update(questionEn, questionAr, questionType);
            return question;
        }

        public void RemoveQuestion(Guid id)
        {
            var question = ValidateQuestion(id);
            _questions.Remove(question);
        }

        public void RemoveMultiAuestions(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                var question = ValidateQuestion(id);
                _questions.Remove(question);
            }
        }

        private QuestionSurvey ValidateQuestion(Guid id)
        {
            var question = _questions.FirstOrDefault(q => q.Id == id);
            if (question is null)
            {
                throw new DomainException("Question not found");
            }
            return question;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

    }
}
