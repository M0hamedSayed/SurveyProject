
using Mapster;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Domain.Models.Survey;

namespace Survey.Application.Mapping.SurveyMapping
{
    public partial class SurveyMapping
    {
        public void GetSurveyDetailsMapping(TypeAdapterConfig config) 
        {
            config.NewConfig<SurveyDetails, AddSurveyResult>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.NameEn, src => src.name_en)
                .Map(dest => dest.NameAr, src => src.name_ar)
                .Map(dest => dest.DescriptionEn, src => src.description_ar)
                .Map(dest => dest.DescriptionAr, src => src.description_ar)
                .Map(dest => dest.IsActive, src => src.is_active)
                .Map(dest => dest.IsRequired, src => src.is_required)
                .Map(dest => dest.ClosingAddressEn, src => src.closing_address_en)
                .Map(dest => dest.ClosingAddressAr, src => src.closing_address_ar)
                .Map(dest => dest.ClosingStatementEn, src => src.closing_statement_en)
                .Map(dest => dest.ClosingStatementAr, src => src.closing_statement_ar)
                .Map(dest => dest.ImageUrl, src => src.image_url)
                .Map(dest => dest.StartDate, src => src.start_date)
                .Map(dest => dest.EndDate, src => src.end_date)
                .Map(dest => dest.SurveyTypeId, src => src.SurveyTypeId)
                .Map(dest => dest.SurveyTypeNameEn, src => src.SurveyTypeNameEn)
                .Map(dest => dest.SurveyTypeNameAr, src => src.SurveyTypeNameAr)
                .IgnoreNonMapped(true);

            config.NewConfig<SurveyDetails, QuestionSurveyResult>()
                .Map(dest => dest.Id, src => src.QuestionId)
                .Map(dest => dest.QuestionEn, src => src.QuestionEn)
                .Map(dest => dest.QuestionAr, src => src.QuestionAr)
                .Map(dest => dest.QuestionType, src => src.QuestionType)
                .IgnoreNonMapped(true);

            config.NewConfig<SurveyDetails, ChoiceSurveyResult>()
                .Map(dest => dest.Id, src => src.ChoiceId)
                .Map(dest => dest.TextEn, src => src.ChoiceTextEn)
                .Map(dest => dest.TextAr, src => src.ChoiceTextAr);

            config.NewConfig<SurveyDetails, EvaluateChoiceSurveyResult>()
                .Map(dest => dest.Id, src => src.EvaluateChoiceId)
                .Map(dest => dest.TextEn, src => src.EvaluateChoiceTextEn)
                .Map(dest => dest.TextAr, src => src.EvaluateChoiceTextAr)
                .Map(dest => dest.Emotion, src => src.EvaluateChoiceEmoji);

        }
    }
}
