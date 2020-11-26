using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
    }
}
