using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Elasticsearch
{
    public class ElasticUtils<T>: IElasticUtils where T : class
    {
        private readonly ElasticClient client;
        private readonly ILogger<ElasticUtils<T>> logger;
        private List<T> _cache = new List<T>();

        public ElasticUtils(ElasticClient client, ILogger<ElasticUtils<T>> logger)
        {
            this.client = client;
            this.logger = logger;
        }
        public async Task SaveSingleAsync<T>(T index)
        {
            if (_cache.Any)
        }
    }
}
