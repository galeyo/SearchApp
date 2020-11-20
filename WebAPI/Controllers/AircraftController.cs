using Application.Aircrafts;
using Application.Aircrafts.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApp.Controllers
{
    public class AircraftController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<AircraftEnvelope>> List([FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await Mediator.Send(new List.Query(limit, offset));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AircraftDto>> Details(int id, CancellationToken ct)
        {
            return await Mediator.Send(new Details.Query { Id = id }, ct);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AircraftDto>> Add([FromForm] Add.Command command)
        {
            return await Mediator.Send(command);
        }
    }
}
