using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Database.Models;
using WebAPI.Database.Repositories.ResultRepository;
using WebAPI.Database.Repositories.ValueRepositories;
using WebAPI.Database.UnitOfWork;
using WebAPI.DTOs.ResultDTOs;
using WebAPI.DTOs.ValueDTOs;
using WebAPI.Features.CalcStatistic;
using WebAPI.Features.UploadCsv;
using WebAPI.Features.UploadCsv.CsvWorker;
using WebAPI.Validations.Primitives;

namespace WebAPI.UnitTests.UploadCsv
{
    public class UploadCsvTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IResultRepository> _resultsRepoMock;
        private readonly Mock<IValueRepository> _valuesRepoMock;
        private readonly Mock<ICsvHelperService> _csvHelperMock;
        private readonly Mock<IStatisticsCalculatorService> _statisticMock;
        private readonly UploadCsvHandler _handler;

        public UploadCsvTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _csvHelperMock = new Mock<ICsvHelperService>();
            _statisticMock = new Mock<IStatisticsCalculatorService>();
            _resultsRepoMock = new Mock<IResultRepository>();
            _valuesRepoMock = new Mock<IValueRepository>();

            _unitOfWorkMock.Setup(u => u.ResultsRepository).Returns(_resultsRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ValuesRepository).Returns(_valuesRepoMock.Object);

            _handler = new UploadCsvHandler(
                _csvHelperMock.Object,
                _unitOfWorkMock.Object,
                _statisticMock.Object);
        }

        [Fact]
        public async Task Handle_WhenCsvInvalid_ShouldReturnFaultBadRequest()
        {
            var stream = new MemoryStream();
            var command = new UploadCsvCommand("test", stream);

            _csvHelperMock.Setup(c => c.ParseCsvAsync(stream, CancellationToken.None))
                .ReturnsAsync(Result<List<ValueDto>>.Failure(new Fault("BadDataTest")));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error!.Message.Should().Contain("BadRequest");
            result.Error!.Details.Count.Should().Be(1);
            result.Error!.Details.First().Message.Should().Contain("BadDataTest");

            _csvHelperMock.Verify(c => c.ParseCsvAsync(stream, CancellationToken.None), Times.Once);
            _statisticMock.Verify(s => s.CalculateStatistics(It.IsAny<string>(), It.IsAny<List<ValueDto>>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.ResultsRepository, Times.Never);
            _unitOfWorkMock.Verify(u => u.ValuesRepository, Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenInvalidValues_ShouldReturnFaultBadRequest()
        {
            var stream = new MemoryStream();
            var command = new UploadCsvCommand("test", stream);
            var values = new List<ValueDto>();

            _csvHelperMock.Setup(c => c.ParseCsvAsync(stream, CancellationToken.None))
                .ReturnsAsync(Result<List<ValueDto>>.Success(values));

            _statisticMock.Setup(s => s.CalculateStatistics(command.FileName, values))
                .Returns(Result<ResultDto>.Failure(new Fault("BadDataTest")));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error!.Message.Should().Contain("BadRequest");
            result.Error!.Details.Count.Should().Be(1);
            result.Error!.Details.First().Message.Should().Contain("BadDataTest");

            _csvHelperMock.Verify(c => c.ParseCsvAsync(stream, CancellationToken.None), Times.Once);
            _statisticMock.Verify(c => c.CalculateStatistics("test", values), Times.Once);
            _unitOfWorkMock.Verify(u => u.ResultsRepository, Times.Never);
            _unitOfWorkMock.Verify(u => u.ValuesRepository, Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenAllDataValidAndFileDataExist_ShouldRewriteData()
        {
            var stream = new MemoryStream();
            var command = new UploadCsvCommand("test", stream);

            var valuesDto = new List<ValueDto> { new() { Date = DateTime.Now, Value = 1, ExecutionTime = 1 } };
            var resultDto = new ResultDto { FileName = "test" };
            var resultEntity = new ResultEntity() { Id = Guid.NewGuid()};

            _csvHelperMock.Setup(c => c.ParseCsvAsync(stream, CancellationToken.None))
                .ReturnsAsync(Result<List<ValueDto>>.Success(valuesDto));

            _statisticMock.Setup(s => s.CalculateStatistics(resultDto.FileName, valuesDto))
                .Returns(Result<ResultDto>.Success(resultDto));

            _unitOfWorkMock.Setup(u => u.ResultsRepository.GetResultAsync(resultDto.FileName))
                .ReturnsAsync(resultEntity);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            _csvHelperMock.Verify(c => c.ParseCsvAsync(stream, CancellationToken.None), Times.Once);
            _statisticMock.Verify(u => u.CalculateStatistics(resultDto.FileName, valuesDto), Times.Once);
            _unitOfWorkMock.Verify(u => u.ResultsRepository.GetResultAsync(resultDto.FileName), Times.Once);
            _unitOfWorkMock.Verify(u => u.ResultsRepository.DeleteResultAsync(resultEntity.Id), Times.Once);
            _unitOfWorkMock.Verify(u => u.ResultsRepository.AddResultAsync(It.IsAny<ResultEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.ValuesRepository.AddValuesAsync(It.IsAny<List<ValueEntity>>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAllDataValidAndNewFileData_ShouldSaveData()
        {
            var stream = new MemoryStream();
            var command = new UploadCsvCommand("test", stream);

            var valuesDto = new List<ValueDto> { new() { Date = DateTime.Now, Value = 1, ExecutionTime = 1 } };
            var resultDto = new ResultDto { FileName = "test" };
            var resultEntity = new ResultEntity() { Id = Guid.NewGuid() };

            _csvHelperMock.Setup(c => c.ParseCsvAsync(stream, CancellationToken.None))
                .ReturnsAsync(Result<List<ValueDto>>.Success(valuesDto));

            _statisticMock.Setup(s => s.CalculateStatistics(resultDto.FileName, valuesDto))
                .Returns(Result<ResultDto>.Success(resultDto));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            _csvHelperMock.Verify(c => c.ParseCsvAsync(stream, CancellationToken.None), Times.Once);
            _statisticMock.Verify(u => u.CalculateStatistics(resultDto.FileName, valuesDto), Times.Once);
            _unitOfWorkMock.Verify(u => u.ResultsRepository.GetResultAsync(resultDto.FileName), Times.Once);
            _unitOfWorkMock.Verify(u => u.ResultsRepository.DeleteResultAsync(resultEntity.Id), Times.Never);
            _unitOfWorkMock.Verify(u => u.ResultsRepository.AddResultAsync(It.IsAny<ResultEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.ValuesRepository.AddValuesAsync(It.IsAny<List<ValueEntity>>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }
}
