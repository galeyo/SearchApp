using Application.Aircrafts.Dto;
using MediatR;
using Nest;
using System.Threading;
using System.Threading.Tasks;

namespace Application.AircraftSearch
{
    public class Search
    {
        public class Query : MediatR.IRequest<AircraftEnvelope>
        {
            public Query(string category, string type, int? year, string queryString)
            {
                Category = category;
                Type = type;
                Year = year;
                QueryString = queryString;
            }

            public string Category { get; }
            public string Type { get; }
            public int? Year { get; }
            public string QueryString { get; }
        }

        public class Handler : IRequestHandler<Query, AircraftEnvelope>
        {
            private readonly ElasticClient client;

            public Handler(ElasticClient client)
            {
                this.client = client;
            }
            public async Task<AircraftEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                return new AircraftEnvelope();
            }
        }
    }
}
