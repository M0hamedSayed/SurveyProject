using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.Features.User.Queries.GetMe
{
    public record GetMeResult
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required List<string> Roles { get; set; }
    }
}
