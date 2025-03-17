using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Survey.Domain.Enums;
using Survey.Domain.Interfaces.Models;

namespace Survey.Domain.Events
{
    public class ConfirmEmailEvent : IDomainEvent
    {
        public string EventType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<NotificationTypes> NotifyTypes  = new List<NotificationTypes>() { NotificationTypes.Email };
    }
}
