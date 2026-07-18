using WebAPI.Database.Models;
using WebAPI.DTOs.ResultDTOs;

namespace WebAPI.Database.Repositories.ResultRepository
{
    public interface IResultRepository
    {
        Task AddResultAsync(ResultEntity result);
        Task DeleteResultAsync(Guid id);
        Task<ResultEntity?> GetResultAsync(string fileName);
        Task<List<ResultEntity>> GetResultsAsync(ResultFilter? resultFilter = null);
    }
}
