using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Enums;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Survey;
using Survey.Infrastructure.DatabaseContext;

namespace Survey.Infrastructure.Repositories
{
    public class SurveyRepository(ApplicationDbContext dbContext) : GenericrepositoryAsync<Surveys>(dbContext), ISurveyRepository
    {
        private readonly DbSet<Surveys> _surveys = dbContext.Set<Surveys>();
        //private readonly IDbContextFactory<ApplicationDbContext> _contextFactory = contextFactory;

        public async Task<(List<Surveys>? surveys, int count)> GetAllSurveys(Expression<Func<Surveys, bool>> criteria, int page = 1, int pageSize = 10)
        {
            var surveysCount = await _surveys.AsNoTracking().Where(criteria).CountAsync();
            var surveys = await _surveys.AsNoTracking()
                .Where(criteria)
                .Include(s => s.questions)
                .ThenInclude(c => c.Choices)
                .Include(s => s.questions)
                .ThenInclude(c => c.EvaluateChoices)
                .Include(s => s.SurveyType)
                .OrderByDescending(s => s.StartDate)
                .AsSplitQuery()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (surveys is null || surveys?.Count == 0) return (new List<Surveys>(), surveysCount);
            // Explicit loading for related data in parallel

            //var loadTasks = surveys?.Select(survey => Task.Run(async () =>
            //{
            //    using var newContext = _contextFactory.CreateDbContext(); // Create a new DbContext instance

            //    foreach (var question in survey.questions)
            //    {
            //        if (question.QuestionType == QuestionType.MultiChoice || question.QuestionType == QuestionType.OneChoice)
            //        {
            //            await newContext.Entry(question).Collection(q => q.Choices).LoadAsync();
            //        }
            //        else if (question.QuestionType == QuestionType.Evaluate)
            //        {
            //            await newContext.Entry(question).Collection(q => q.EvaluateChoices).LoadAsync();
            //        }
            //    }
            //}));
            //await Task.WhenAll(loadTasks!); // Run all queries in parallel
            return (surveys, surveysCount);
        }

        public new async Task<Surveys?> FindAsync(Expression<Func<Surveys, bool>> criteria, string[]? includes = null)
        {
            var survey = await _surveys.AsNoTracking().Where(criteria)
                .Include(s => s.questions)
                .ThenInclude(c => c.Choices)
                .Include(s => s.questions)
                .ThenInclude(c => c.EvaluateChoices)
                .Include(s => s.SurveyType)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if(survey is null) return null;
            //var tasks = Task.Run(async () => {
            //    using var newContext = _contextFactory.CreateDbContext(); // Create a new DbContext instance

            //    foreach (var question in survey.questions)
            //    {
            //        if (question.QuestionType == QuestionType.MultiChoice || question.QuestionType == QuestionType.OneChoice)
            //        {
            //            await newContext.Entry(question).Collection(q => q.Choices).LoadAsync();
            //        }
            //        else if (question.QuestionType == QuestionType.Evaluate)
            //        {
            //            await newContext.Entry(question).Collection(q => q.EvaluateChoices).LoadAsync();
            //        }
            //    }
            //});
            //await Task.WhenAll(tasks);
            return survey;
        }
        public async Task<Surveys?> FindWithTracking(Expression<Func<Surveys, bool>> criteria)
        {
            var survey = await _surveys.AsTracking()
                .Include(s => s.questions)
                .ThenInclude(c => c.Choices)
                .Include(s => s.questions)
                .ThenInclude(c => c.EvaluateChoices)
                .Include(s => s.SurveyType)
                .FirstOrDefaultAsync(criteria);

            //if (survey is null) return null;
            //var tasks = Task.Run(async () => {
            //    using var newContext = _contextFactory.CreateDbContext(); // Create a new DbContext instance

            //    foreach (var question in survey.questions)
            //    {
            //        if (question.QuestionType == QuestionType.MultiChoice || question.QuestionType == QuestionType.OneChoice)
            //        {
            //            await newContext.Entry(question).Collection(q => q.Choices).LoadAsync();
            //        }
            //        else if (question.QuestionType == QuestionType.Evaluate)
            //        {
            //            await newContext.Entry(question).Collection(q => q.EvaluateChoices).LoadAsync();
            //        }
            //    }
            //});
            //await Task.WhenAll(tasks);
            return survey;
        }

    }


    public class SurveyTypeRepository(ApplicationDbContext dbContext) : GenericrepositoryAsync<SurveyType>(dbContext), ISurveyTypeRepository
    {
        private readonly DbSet<SurveyType> _surveyType = dbContext.Set<SurveyType>();
    }

    public class SurveyQuestionRepository(ApplicationDbContext dbContext) : GenericrepositoryAsync<QuestionSurvey>(dbContext), IQuestionSurveyRepository
    {
        private readonly DbSet<QuestionSurvey> _surveyQuestion = dbContext.Set<QuestionSurvey>();
    }

    public class ChoiceSurveynRepository(ApplicationDbContext dbContext) : GenericrepositoryAsync<ChoiceSurvey>(dbContext), IChoiceRepository
    {
        private readonly DbSet<ChoiceSurvey> _choice = dbContext.Set<ChoiceSurvey>();
    }

    public class EvaluateChoiceSurveynRepository(ApplicationDbContext dbContext) : GenericrepositoryAsync<EvaluateChoiceSurvey>(dbContext), IEvaluateChoiceRepository
    {
        private readonly DbSet<EvaluateChoiceSurvey> _evaluateChoice = dbContext.Set<EvaluateChoiceSurvey>();
    }
    public class SurveyResponseRepository(ApplicationDbContext dbContext) : GenericrepositoryAsync<SurveyResponse>(dbContext), ISurveyResponseRepository
    {
        private readonly DbSet<SurveyResponse> _surveyResponse = dbContext.Set<SurveyResponse>();
    }
}
