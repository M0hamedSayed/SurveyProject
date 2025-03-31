using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Survey.Application.Base;

namespace Survey.Application.Features.surveyFeature.Commands.AddSurveyPhoto
{
    public record AddSurveyPhotoCommand : IRequest<Response<string>>
    {
        public required IFormFile Image { get; set; }
    }
}
