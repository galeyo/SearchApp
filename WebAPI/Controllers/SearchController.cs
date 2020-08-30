using Application.Aircrafts.Dto;
using Application.AircraftSearch;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SearchApp.Controllers
{
    public class SearchController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<AircraftEnvelope>> Search(
            [FromQuery] string category,
            [FromQuery] string type,
            [FromQuery] int? year,
            [FromBody] string query)
        {
            return await Mediator.Send(new Search.Query(category, type, year, query));
        }

        [HttpPut("reindex")]
        public async Task<Unit> ReIndex()
        {
            return await Mediator.Send(new ReIndex.Command());
        }
    }
}
