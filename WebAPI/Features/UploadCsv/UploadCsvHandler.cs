using MediatR;
using WebAPI.Database.Models;
using WebAPI.Database.UnitOfWork;
using WebAPI.Features.CalcStatistic;
using WebAPI.Features.UploadCsv.CsvWorker;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.UploadCsv
{
    public class UploadCsvHandler : IRequestHandler<UploadCsvCommand, Result>
    {
        private ICsvHelperService _csvHelperService;
        private IUnitOfWork _unitOfWork;
        private IStatisticsCalculatorService _statisticsCalculator;
        public UploadCsvHandler(
            ICsvHelperService csvHelperService,
            IUnitOfWork unitOfWork,
            IStatisticsCalculatorService statisticsCalculator)
        {
            _csvHelperService = csvHelperService;
            _unitOfWork = unitOfWork;
            _statisticsCalculator = statisticsCalculator;
        }
        public async Task<Result> Handle(UploadCsvCommand command, CancellationToken cancellationToken)
        {
            var resultValues = await _csvHelperService.ParseCsvAsync(command.FileStream, cancellationToken);

            if (resultValues.IsFailure)
            {
                var error = new Fault("BadRequest");
                error.Details.Add(resultValues.Error!);
                return Result.Failure(error);
            }

            var valueDtos = resultValues.Value;

            var resultStatistics = _statisticsCalculator.CalculateStatistics(command.FileName, resultValues.Value!);

            if (resultStatistics.IsFailure)
            {
                var error = new Fault("BadRequest");
                error.Details.Add(resultStatistics.Error!);
                return Result.Failure(error);
            }

            var resultDto = resultStatistics.Value;

            var existingResult = await _unitOfWork.ResultsRepository.GetResultAsync(command.FileName);
            if (existingResult != null)
            {
                await _unitOfWork.ResultsRepository.DeleteResultAsync(existingResult.Id);
            }

            var resultId = Guid.NewGuid();
            await _unitOfWork.ResultsRepository.AddResultAsync(
                new ResultEntity
                {
                    Id = resultId,
                    FileName = resultDto!.FileName,
                    TimeDeltaSeconds = resultDto.TimeDeltaSeconds,
                    FirstOperationDate = resultDto.FirstOperationDate,
                    AverageExecutionTime = resultDto.AverageExecutionTime,
                    AverageValue = resultDto.AverageValue,
                    MedianValue = resultDto.MedianValue,
                    MaxValue = resultDto.MaxValue,
                    MinValue = resultDto.MinValue
                });

            var valueEntities = valueDtos!.Select(dto
                => new ValueEntity
                {
                    Id = Guid.NewGuid(),
                    Date = dto.Date,
                    ExecutionTime = dto.ExecutionTime,
                    Value = dto.Value,
                    ResultId = resultId
                }).ToList();

            await _unitOfWork.ValuesRepository.AddValuesAsync(valueEntities);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}