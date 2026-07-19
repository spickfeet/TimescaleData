using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs.ValueDTOs;
using WebAPI.Features.CalcStatistic;
using WebAPI.Validations.Primitives;

namespace WebAPI.UnitTests.CalcStatistic
{
    public class StatisticsCalculatorTests
    {
        private readonly IStatisticsCalculatorService _calculator;

        public StatisticsCalculatorTests()
        {
            _calculator = new StatisticsCalculatorService();
        }

        [Fact]
        public void CalculateStatistics_WhenOneCountValues_ShouldReturnCorrectResults()
        {
            var rows = new List<ValueDto>
            {
                new() { Value = 10, ExecutionTime = 1, Date = new DateTime(2001, 1, 1) },
            };

            var result = _calculator.CalculateStatistics("test", rows);

            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Value!.FileName.Should().Be("test");
            result.Value!.TimeDeltaSeconds.Should().Be(0);
            result.Value!.FirstOperationDate.Should().Be(new DateTime(2001, 1, 1));
            result.Value!.AverageExecutionTime.Should().Be(1d);
            result.Value!.AverageValue.Should().Be(10d);
            result.Value!.MedianValue.Should().Be(10d);
            result.Value!.MaxValue.Should().Be(10d);
            result.Value!.MinValue.Should().Be(10d);
        }

        [Fact]
        public void CalculateStatistics_WhenOddCountValues_ShouldReturnCorrectResults()
        {
            var rows = new List<ValueDto>
            {
                new() { Value = 10, ExecutionTime = 1, Date = new DateTime(2001, 1, 1) },
                new() { Value = 20, ExecutionTime = 2, Date = new DateTime(2001, 1, 2) },
                new() { Value = 30, ExecutionTime = 3, Date = new DateTime(2001, 1, 3) }
            };

            var result = _calculator.CalculateStatistics("test", rows);

            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Value!.FileName.Should().Be("test");
            result.Value!.TimeDeltaSeconds.Should().Be((new DateTime(2001, 1, 3) - new DateTime(2001, 1, 1)).TotalSeconds);
            result.Value!.FirstOperationDate.Should().Be(new DateTime(2001, 1, 1));
            result.Value!.AverageExecutionTime.Should().Be(2d);
            result.Value!.AverageValue.Should().Be(20d);
            result.Value!.MedianValue.Should().Be(20d);
            result.Value!.MaxValue.Should().Be(30d);
            result.Value!.MinValue.Should().Be(10d);
        }

        [Fact]
        public void CalculateStatistics_WhenEvenCountValues_ShouldReturnCorrectResults()
        {
            var rows = new List<ValueDto>
            {
                new() { Value = 10, ExecutionTime = 1, Date = new DateTime(2001, 1, 1) },
                new() { Value = 20, ExecutionTime = 2, Date = new DateTime(2001, 1, 2) },
                new() { Value = 30, ExecutionTime = 3, Date = new DateTime(2001, 1, 3) },
                new() { Value = 40, ExecutionTime = 4, Date = new DateTime(2001, 1, 4) }
            };

            var result = _calculator.CalculateStatistics("test", rows);

            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Value!.FileName.Should().Be("test");
            result.Value!.TimeDeltaSeconds.Should().Be((new DateTime(2001, 1, 4) - new DateTime(2001, 1, 1)).TotalSeconds);
            result.Value!.FirstOperationDate.Should().Be(new DateTime(2001, 1, 1));
            result.Value!.AverageExecutionTime.Should().Be(2.5d);
            result.Value!.AverageValue.Should().Be(25d);
            result.Value!.MedianValue.Should().Be(25d);
            result.Value!.MaxValue.Should().Be(40d);
            result.Value!.MinValue.Should().Be(10d);
        }

        [Fact]
        public void CalculateStatistics_WhenNullValues_ShouldReturnFaultInvalidValues()
        {
            var result = _calculator.CalculateStatistics("test", null);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
            result.Error.Error.Should().Be("InvalidValues");
            result.Error.Details.Count.Should().Be(1);
            result.Error.Details.First().Error.Should().Be("Список Values не может быть пустым");
        }

        [Fact]
        public void CalculateStatistics_WhenEmptyValues_ShouldReturnFaultInvalidValues()
        {
            var rows = new List<ValueDto>();

            var result = _calculator.CalculateStatistics("test", rows);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
            result.Error.Error.Should().Be("InvalidValues");
            result.Error.Details.Count.Should().Be(1);
            result.Error.Details.First().Error.Should().Be("Список Values не может быть пустым");
        }
    }
}
