using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Application.Interfaces;
using Survey.Domain.Enums;
using Survey.Domain.Interfaces.Repositories;

namespace Survey.Application.Features.surveyFeature.Commands.UpdateSurvey
{
    public class UpdateSurvyVaidator : AbstractValidator<UpdateSurveyCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateSurvyVaidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ApplyValidationRules();
        }

        public void ApplyValidationRules()
        {
            RuleFor(x => x.Id)
                .Must(g => g == null || g != Guid.Empty).WithMessage("SurveyId must be a valid GUID.");
            RuleFor(x => x.SurveyTypeId)
            .NotEmpty().WithMessage("SurveyTypeId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("SurveyTypeId must be a valid GUID.")
            .MustAsync(SurveyTypeExists).WithMessage("SurveyTypeId does not exist.");

            RuleFor(x => x.NameEn).NotEmpty().WithMessage("Survey name (English) is required.")
                .MaximumLength(100).WithMessage("The Max Length is 100");
            RuleFor(x => x.NameAr).NotEmpty().WithMessage("Survey name (Arabic) is required.")
                .MaximumLength(100).WithMessage("The Max Length is 100");
            RuleFor(x => x.DescriptionEn)
                .MaximumLength(500).WithMessage("The max length is 500");
            RuleFor(x => x.DescriptionAr)
                .MaximumLength(500).WithMessage("The max length is 500");
            RuleFor(x => x.ClosingAddressAr)
                .MaximumLength(100).WithMessage("The max length is 100");
            RuleFor(x => x.ClosingAddressEn)
                .MaximumLength(100).WithMessage("The max length is 100");
            RuleFor(x => x.ClosingStatementEn)
                .MaximumLength(500).WithMessage("The max length is 500");
            RuleFor(x => x.ClosingStatementAr)
                .MaximumLength(500).WithMessage("The max length is 500");
            RuleFor(x => x.Timezone)
                .MaximumLength(100).WithMessage("The max length is 100");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("Start date must be before end date.");

            RuleFor(x => x.Questions)
                .NotEmpty().WithMessage("At least one question is required.");

            RuleForEach(x => x.Questions).SetValidator(new QuestionSurveyUpdateValidator());
        }

        private async Task<bool> SurveyTypeExists(Guid surveyTypeId, CancellationToken cancellationToken)
        {
            var type = await _unitOfWork.SurveyTypeRepository.GetByIdAsync(surveyTypeId);
            return type is not null;
        }
    }

    public class QuestionSurveyUpdateValidator : AbstractValidator<QuestionSurveyUpdateCommand>
    {
        public QuestionSurveyUpdateValidator()
        {
            RuleFor(x => x.Id)
                .Must(g => g == null || g != Guid.Empty).WithMessage("QuestionId must be a valid GUID.");
            RuleFor(x => x.QuestionEn).NotEmpty().WithMessage("Question (English) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
            RuleFor(x => x.QuestionAr).NotEmpty().WithMessage("Question (Arabic) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");

            RuleFor(x => x)
                .Must(x => (x.Choices != null && x.EvaluateChoices == null) ||
                           (x.Choices == null && x.EvaluateChoices != null) ||
                           (x.QuestionType == QuestionType.Sample))
                .WithMessage("Each question must have either Choices or EvaluateChoices, but not both.");
            RuleFor(x => x)
                .Must(x =>
                (x.Choices == null && x.EvaluateChoices == null && (x.QuestionType == QuestionType.Sample || x.QuestionType == QuestionType.Evaluate)) ||
                (x.EvaluateChoices != null && x.QuestionType == QuestionType.Evaluate) ||
                (x.Choices != null && (x.QuestionType == QuestionType.OneChoice || x.QuestionType == QuestionType.MultiChoice))
                ).WithMessage("Invalid Question");
            RuleFor(x => x.Choices)
                .Must(choices => choices == null || choices.Count <= 10)
                .WithMessage("A question cannot have more than 10 choices.");

            RuleFor(x => x.EvaluateChoices)
                .Must(choices => choices == null || choices.Count <= 10)
                .WithMessage("A question cannot have more than 10 evaluation choices.");

            RuleForEach(x => x.Choices).SetValidator(new ChoiceSurveyUpdateValidator());
            RuleForEach(x => x.EvaluateChoices).SetValidator(new EvaluateChoiceSurveyUpdateValidator());
        }
    }

    public class ChoiceSurveyUpdateValidator : AbstractValidator<ChoiceSurveyUpdateCommand>
    {
        public ChoiceSurveyUpdateValidator()
        {
            RuleFor(x => x.Id)
                .Must(g => g == null || g != Guid.Empty).WithMessage("ChoiceId must be a valid GUID.");
            RuleFor(x => x.TextEn).NotEmpty().WithMessage("Choice text (English) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
            RuleFor(x => x.TextAr).NotEmpty().WithMessage("Choice text (Arabic) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
        }
    }

    public class EvaluateChoiceSurveyUpdateValidator : AbstractValidator<EvaluateChoiceSurveyUpdateCommand>
    {
        public EvaluateChoiceSurveyUpdateValidator()
        {
            RuleFor(x => x.Id)
                .Must(g => g == null || g != Guid.Empty).WithMessage("ChoiceId must be a valid GUID.");
            RuleFor(x => x.TextEn).NotEmpty().WithMessage("Evaluation choice text (English) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
            RuleFor(x => x.TextAr).NotEmpty().WithMessage("Evaluation choice text (Arabic) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
            RuleFor(x => x.Emotion).NotEmpty().WithMessage("Emotion is required.")
                .MaximumLength(10).WithMessage("The Max Length is 10");
        }
    }
}
