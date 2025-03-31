using Survey.Domain.Exceptions;

namespace Survey.Domain.ValueObjects.Survey
{
    public record SurveyId
    {
        public Guid Value { get; }
        private SurveyId(Guid value) => Value = value;

        public static SurveyId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("SurveyId cannot be empty.");
            }
            return new SurveyId(value);
        }
    }
}
