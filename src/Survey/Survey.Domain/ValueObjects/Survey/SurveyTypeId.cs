using Survey.Domain.Exceptions;

namespace Survey.Domain.ValueObjects.Survey
{
    public record SurveyTypeId
    {
        public Guid Value { get; }
        private SurveyTypeId(Guid value) => Value = value;

        public static SurveyTypeId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("SurveyTypeId cannot be empty.");
            }
            return new SurveyTypeId(value);
        }
    }
}
