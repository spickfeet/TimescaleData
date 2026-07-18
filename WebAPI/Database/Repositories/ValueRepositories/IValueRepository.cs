using WebAPI.Database.Models;
using WebAPI.DTOs.PageDTOs;
using WebAPI.DTOs.SortDTOs;
using WebAPI.DTOs.ValueDTOs;

namespace WebAPI.Database.Repositories.ValueRepositories
{
    public interface IValueRepository
    {
        Task AddValuesAsync(List<ValueEntity> values);
        Task<PagedResult<ValueEntity>> GetPagedValuesAsync(PageParams? pageParams = null, ValueFilter? valueFilter = null, SortParams? sortParams = null);
    }
}
