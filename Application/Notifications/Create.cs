using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Notifications
{
    public class Create
    {
        public class Command : IRequest<List<NotificationDto>>
        {
            public string[] Usernames { get; set; }
            public string Body { get; set; }
        }

        public class Handler : IRequestHandler<Command, List<NotificationDto>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<List<NotificationDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var notificationsDto = new List<NotificationDto>();
                foreach (var username in request.Usernames)
                {

                    var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);

                    var notification = new Domain.Notification
                    {
                        Body = request.Body,
                        IsRead = false,
                        UserId = user.Id
                    };

                    await _context.Notifications.AddAsync(notification);

                }

                var success = await _context.SaveChangesAsync() > 0;

                if (success)
                {
                    //foreach (var username in request.Usernames)
                    //{
                    //    var notification = await _context.Notifications
                    //        .Where(x => !x.IsRead && x.Users.Any(x => x.UserName == username))
                    //        .ToListAsync();
                    //    notificationsDto.Add(new NotificationDto
                    //    {
                    //        Body = notification.
                    //    })
                    //}

                }

                throw new Exception("Problem saving changes");
            }
        }
    }
}
