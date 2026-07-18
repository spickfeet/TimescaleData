using WebAPI.DTOs.ResultDTOs;
using WebAPI.DTOs.ValueDTOs;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.Features.CalcStatistic
{
    public class StatisticsCalculatorService : IStatisticsCalculatorService
    {
        public Result<ResultDto> CalculateStatistics(string fileName, List<ValueDto> values)
        {
            if (values == null || !values.Any())
            {
                var error = new Fault("InvalidValues");
                error.Details.Add(new("Список Values не может быть пустым"));
                Result.Failure(error);
            }

            var minDate = values.Min(r => r.Date);
            var maxDate = values.Max(r => r.Date);
            var timeDelta = (maxDate - minDate).TotalSeconds;

            var averageExecutionTime = values.Average(v => v.ExecutionTime);
            var averageValue = values.Average(v => v.Value);

            var sortedValues = values.Select(v => v.Value).OrderBy(v => v).ToList();
            var medianValue = CalculateMedian(sortedValues);

            var maxValue = values.Max(r => r.Value);
            var minValue = values.Min(r => r.Value);

            return Result<ResultDto>.Success(new ResultDto
            {
                FileName = fileName,
                TimeDeltaSeconds = timeDelta,
                FirstOperationDate = minDate,
                AverageExecutionTime = averageExecutionTime,
                AverageValue = averageValue,
                MedianValue = medianValue,
                MaxValue = maxValue,
                MinValue = minValue
            });
        }

        private double CalculateMedian(List<double> sortedValues)
        {
            int count = sortedValues.Count;

            if (count % 2 == 0)
            {
                return (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2.0;
            }
            else
            {
                return sortedValues[count / 2];
            }
        }
    }
}
