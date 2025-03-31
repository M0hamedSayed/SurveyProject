using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Survey.Application.Features.surveyFeature.Queries.GetSurveyUsers
{
    public class GetSurveyUsersValidators : AbstractValidator<GetSurveyUsersQuery>
    {
        public GetSurveyUsersValidators()
        {
            ValidateSurveyId();
        }

        public void ValidateSurveyId()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Must Not Empty")
                .NotNull().WithMessage("Is Required")
                .Must(g => g != Guid.Empty).WithMessage("Guid cannot be empty.");
            RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 1000).WithMessage("Page size must be between 1 and 100.");
        }
    }
}
