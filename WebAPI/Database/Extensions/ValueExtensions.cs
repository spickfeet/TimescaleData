using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI.Database.Models;
using WebAPI.DTOs.PageDTOs;
using WebAPI.DTOs.ResultDTOs;
using WebAPI.DTOs.SortDTOs;
using WebAPI.DTOs.ValueDTOs;

namespace WebAPI.Database.Extensions
{
    public static class ValueExtensions
    {
        public static IQueryable<ValueEntity> Filter(this IQueryable<ValueEntity> query, ValueFilter? valueFilter)
        {
            if (valueFilter == null) return query;

            if (!string.IsNullOrWhiteSpace(valueFilter.FileName))
                query = query.Where(v => v.Result.FileName == valueFilter.FileName);

            return query;
        }

        public static IQueryable<ValueEntity> Sort(this IQueryable<ValueEntity> query, SortParams? sortParams)
        {
            if (sortParams == null) return query;

            if (sortParams.IsDescending != null && sortParams.IsDescending == true)
            {
                return query.OrderByDescending(GetKeySelector(sortParams.OrderBy));
            }
            return query.OrderBy(GetKeySelector(sortParams.OrderBy));
        }

        private static Expression<Func<ValueEntity, object>> GetKeySelector(string? orderBy)
        {
            if (orderBy == null)
            {
                return v => v.Date;
            }
            return orderBy.ToLower() switch
            {
                "filename" => v => v.Result.FileName,
                "date" => v => v.Date,
                "executiontime" => v => v.ExecutionTime,
                "value" => e => e.Value,
                _ => v => v.Date,
            };
        }

        public static async Task<PagedResult<ValueEntity>> ToPagedResultAsync(this IQueryable<ValueEntity> query, PageParams? pageParams = null)
        {
            if (pageParams == null) pageParams = new();

            int page = Math.Max(1, pageParams.Page ?? 1);
            int pageSize = Math.Min(Math.Max(1, pageParams.PageSize ?? 10), 20);

            int totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return new PagedResult<ValueEntity>(new List<ValueEntity>(), pageSize, 1, totalCount);
            }

            List<ValueEntity> data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<ValueEntity>(data, pageSize, page, totalCount);
        }
    }
}
