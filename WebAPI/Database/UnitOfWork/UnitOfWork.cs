using System;
using WebAPI.Database.Repositories.ResultRepository;
using WebAPI.Database.Repositories.ValueRepositories;

namespace WebAPI.Database.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TimescaleDbContext _context;
        private IResultRepository _resultRepository;
        private IValueRepository _valueRepository;
        public IResultRepository ResultsRepository => _resultRepository;
        public IValueRepository ValuesRepository => _valueRepository;

        public UnitOfWork(TimescaleDbContext context, IResultRepository resultRepository, IValueRepository valueRepository)
        {
            _context = context;
            _resultRepository = resultRepository;
            _valueRepository = valueRepository;
        }


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
