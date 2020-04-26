using Application.Aircrafts;
using Application.Aircrafts.Dto;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApp.Controllers
{
    public class AircraftController : BaseController
    {
        [HttpGet]
        public async Task<ICollection<AircraftDto>> List()
        {
            var aircrafts = await Mediator.Send(new List.Query());
            return aircrafts;
        }
    }
}
