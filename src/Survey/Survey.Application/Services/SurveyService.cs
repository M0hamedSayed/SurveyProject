using System.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;
using Survey.Application.Extensions;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Application.Features.surveyFeature.Commands.UpdateSurvey;
using Survey.Application.Features.surveyFeature.Queries.GetAllSurveys;
using Survey.Application.Features.surveyFeature.Queries.GetSurveyUsers;
using Survey.Application.Interfaces;
using Survey.Domain.Abstractions;
using Survey.Domain.Enums;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Identity;
using Survey.Domain.Models.Survey;
using Survey.Domain.ValueObjects.Survey;
namespace Survey.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<SurveyService> _logger;
        public SurveyService(IUnitOfWork unitOfWork, IWebHostEnvironment env, ICurrentUserService currentUserService, ILogger<SurveyService> logger)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Surveys> AddSurvey(AddSurveyCommand request)
        {
            Guid.TryParse(request.SurveyTypeId, out var typeId);

            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // create survey
                var survey = Surveys.Create
                    (
                    Guid.NewGuid(),
                    _currentUserService.GetUserId(),
                   typeId,
                    request.NameEn,
                    request.NameAr,
                    request.DescriptionEn,
                    request.DescriptionAr,
                    request.ClosingAddressAr,
                    request.ClosingAddressEn,
                    request.ClosingStatementAr,
                    request.ClosingStatementEn,
                    request.StartDate.ToUniversalTime(),
                    request.EndDate.ToUniversalTime(),
                    request.Timezone,
                    request.ImageUrl,
                    request.IsRequired
                    );

                survey = AddQuestions(survey, request.Questions);
                await _unitOfWork.SurveyRepository.AddAsync(survey);
                await _unitOfWork.Complete();
                await transaction.CommitAsync();
                return survey;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Something happen when save survey , try again later");
            }
        }

        public async Task<List<SurveyDetails>> AddSurveyWithSP(AddSurveyCommand request)
        {
            var userWithRoles = await _currentUserService.GetCurrentUserWithRolesAsync();
            bool isAdmin = userWithRoles.roles.Where(r => r.Equals("Admin")).Any();
            if (userWithRoles.user is null) throw new UnauthorizedAccessException("Unauthorized");

            Guid? userId = isAdmin ? userWithRoles.user.Id : userWithRoles.user.ManagerId;
            if(userId == null) throw new UnauthorizedAccessException("Unauthorized");
            // Prepare DataTables
            var surveyDetailsTable = request.ToSurveyDetailsTable((Guid)userId);
            var questionsTable = request.Questions.ToQuestionsTable();
            var choicesTable = request.Questions.ToChoicesTable();
            var evaluateChoicesTable = request.Questions.ToEvaluateChoicesTable();

            return await _unitOfWork.SurveyRepository.AddSurveyWithSp(surveyDetailsTable, questionsTable, choicesTable, evaluateChoicesTable);
        }
        public async Task<Surveys?> UpdateSurvey(UpdateSurveyCommand request)
        {
            // get required survey
            var survey = await GetSurveyById(request.Id, true);
            if (survey == null || survey.IsActive) return null;
            // update survey
            survey.Update(
                    request.SurveyTypeId,
                    request.NameEn,
                    request.NameAr,
                    request.DescriptionEn,
                    request.DescriptionAr,
                    request.ClosingAddressAr,
                    request.ClosingAddressEn,
                    request.ClosingStatementAr,
                    request.ClosingStatementEn,
                    request.StartDate,
                    request.EndDate,
                    request.Timezone,
                    request.ImageUrl,
                    request.IsRequired
            );
            //Remove Deleted Questions
            var existingQuestionIds = request.Questions.Where(q => q.Id.HasValue).Select(q => q.Id).ToList();
            var removalQuestionIds = survey.questions.Where(q => !existingQuestionIds.Contains(q.Id)).Select(q => q.Id).ToList();
            survey.RemoveMultiAuestions(removalQuestionIds);


            foreach (var questionDto in request.Questions)
            {
                if(questionDto.Id.HasValue)
                {
                    var question = survey.questions.FirstOrDefault(q=> q.Id == questionDto.Id);
                    if (question == null) throw new KeyNotFoundException("Question not found");
                   

                    //_unitOfWork.QuestionSurveyRepository.Entry(question).State = EntityState.Modified;
                    question = survey.UpdateQuestion(question.Id, questionDto.QuestionEn, questionDto.QuestionAr, questionDto.QuestionType);

                    // Remove Deleted Choices
                    var existingChoicesIds = questionDto?.Choices?.Where(c => c.Id != null && c.Id.HasValue).Select(c => c.Id).ToList();
                    var existingEvaluateChoicesIds = questionDto?.EvaluateChoices?.Where(c => c.Id != null && c.Id.HasValue).Select(c => c.Id).ToList();
                    if (existingChoicesIds is not null && existingChoicesIds.Any())
                    {
                        List<object> removalChoiceIds = question.Choices.Where(c => !existingChoicesIds.Contains(c.Id.Value)).Select(c => c.Id as object).ToList();
                        question.RemoveMultiChoise(removalChoiceIds);
                    }
                    if (existingEvaluateChoicesIds is not null && existingEvaluateChoicesIds.Any())
                    {
                        List<object> removalChoiceIds = question.EvaluateChoices.Where(c => !existingEvaluateChoicesIds.Contains(c.Id.Value)).Select(c => c.Id as object).ToList();
                        question.RemoveMultiChoise(removalChoiceIds);
                    }

                    if (questionDto?.QuestionType is not QuestionType.Sample)
                    {
                        if (questionDto?.Choices != null && questionDto.QuestionType is not QuestionType.Evaluate)
                            foreach (var choiceDto in questionDto.Choices)
                            {
                                if (choiceDto.Id is not null && choiceDto.Id.HasValue)
                                {
                                    var choice = question.Choices.FirstOrDefault(q => q.Id.Value == choiceDto.Id);
                                    if (choice == null) throw new KeyNotFoundException("Choice not found");

                                    question.UpdateChoice(choice.Id, choiceDto.TextEn, choiceDto.TextAr);
                                }
                                else // add new choice
                                {
                                    question.AddChoice(choiceDto.TextEn, choiceDto.TextAr);
                                }
                            }
                        else if(questionDto?.EvaluateChoices != null && questionDto.QuestionType is  QuestionType.Evaluate)
                            foreach (var evaluateChoiceDto in questionDto.EvaluateChoices)
                            {
                                if (evaluateChoiceDto.Id is not null && evaluateChoiceDto.Id.HasValue)
                                {
                                    var evaludateChoice = question.EvaluateChoices.FirstOrDefault(q => q.Id.Value == evaluateChoiceDto.Id);
                                    if (evaludateChoice == null) throw new KeyNotFoundException("Evaluate Choice not found");

                                    question.UpdateChoice(evaludateChoice.Id, evaluateChoiceDto.TextEn, evaluateChoiceDto.TextAr, evaluateChoiceDto.Emotion);
                                }
                                else // add new evaluate choice
                                    question.AddChoice(evaluateChoiceDto.TextEn, evaluateChoiceDto.TextAr, evaluateChoiceDto.Emotion);

                            }
                        else throw new Exception("Invalid Choices");
                    }
                }
                else
                {
                    var questionSurveyEntity = survey.AddNewQuestions
                    (
                        Guid.NewGuid(),
                        questionDto.QuestionEn,
                        questionDto.QuestionAr,
                        questionDto.QuestionType
                    );
                    _unitOfWork.QuestionSurveyRepository.Entry(questionSurveyEntity).State = EntityState.Added;

                    if (questionDto.QuestionType is not QuestionType.Sample)
                        questionSurveyEntity = AddChoices<ChoiceSurveyUpdateCommand, EvaluateChoiceSurveyUpdateCommand>(questionSurveyEntity, questionDto?.Choices, questionDto?.EvaluateChoices);
                    
                }
            }
            _unitOfWork.SurveyRepository.Update(survey);
            await _unitOfWork.Complete();
            return survey;
        }
        public async Task<(List<Surveys>? surveys, int count)> GetAllSurveys(GetAllSurveysQuery request)
        {
            var userWithRoles = await _currentUserService.GetCurrentUserWithRolesAsync();
            
            var criteria = HandleGetAllSurveysCriteria(request, userWithRoles);

            return await _unitOfWork.SurveyRepository.GetAllSurveys(criteria, request.PageNumber, request.PageSize);
        }

        public async Task<(List<SurveyDetails>? surveyDetails, int count)> GetAllSurveysWithSp(GetAllSurveysQuery request)
        {
            var userWithRoles = await _currentUserService.GetCurrentUserWithRolesAsync();
            bool isAdmin = userWithRoles.roles.Where(r => r.Equals("Admin")).Any();
            if (userWithRoles.user is null) throw new UnauthorizedAccessException("Unauthorized");

            var sDetials = await _unitOfWork.SurveyRepository.GetAllSurveysWithSP( 
                    userWithRoles.user.Id,
                    isAdmin,
                    request.StartDate,
                    request.EndDate,
                    request.SurveyTypeId,
                    request.IsActive,
                    request.Search,
                    request.PageNumber,
                    request.PageSize
                );
            return (sDetials, sDetials?.FirstOrDefault()?.SurveyCount ?? 0);
        } 

        public async Task<Surveys?> GetSurveyById(Guid surveyId, bool withTracking = false)
        {
            var userWithRoles = await _currentUserService.GetCurrentUserWithRolesAsync();
            bool isAdmin = userWithRoles.roles.Where(r => r.Equals("Admin")).Any();
            if (userWithRoles.user is null) throw new UnauthorizedAccessException("Unauthorized");

            Expression<Func<Surveys, bool>> criteria = s => (isAdmin ? s.UserId == userWithRoles.user.Id : s.UserId == userWithRoles.user.ManagerId) && s.Id == surveyId;
            Expression<Func<Surveys, bool>> userCriteria = s => isAdmin == false ? s.IsActive || s.StartDate <= DateTime.UtcNow : true;
            criteria = CombineExpressions(criteria, userCriteria);

            return withTracking ? await _unitOfWork.SurveyRepository.FindWithTracking(criteria) : await _unitOfWork.SurveyRepository.FindAsync(criteria);
        }
         
        public async Task<List<SurveyDetails>?> GetSurveyByIdWithSP(Guid surveyId)
        {
            var userWithRoles = await _currentUserService.GetCurrentUserWithRolesAsync();
            bool isAdmin = userWithRoles.roles.Where(r => r.Equals("Admin")).Any();
            if (userWithRoles.user is null) throw new UnauthorizedAccessException("Unauthorized");

            return await _unitOfWork.SurveyRepository.FindWithSP(surveyId, userWithRoles.user.Id, isAdmin);
        }

        public async Task<(List<SurveyType>? surveyTypes, int count)> GetAllSurveytypes(string? search,int pageNumber, int pageSize)
        {
            Expression<Func<SurveyType, bool>> criteria = t => (search != null && search.Trim() != "") ? (t.NameEn.Contains(search.Trim()) || t.NameAr.Contains(search.Trim())) : true ;
            var surveyTypes = await  _unitOfWork.SurveyTypeRepository.FindAllAsync( criteria,null, pageSize, (pageNumber - 1) * pageSize);
            var count = await _unitOfWork.SurveyTypeRepository.GetTableNoTracking().CountAsync(criteria);

            return (surveyTypes.ToList(), count);
        }


        public async Task<bool> DeleteSurvey(Guid surveyId)
        {
            return true;
        }

        public async Task<(List<string?>? emails, int count)> GetSurveyUsers(GetSurveyUsersQuery request)
        {
            if(request.AllUsers)
            {
                Guid? managerId = await _unitOfWork.SurveyRepository
                    .GetTableNoTracking()
                    .Where(s => s.Id == request.Id)
                    .Select(s=> s.UserId)
                    .FirstOrDefaultAsync();

                if (managerId is null || managerId == Guid.Empty) return (null, 0);

                var emails = await _unitOfWork.UserRepository.GetTableNoTracking().Where(u => u.ManagerId == managerId)
                    .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                    .Select( u => u.Email).ToListAsync();
                var count = await _unitOfWork.UserRepository.GetTableNoTracking().Where(u => u.ManagerId == managerId).CountAsync();
                return (emails, count);
            }
            else
            {
                var respondedUserIds = await _unitOfWork.ResponseRepository.GetTableNoTracking()
                    .Where(sr => sr.SurveyId == request.Id)
                    .Select(sr => sr.UserId)
                    .ToListAsync();

                Guid? managerId = await _unitOfWork.SurveyRepository
                    .GetTableNoTracking()
                    .Where(s => s.Id == request.Id)
                    .Select(s => s.UserId)
                    .FirstOrDefaultAsync();

                if (managerId is null || managerId == Guid.Empty) return (null, 0);

                respondedUserIds.Add(managerId.Value);

                // Get all users who have NOT responded
                var emails = await _unitOfWork.UserRepository.GetTableNoTracking()
                    .Where(u => !respondedUserIds.Contains(u.Id) && u.ManagerId == managerId)
                    .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                    .Select(u => u.Email)
                    .ToListAsync();
                var count = await _unitOfWork.UserRepository.GetTableNoTracking()
                    .Where(u => !respondedUserIds.Contains(u.Id)).CountAsync();
                return (emails, count);
            }
        }

        private Expression<Func<Surveys, bool>> HandleGetAllSurveysCriteria(GetAllSurveysQuery request , (ApplicationUser? user, IList<string> roles) userWithRoles)
        {
            bool isAdmin = userWithRoles.roles.Where(r => r.Equals("Admin")).Any();
            if (userWithRoles.user is null) throw new UnauthorizedAccessException("Unauthorized");

            Expression<Func<Surveys, bool>> criteria = s => isAdmin ? s.UserId == userWithRoles.user.Id : s.UserId == userWithRoles.user.ManagerId;
            Expression<Func<Surveys, bool>> userCriteria = s => isAdmin == false ? s.IsActive || s.StartDate <= DateTime.UtcNow : true;
            criteria = CombineExpressions(criteria, userCriteria);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                criteria = CombineExpressions(criteria, s => s.NameAr.Contains(request.Search.Trim()) || s.NameEn.Contains(request.Search.Trim()));
            }
            if (request.StartDate.HasValue && !request.EndDate.HasValue)
            {
                criteria = CombineExpressions(criteria, s => s.StartDate >= request.StartDate);
            }
            else if (!request.StartDate.HasValue && request.EndDate.HasValue)
            {
                criteria = CombineExpressions(criteria, s => s.EndDate <= request.EndDate);
            }
            else if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                criteria = CombineExpressions(criteria, s => s.StartDate >= request.StartDate && s.EndDate <= request.EndDate);
            }
            if (!string.IsNullOrWhiteSpace(request.SurveyTypeId))
            {
                if (Guid.TryParse(request.SurveyTypeId, out var typeId))
                    criteria = CombineExpressions(criteria, s => s.SurveyTypeId == typeId);
            }
            if (request.IsActive.HasValue)
            {
                criteria = CombineExpressions(criteria, s => s.IsActive == request.IsActive.Value);
            }
            return criteria;
        }
        private static Expression<Func<T, bool>> CombineExpressions<T>(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var combined = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    Expression.Invoke(expr1, parameter),
                    Expression.Invoke(expr2, parameter)
                ), parameter);

            return combined;
        }

        private Surveys AddQuestions(Surveys survey, List<QuestionSurveyDto> questions)
        {
            foreach (var question in questions)
            {

                var questionSurveyEntity = survey.AddNewQuestions
                    (
                        Guid.NewGuid(),
                        question.QuestionEn,
                        question.QuestionAr,
                        question.QuestionType
                    );
                if (question.QuestionType is not QuestionType.Sample)
                    questionSurveyEntity = AddChoices<ChoiceSurveyDto, EvaluateChoiceSurveyDto>(questionSurveyEntity, question.Choices, question.EvaluateChoices);
            }

            return survey;
        }

        private QuestionSurvey AddChoices<C,E>(
            QuestionSurvey questionSurvey,
            List<C>? choices,
            List<E>? evaluates
            )
        {
            if (questionSurvey == null)
            {
                throw new ArgumentNullException(nameof(questionSurvey), "QuestionSurvey cannot be null.");
            }

            bool hasChoices = choices is not null && choices.Any();
            bool hasEvaluates = evaluates is not null && evaluates.Any();

            if (questionSurvey.QuestionType == QuestionType.Evaluate)
            {
                if (hasEvaluates)
                {
                    foreach (var evaluate in evaluates!)
                    {

                        if (evaluate is EvaluateChoiceSurveyUpdateCommand evalCmd)
                        {
                            questionSurvey.AddChoice(evalCmd.TextEn, evalCmd.TextAr, evalCmd.Emotion);
                        }
                        else if (evaluate is EvaluateChoiceSurveyDto evalDto)
                        {
                            questionSurvey.AddChoice(evalDto.TextEn, evalDto.TextAr, evalDto.Emotion);
                        }
                    }
                }
            }
            else
            {
                if (hasChoices)
                {
                    foreach (var choice in choices!)
                    {
                        if (choice is ChoiceSurveyUpdateCommand choiceCmd)
                        {
                            questionSurvey.AddChoice(choiceCmd.TextEn, choiceCmd.TextAr);
                        }
                        else if (choice is ChoiceSurveyDto choiceDto)
                        {
                            questionSurvey.AddChoice(choiceDto.TextEn, choiceDto.TextAr);
                        }
                    }
                }
            }

            return questionSurvey;
        }

        public async Task<string> UploadImage(IFormFile image)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{Guid.NewGuid().ToString().Replace("-", string.Empty)}{Path.GetExtension(image.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            return $"/uploads/{uniqueFileName}";
        }


    }
}
