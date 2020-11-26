using Application.Aircrafts;
using Application.Aircrafts.Dto;
using MediatR;
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
        [HttpPost("{id}/subscribe")]
        [Authorize]
        public async Task<ActionResult<Unit>> Subscribe(int id)
        {
            return await Mediator.Send(new Subscribe.Command { AircraftId = id });
        }

        [HttpDelete("{id}/subscribe")]
        [Authorize]
        public async Task<ActionResult<Unit>> Unsubscribe(int id)
        {
            return await Mediator.Send(new Unsubscribe.Command { AircraftId = id });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AircraftDto>> Add([FromForm] Add.Command command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost("{id}/image")]
        [Authorize]
        public async Task<ActionResult<Unit>> AddImage(int id, [FromForm] AddImage.Command command)
        {
            command.AircraftId = id;
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<AircraftDto>> Edit(int id, Edit.Command command, CancellationToken ct)
        {
            command.Id = id;
            return await Mediator.Send(command, ct);
        }

        [HttpGet("categories")]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> Categories()
        {
            return await Mediator.Send(new Categories.Query());
        }

        [HttpGet("types")]
        [Authorize]
        public async Task<ActionResult<TypeDto>> Types()
        {
            return await Mediator.Send(new Types.Query());
        }
    }
}
