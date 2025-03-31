using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record SendEmailEvent (Guid Id,string Email, string Body, string Subject, bool IsHtml) : DomainEvent
    {
    }
}
