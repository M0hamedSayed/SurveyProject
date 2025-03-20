using Microsoft.AspNetCore.Identity;
using Survey.Domain.ValueObjects.Identity;

namespace Survey.Domain.Models.Identity
{
    public class ApplicationRole: IdentityRole<Guid>
    {
        private ApplicationRole() { }
        public ApplicationRole( string roleName) 
        {
            Name = roleName;
        }
    }
}
