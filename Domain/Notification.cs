using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<AppUser> Users { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
    }
}
