using Application.Aircrafts;
using Application.Aircrafts.Dto;
using Microsoft.AspNetCore.Mvc;
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

    }
}
