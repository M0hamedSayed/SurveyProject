using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Survey.Application.Features.surveyFeature.Queries.GetSurveyById
{
    public class GetSurveyByIdValidator : AbstractValidator<GetSurveyByIdQuery>
    {
        public GetSurveyByIdValidator() 
        {
            ValidateSurveyId();
        }

        public void ValidateSurveyId()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Must Not Empty")
                .NotNull().WithMessage("Is Required")
                .Must(g => g != Guid.Empty).WithMessage("Guid cannot be empty.");
        }
    }
}
