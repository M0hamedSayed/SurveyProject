using Mapster;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Domain.Models.Survey;

namespace Survey.Application.Mapping.SurveyMapping
{
    public partial class SurveyMapping
    {
        public void AddSurveyMapping(TypeAdapterConfig config)
        {
            // Mapping from Aggregate to Database Entity
            config.NewConfig<Surveys, AddSurveyResult>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.SurveyTypeId, src => src.SurveyTypeId)
                .Map(dest => dest.NameEn, src => src.NameEn)
                .Map(dest => dest.NameAr, src => src.NameAr)
                .Map(dest => dest.StartDate, src => src.StartDate)
                .Map(dest => dest.EndDate, src => src.EndDate)
                .Map(dest => dest.DescriptionEn, src => src.DescriptionEn)
                .Map(dest => dest.DescriptionAr, src => src.DescriptionAr)
                .Map(dest => dest.IsRequired, src => src.IsRequired)
                .Map(dest => dest.IsActive, src => src.IsActive)
                .Map(dest => dest.Timezone, src => src.Timezone)
                .Map(dest => dest.Questions, src => src.questions.Adapt<List<QuestionSurveyResult>>())
                .Map(dest => dest.SurveyTypeNameEn, src => src.SurveyType.NameEn)
                .Map(dest => dest.SurveyTypeNameAr, src => src.SurveyType.NameAr);


            config.NewConfig<QuestionSurvey, QuestionSurveyResult>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.QuestionEn, src => src.QuestionEn)
                .Map(dest => dest.QuestionAr, src => src.QuestionAr)
                .Map(dest => dest.QuestionType, src => src.QuestionType)
                .Map(dest => dest.Choices, src => src.Choices.Adapt<List<ChoiceSurveyResult>>())
                .Map(dest => dest.EvaluateChoices, src => src.EvaluateChoices.Adapt<List<EvaluateChoiceSurveyResult>>());

            config.NewConfig<ChoiceSurvey, ChoiceSurveyResult>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.TextEn, src => src.TextEn)
                .Map(dest => dest.TextAr, src => src.TextAr);

            config.NewConfig<EvaluateChoiceSurvey, EvaluateChoiceSurveyResult>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.TextEn, src => src.TextEn)
                .Map(dest => dest.TextAr, src => src.TextAr)
                .Map(dest => dest.Emotion, src => src.Emotion);
        }
    }
}
