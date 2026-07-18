using WebAPI.Database.Models;
using WebAPI.DTOs.ResultDTOs;

namespace WebAPI.Database.Extensions
{
    public static class ResultEntityExtensions
    {
        public static IQueryable<ResultEntity> Filter(this IQueryable<ResultEntity> query, ResultFilter? resultFilter)
        {
            if (resultFilter == null) return query;

            string? name = resultFilter.FileName?.Trim();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(r => r.FileName.Contains(name));

            if (resultFilter.FirstOperationDateAfter != null)
                query = query.Where(r => r.FirstOperationDate >= resultFilter.FirstOperationDateAfter);

            if (resultFilter.FirstOperationDateBefore != null)
                query = query.Where(r => r.FirstOperationDate <= resultFilter.FirstOperationDateBefore);

            if (resultFilter.MinAverageValue != null)
                query = query.Where(r => r.AverageValue >= resultFilter.MinAverageValue);

            if (resultFilter.MaxAverageValue != null)
                query = query.Where(r => r.AverageValue <= resultFilter.MaxAverageValue);

            if (resultFilter.MinAverageExecutionTime != null)
                query = query.Where(r => r.AverageExecutionTime >= resultFilter.MinAverageExecutionTime);

            if (resultFilter.MaxAverageExecutionTime != null)
                query = query.Where(r => r.AverageExecutionTime <= resultFilter.MaxAverageExecutionTime);

            return query;
        }
    }
}
