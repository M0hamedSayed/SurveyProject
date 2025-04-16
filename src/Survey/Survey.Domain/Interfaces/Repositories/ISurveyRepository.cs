using System.Data;
using System.Linq.Expressions;
using Survey.Domain.Models.Survey;

namespace Survey.Domain.Interfaces.Repositories
{
    public interface ISurveyRepository : IGenericRepositoryAsync<Surveys>
    {
        public Task<(List<Surveys>? surveys, int count)> GetAllSurveys(Expression<Func<Surveys, bool>> criteria, int page = 1, int pageSize = 10);
        public Task<Surveys?> FindWithTracking(Expression<Func<Surveys, bool>> criteria);
        public Task<List<SurveyDetails>?> FindWithSP(Guid surveyId, Guid userId, bool isAdmin);
        public Task<List<SurveyDetails>?> GetAllSurveysWithSP
            (
                Guid userId,
                bool isAdmin,
                DateTime? startDate,
                DateTime? enddate,
                string? surveyTypeId,
                bool? isActive,
                string? search,
                int page = 1,
                int pageSize = 10
            );
        public Task<List<SurveyDetails>> AddSurveyWithSp(DataTable surveyTable, DataTable questionTable, DataTable? choiceTable, DataTable? evaluateChoicetable);
    }

    public interface ISurveyTypeRepository : IGenericRepositoryAsync<SurveyType>
    {
    }

    public interface IQuestionSurveyRepository : IGenericRepositoryAsync<QuestionSurvey>
    {
    }

    public interface IChoiceRepository : IGenericRepositoryAsync<ChoiceSurvey>
    {
    }

    public interface IEvaluateChoiceRepository : IGenericRepositoryAsync<EvaluateChoiceSurvey>
    {
    }

    public interface ISurveyResponseRepository : IGenericRepositoryAsync<SurveyResponse> { }
}
