using WebAPI.Database.Repositories.ResultRepository;
using WebAPI.Database.Repositories.ValueRepositories;

namespace WebAPI.Database.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IResultRepository ResultsRepository { get; }
        IValueRepository ValuesRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
