using System.Data;
using System.Linq.Expressions;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<List<SurveyDetails>?> FindWithSP(Guid surveyId, Guid userId, bool isAdmin)
        {
            var parameters = new[]
            {
                new SqlParameter("@SurveyId",surveyId),
                new SqlParameter("@UserId",userId),
                new SqlParameter("@IsAdmin",isAdmin)
            };
            // Execute stored procedure
            var result = await _context.Set<SurveyDetails>()
                .FromSqlRaw("EXEC getOneSurvey @SurveyId, @UserId, @IsAdmin", parameters)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }

        public async Task<List<SurveyDetails>?> GetAllSurveysWithSP
            (
                Guid userId, 
                bool isAdmin, 
                DateTime? startDate, 
                DateTime? endDate, 
                string? surveyTypeId,
                bool? isActive,
                string? search,
                int page = 1, 
                int pageSize = 10
            )
        {
            // Validate and convert surveyTypeId
            Guid? surveyTypeGuid = null;
            if (!string.IsNullOrWhiteSpace(surveyTypeId))
            {
                if (Guid.TryParse(surveyTypeId, out var guidValue))
                {
                    surveyTypeGuid = guidValue;
                }
                else
                {
                    throw new ArgumentException("surveyTypeId must be a valid GUID");
                }
            }

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@IsAdmin", isAdmin),
                new SqlParameter("@Search", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search),
                new SqlParameter("@StartDate", startDate.HasValue ? (object)startDate.Value : DBNull.Value),
                new SqlParameter("@EndDate", endDate.HasValue ? (object)endDate.Value : DBNull.Value),
                new SqlParameter("@SurveyTypeId", surveyTypeGuid.HasValue ? (object)surveyTypeGuid.Value : DBNull.Value),
                new SqlParameter("@IsActive", isActive.HasValue ? (object)isActive.Value : DBNull.Value),
                new SqlParameter("@Page", page),
                new SqlParameter("@PageSize", pageSize)
            };

            var result = await _context.Set<SurveyDetails>()
                .FromSqlRaw("EXEC sp_GetFilteredSurveys @UserId, @IsAdmin, @Search, @StartDate, @EndDate, @SurveyTypeId, @IsActive, @Page, @PageSize", parameters)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }

        public async Task<List<SurveyDetails>> AddSurveyWithSp(DataTable surveyTable, DataTable questionTable, DataTable? choiceTable, DataTable? evaluateChoicetable)
        {
            // Create parameters
            var parameters = new[]
            {
                new SqlParameter("@SurveyDetails", surveyTable)
                {
                    TypeName = "Survey.SurveyTableType",
                    SqlDbType = SqlDbType.Structured
                },
                new SqlParameter("@SurveyQuestions", questionTable)
                {
                    TypeName = "Survey.SurveyQuestionsType",
                    SqlDbType = SqlDbType.Structured
                },
                new SqlParameter("@SurveyChoices", choiceTable)
                {
                    TypeName = "Survey.SurveyChoicesType",
                    SqlDbType = SqlDbType.Structured
                },
                new SqlParameter("@EvaluateChoices", evaluateChoicetable)
                {
                    TypeName = "Survey.EvaluateChoicesType",
                    SqlDbType = SqlDbType.Structured
                }
            };

            return await _context.Set<SurveyDetails>()
            .FromSqlRaw("EXEC [Survey].[sp_InsertSurvey] @SurveyDetails, @SurveyQuestions, @SurveyChoices, @EvaluateChoices", parameters)
            .AsNoTracking()
            .ToListAsync();
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
