﻿using Application.Errors;
using Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Aircrafts
{
    public class Subscribe
    {
        public class Command : IRequest
        {
            public int AircraftId { get; set; }
        }
        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var aircraft = await _context.Aircraft.FindAsync(new object[] { request.AircraftId }, cancellationToken);

                if (aircraft == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Aircraft = "Aircraft not found" });

                var user = await _context.Users
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername());

                var subscribe = await _context.Subscribes
                    .SingleOrDefaultAsync(x => x.AircraftId == aircraft.Id && x.UserId == user.Id);

                if (subscribe != null)
                    throw new RestException(HttpStatusCode.BadRequest, new { Subscrube = "Already subscribe to this aircraft" });
                subscribe = new Domain.Subscribe
                {
                    Aircraft = aircraft,
                    User = user
                };

                await _context.Subscribes.AddAsync(subscribe);

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");

            }
        }
    }
}
