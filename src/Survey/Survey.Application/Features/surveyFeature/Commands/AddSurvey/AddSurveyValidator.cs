using FluentValidation;
using Microsoft.AspNetCore.Http;
using Survey.Application.Interfaces;
using Survey.Domain.Enums;
using Survey.Domain.Interfaces.Repositories;

namespace Survey.Application.Features.surveyFeature.Commands.AddSurvey
{
    public class AddSurveyValidator : AbstractValidator<AddSurveyCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddSurveyValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            AddSurveyCommandValidator();
        }
        public void AddSurveyCommandValidator()
        {
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

            RuleForEach(x => x.Questions).SetValidator(new QuestionSurveyValidator());
        }

        private async Task<bool> SurveyTypeExists(string surveyTypeId, CancellationToken cancellationToken)
        {
            if(Guid.TryParse(surveyTypeId, out var typeId))
            {
                var type = await _unitOfWork.SurveyTypeRepository.GetByIdAsync(typeId);
                return type is not null;
            } else return false;
        }
    }

    public class QuestionSurveyValidator : AbstractValidator<QuestionSurveyDto>
    {
        public QuestionSurveyValidator()
        {
            RuleFor(x => x.QuestionEn).NotEmpty().WithMessage("Question (English) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
            RuleFor(x => x.QuestionAr).NotEmpty().WithMessage("Question (Arabic) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");

            RuleFor(x => x)
                .Must(x => ((x.Choices != null || x.Choices?.Count > 0) && (x.EvaluateChoices == null || !x.EvaluateChoices.Any())) ||
                           ((x.Choices == null || !x.Choices.Any()) &&( x.EvaluateChoices != null || x.EvaluateChoices?.Count > 0)) ||
                           (x.QuestionType == QuestionType.Sample))
                .WithMessage("Each question must have either Choices or EvaluateChoices, but not both.");
            RuleFor(x => x)
                .Must(x =>
                ((x.Choices == null || !x.Choices.Any()) && (x.EvaluateChoices == null || !x.EvaluateChoices.Any()) && (x.QuestionType == QuestionType.Sample || x.QuestionType == QuestionType.Evaluate)) ||
                ((x.EvaluateChoices != null || x.EvaluateChoices?.Count > 0) && x.QuestionType == QuestionType.Evaluate) ||
                ((x.Choices != null || x.Choices?.Count > 0) && (x.QuestionType == QuestionType.OneChoice || x.QuestionType == QuestionType.MultiChoice))
                ).WithMessage("Invalid Question");
            RuleFor(x => x.Choices)
                .Must(choices => choices == null || choices.Count <= 10 || !choices.Any())
                .WithMessage("A question cannot have more than 10 choices.");

            RuleFor(x => x.EvaluateChoices)
                .Must(choices => choices == null || choices.Count <= 10 || !choices.Any())
                .WithMessage("A question cannot have more than 10 evaluation choices.");

            RuleForEach(x => x.Choices).SetValidator(new ChoiceSurveyValidator());
            RuleForEach(x => x.EvaluateChoices).SetValidator(new EvaluateChoiceSurveyValidator());
        }
    }

    public class ChoiceSurveyValidator : AbstractValidator<ChoiceSurveyDto>
    {
        public ChoiceSurveyValidator()
        {
            RuleFor(x => x.TextEn).NotEmpty().WithMessage("Choice text (English) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
            RuleFor(x => x.TextAr).NotEmpty().WithMessage("Choice text (Arabic) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
        }
    }

    public class EvaluateChoiceSurveyValidator : AbstractValidator<EvaluateChoiceSurveyDto>
    {
        public EvaluateChoiceSurveyValidator()
        {
            RuleFor(x => x.TextEn).NotEmpty().WithMessage("Evaluation choice text (English) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
            RuleFor(x => x.TextAr).NotEmpty().WithMessage("Evaluation choice text (Arabic) is required.")
                .MaximumLength(300).WithMessage("The Max Length is 300");
            RuleFor(x => x.Emotion).NotEmpty().WithMessage("Emotion is required.")
                .MaximumLength(10).WithMessage("The Max Length is 10");
        }
    }
}
