﻿using System;

namespace Domain
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public virtual AppUser Author { get; set; }
        public int AircraftId { get; set; }
        public virtual Aircraft Aircraft { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
