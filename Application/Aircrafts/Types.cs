using Application.Aircrafts.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Aircrafts
{
    public class Types
    {
        public class Query : IRequest<TypeDto> { }
        public class Handler : IRequestHandler<Query, TypeDto>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<TypeDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var types = await _context.Type.ToListAsync();
                var typesDto = new TypeDto();
                typesDto.Types = new string[types.Count];
                for (int i = 0; i < types.Count; i++)
                {
                    typesDto.Types[i] = types[i].TypeName;
                }

                return typesDto;
            }
        }
    }
}
