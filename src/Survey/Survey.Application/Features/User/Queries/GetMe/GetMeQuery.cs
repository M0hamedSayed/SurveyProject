using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Survey.Application.Base;

namespace Survey.Application.Features.User.Queries.GetMe
{
    public record GetMeQuery : IRequest<Response<GetMeResult>>
    {
    }
}
