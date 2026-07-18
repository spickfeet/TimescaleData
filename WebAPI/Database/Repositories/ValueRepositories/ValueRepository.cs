using Microsoft.EntityFrameworkCore;
using WebAPI.Database.Extensions;
using WebAPI.Database.Models;
using WebAPI.DTOs.PageDTOs;
using WebAPI.DTOs.SortDTOs;
using WebAPI.DTOs.ValueDTOs;

namespace WebAPI.Database.Repositories.ValueRepositories
{
    public class ValueRepository : IValueRepository
    {
        private readonly TimescaleDbContext _context;
        public ValueRepository(TimescaleDbContext context) 
        {
            _context = context;
        }
        public async Task AddValuesAsync(List<ValueEntity> values)
        {
            await _context.Values.AddRangeAsync(values);
        }

        public async Task<PagedResult<ValueEntity>> GetPagedValuesAsync(PageParams? pageParams = null, ValueFilter? valueFilter = null, SortParams? sortParams = null)
        {
            return await _context.Values.AsNoTracking().Include(v => v.Result).Filter(valueFilter).Sort(sortParams).ToPagedResultAsync(pageParams);
        }
    }
}
