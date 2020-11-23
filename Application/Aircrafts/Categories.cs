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
    public class Categories
    {
        public class Query : IRequest<CategoryDto> { }

        public class Handler : IRequestHandler<Query, CategoryDto>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<CategoryDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var categories = await _context.Category.ToListAsync();
                var categoryDto = new CategoryDto();
                categoryDto.Categories = new string[categories.Count];
                for (int i = 0; i < categories.Count; i++)
                {
                    categoryDto.Categories[i] = categories[i].CategoryName;
                }

                return categoryDto;
            }
        }
    }
}
