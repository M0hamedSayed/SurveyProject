using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.Features.surveyFeature.Queries.GetAllSurveyTypes
{
    public record GetAllSurveyTypesResult
    {
        public required Guid Id { get; set; }
        public required string NameEn {  get; set; }
        public required string NameAr { get; set; }
    }
}
