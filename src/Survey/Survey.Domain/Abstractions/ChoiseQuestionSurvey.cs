using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Survey.Domain.ValueObjects.Survey;

namespace Survey.Domain.Abstractions
{
    public abstract class ChoiseQuestionSurvey<T>
    {
        [Required]
        [Key]
        public T Id { get; protected set; }
        [Required]
        [Column("question_id")]
        public Guid QuestionSurveyId { get; private set; }
        [Required]
        [MaxLength(300)]
        [Column("text_en")]
        public string TextEn { get; protected set; }
        [Required]
        [MaxLength(300)]
        [Column("text_ar")]
        public string TextAr { get; protected set; }

        protected ChoiseQuestionSurvey() { }
        protected ChoiseQuestionSurvey(T id, Guid questionId, string textEn, string textAr)
        {
            Id = id;
            TextEn = textEn;
            TextAr = textAr;
            QuestionSurveyId = questionId;
        }

        public virtual void Update(string textEn, string textAr)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textAr);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(textEn);

            TextEn = textEn;
            TextAr = textAr;
        }

        public virtual void Update(string textEn, string textAr, string emoition)
        {
            Update(textEn, textAr);
        }
    }
}
