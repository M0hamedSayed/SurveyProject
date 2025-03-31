using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Survey.Application.Base;
using Survey.Domain.Abstractions;

namespace Survey.Application.Features.surveyFeature.Queries.GetSurveyUsers
{
    public class GetSurveyUsersQuery : IRequest<Response<PaginatedResult<string>>>
    {
        public required Guid Id { get; set; }
        public bool AllUsers {  get; set; } = true;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
