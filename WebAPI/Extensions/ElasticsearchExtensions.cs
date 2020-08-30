using Application.Aircrafts.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace SearchApp.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["Elasticsearch:Url"];
            var defaultIndex = configuration["Elasticsearch:DefaultIndex"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex);
            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton(client);

            CreateIndex(client, defaultIndex);

        }

        private static void CreateIndex(ElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName,
                index => index.Map<AircraftDto>(d => d.Properties(p => p.Text(p => p.Name("aircraftName")))));
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings.
                DefaultMappingFor<AircraftDto>(m => m
                    .IdProperty(a => a.Id));

        }
    }
}
