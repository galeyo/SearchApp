using Application.Aircrafts.Dto;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Aircrafts
{
    public class List
    {
        public class Query : IRequest<ICollection<AircraftDto>>
        {

        }
        public class Handler : IRequestHandler<Query, ICollection<AircraftDto>>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }
            public async Task<ICollection<AircraftDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var aircrafts = await context.Aircraft.Take(80).ToListAsync();
                var aircraftsDto = new List<AircraftDto>();
                foreach(var aircraft in aircrafts)
                {
                    var aircraftCategory = aircraft.AircraftCategory.ToList();
                    var aircraftTypes = aircraft.AircraftTypes.ToList();
                    var categories = new List<string>();
                    var types = new List<string>();
                    foreach (var category in aircraftCategory)
                    {
                        categories.Add(category.Category.CategoryName);
                    }
                    foreach (var type in aircraftTypes)
                    {
                        types.Add(type.Type.TypeName);
                    }
                    aircraftsDto.Add(new AircraftDto
                    {
                        AircraftName = aircraft.AircraftName,
                        Description = aircraft.Description,
                        Image = aircraft.Image,
                        Categories = categories,
                        Types = types
                    });
                }

                return aircraftsDto;
            }
        }
    }
}
