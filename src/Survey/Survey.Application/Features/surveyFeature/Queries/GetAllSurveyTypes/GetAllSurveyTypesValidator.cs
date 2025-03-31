using FluentValidation;

namespace Survey.Application.Features.surveyFeature.Queries.GetAllSurveyTypes
{
    public class GetAllSurveyTypesValidator : AbstractValidator<GetAllSurveyTypesQuery>
    {
        public GetAllSurveyTypesValidator()
        {
            ApplyValidationsRules();
        }

        public void ApplyValidationsRules()
        {
            RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.Search)
                .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Search));
        }
    
    }
}
