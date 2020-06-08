using Application.Aircrafts.Dto;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Aircrafts
{
    public class List
    {
        public class Query : IRequest<AircraftEnvelope>
        {
            public Query(int? limit, int? offset)
            {
                Limit = limit;
                Offset = offset;
            }

            public int? Limit { get; set; }
            public int? Offset { get; set; }

        }
        public class Handler : IRequestHandler<Query, AircraftEnvelope>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }
            public async Task<AircraftEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = context.Aircraft.AsQueryable();
                var aircrafts = await query
                    .Skip(request.Offset ?? 0)
                    .Take(request.Limit ?? 9)
                    .ToListAsync();

                return new AircraftEnvelope
                {
                    Aircrafts = mapper.Map<List<AircraftDto>>(aircrafts),
                    Count = query.Count()
                };
            }
        }
    }
}
