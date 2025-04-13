using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsterMapper;
using MediatR;
using Survey.Application.Base;
using Survey.Application.Interfaces;
using Survey.Domain.Models.Identity;

namespace Survey.Application.Features.User.Queries.GetMe
{
    public class GetMeHandler(ICurrentUserService currentUserService, IMapper mapper) : ResponseHandler, IRequestHandler<GetMeQuery, Response<GetMeResult>>
    {
        public async Task<Response<GetMeResult>> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            var userData = await currentUserService.GetCurrentUserWithRolesAsync();

            if (userData.user is null) return BadRequest<GetMeResult>("Something wrong");

            GetMeResult result = mapper.Map<GetMeResult>(userData.user);
            result.Roles = userData.roles.ToList();

            return Success<GetMeResult>(result);
        }
    }
}
