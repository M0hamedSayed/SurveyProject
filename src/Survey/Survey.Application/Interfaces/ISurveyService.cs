using Microsoft.AspNetCore.Http;
using Survey.Application.Features.surveyFeature.Commands.AddSurvey;
using Survey.Application.Features.surveyFeature.Commands.UpdateSurvey;
using Survey.Application.Features.surveyFeature.Queries.GetAllSurveys;
using Survey.Application.Features.surveyFeature.Queries.GetSurveyUsers;
using Survey.Domain.Abstractions;
using Survey.Domain.Models.Survey;

namespace Survey.Application.Interfaces
{
    public interface ISurveyService
    {
        public Task<Surveys> AddSurvey(AddSurveyCommand request);
        public Task<string> UploadImage(IFormFile image);
        public Task<(List<Surveys>? surveys, int count)> GetAllSurveys(GetAllSurveysQuery request);
        public Task<Surveys?> GetSurveyById(Guid surveyId, bool withTracking = false);
        public Task<(List<SurveyType>? surveyTypes, int count)> GetAllSurveytypes(string? search, int pageNumber, int pageSize);
        public Task<Surveys?> UpdateSurvey(UpdateSurveyCommand request);
        public  Task<(List<string?>? emails, int count)> GetSurveyUsers(GetSurveyUsersQuery request);
    }
}
