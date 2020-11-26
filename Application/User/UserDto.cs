using Common.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.User
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public ICollection<NotificationDto> Notifications { get; set; }
        public ICollection<int> Subscribes { get; set; }
    }
}
