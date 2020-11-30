using Application.Errors;
using Application.Extensions;
using Common.Notifications;
using Common.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User
{
    public class Login
    {
        public class Query : IRequest<UserDto>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotEmpty().Password();
            }
        }


        public class Handler : IRequestHandler<Query, UserDto>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IJwtGenerator _jwtGenerator;

            public Handler(DataContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator)
            {
                _context = context;
                _userManager = userManager;
                _signInManager = signInManager;
                _jwtGenerator = jwtGenerator;
            }
            public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                    throw new RestException(HttpStatusCode.Unauthorized);

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (result.Succeeded)
                {
                    var notifications = await _context.Notifications.Where(x => x.UserId == user.Id && !x.IsRead).ToListAsync();
                    var notificationsDto = new List<NotificationDto>();
                    foreach (var notification in notifications)
                    {
                        notificationsDto.Add(new NotificationDto
                        {
                            Body = notification.Body,
                            Id = notification.Id,
                            IsRead = notification.IsRead
                        });
                    }
                    var subscribes = await _context.Subscribes.Where(x => x.UserId == user.Id).ToListAsync();
                    var subscribesDto = new List<int>();
                    foreach (var subscribe in subscribes)
                    {
                        subscribesDto.Add(subscribe.AircraftId);
                    }
                    return new UserDto
                    {
                        DisplayName = user.DisplayName,
                        Token = _jwtGenerator.CreateToken(user),
                        UserName = user.UserName,
                        Image = user.Image,
                        Notifications = notificationsDto,
                        Subscribes = subscribesDto
                    };
                }

                throw new RestException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
