using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Interfaces;

namespace Survey.Application.Features.surveyFeature.Commands.AddSurveyPhoto
{
    public class AddSurveyPhotoHandler (ISurveyService surveyService) : ResponseHandler, IRequestHandler<AddSurveyPhotoCommand, Response<string>>
    {
        public async Task<Response<string>> Handle(AddSurveyPhotoCommand request, CancellationToken cancellationToken)
        {
            var url = await surveyService.UploadImage(request.Image);
            return Success(url);
        }
    }
}
