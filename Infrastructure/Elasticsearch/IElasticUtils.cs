using System.Threading.Tasks;

namespace Infrastructure.Elasticsearch
{
    public interface IElasticUtils
    {
        Task SaveSingleAsync<T>(T index);
    }
}