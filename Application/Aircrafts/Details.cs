using Application.Aircrafts.Dto;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
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
    public class Details
    {
        public class Query : IRequest<AircraftDto>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, AircraftDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<AircraftDto> Handle(Query request, CancellationToken cancellationToken)
            {
                
                var aircraft = await _context.Aircraft
                    .FindAsync(new object[] { request.Id }, cancellationToken);
                if (aircraft == null)
                    throw new RestException(HttpStatusCode.NotFound, new { aircraft = "Not found" });
                var comments = aircraft.Comments.ToList();
                return _mapper.Map<Aircraft, AircraftDto>(aircraft);
            }
        }
    }
}
