using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebAPI.Database.Extensions;
using WebAPI.Database.Models;
using WebAPI.DTOs.ResultDTOs;
using WebAPI.Exceptions;

namespace WebAPI.Database.Repositories.ResultRepository
{
    public class ResultRepository : IResultRepository
    {
        private readonly TimescaleDbContext _context;
        public ResultRepository(TimescaleDbContext context) 
        {
            _context = context;
        }
        public async Task AddResultAsync(ResultEntity result)
        {
            await _context.Results.AddAsync(result);
        }

        public async Task DeleteResultAsync(Guid id)
        {
            var entity = await _context.Results.FindAsync(id) ?? throw new EntityNotFoundException("ResultsEntity", id);
            _context.Results.Remove(entity);
        }

        public async Task<List<ResultEntity>> GetResultsAsync(ResultFilter? resultFilter = null)
        {
            return await _context.Results.AsNoTracking().Filter(resultFilter).ToListAsync();
        }
        public async Task<ResultEntity?> GetResultAsync(string fileName)
        {
            return await _context.Results.AsNoTracking().FirstOrDefaultAsync(r => r.FileName == fileName);
        }
    }
}
