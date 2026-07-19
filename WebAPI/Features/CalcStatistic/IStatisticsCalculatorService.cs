using WebAPI.DTOs.ResultDTOs;
using WebAPI.DTOs.ValueDTOs;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.CalcStatistic
{
    public interface IStatisticsCalculatorService
    {
        Result<ResultDto> CalculateStatistics(string fileName, List<ValueDto> rows);
    }
}
