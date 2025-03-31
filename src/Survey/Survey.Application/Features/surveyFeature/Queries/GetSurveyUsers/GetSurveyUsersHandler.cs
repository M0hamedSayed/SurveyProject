using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Interfaces;
using Survey.Domain.Abstractions;

namespace Survey.Application.Features.surveyFeature.Queries.GetSurveyUsers
{
    public class GetSurveyUsersHandler (ISurveyService surveyService) : ResponseHandler, IRequestHandler<GetSurveyUsersQuery, Response<PaginatedResult<string>>>
    {
        public async Task<Response<PaginatedResult<string>>> Handle(GetSurveyUsersQuery request, CancellationToken cancellationToken)
        {
            var res = await surveyService.GetSurveyUsers(request);

            if(res.emails == null || !res.emails.Any()) return NotFound<PaginatedResult<string>>();

            var data = PaginatedResult<string>.Success(res.emails,res.count, request.PageNumber, request.PageSize);
            return Success<PaginatedResult<string>>(data);
        }
    }
}
