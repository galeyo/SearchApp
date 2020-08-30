using Application.Aircrafts.Dto;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nest;
using Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.AircraftSearch
{
    public class ReIndex
    {
        public class Command : MediatR.IRequest { }
        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly ElasticClient client;
            private readonly IMapper mapper;

            public Handler(DataContext context, ElasticClient client, IMapper mapper)
            {
                this.context = context;
                this.client = client;
                this.mapper = mapper;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var aircrafts = await context.Aircraft.ToListAsync();

                var aircraftsDto = mapper.Map<List<AircraftDto>>(aircrafts);
                //clear all docs from index
                var deleteDocsInIndexResponse = await client.DeleteByQueryAsync<AircraftDto>(d => d
                    .MatchAll());
                if (deleteDocsInIndexResponse.OriginalException != null) throw deleteDocsInIndexResponse.OriginalException;
                //add documents from database
                var addDocsInIndexResponse = await client.IndexManyAsync(aircraftsDto);

                if (addDocsInIndexResponse.OriginalException != null) throw addDocsInIndexResponse.OriginalException;

                if (!addDocsInIndexResponse.Errors && deleteDocsInIndexResponse.Failures.Count != 0) return Unit.Value;

                throw new Exception("Problem with reindex");
            }
        }
    }
}
