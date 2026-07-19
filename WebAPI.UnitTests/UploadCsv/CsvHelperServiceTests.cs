using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Features.UploadCsv.CsvWorker;
using WebAPI.Validations.Primitives;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.UnitTests.UploadCsv
{
    public class CsvHelperServiceTests
    {
        private readonly ICsvHelperService _service;

        public CsvHelperServiceTests()
        {
            _service = new CsvHelperService();
        }

        private static Stream CreateStream(string csvContent)
        {
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            return new MemoryStream(bytes);
        }

        [Fact]
        public async Task ParseCsvAsync_WhenCsvIsValid_ShouldReturnSuccess()
        {
            var csv = """
                Date;ExecutionTime;Value
                2024-01-01T00:00:00.0000Z;1.5;100.0
                2024-01-02T00:00:00.0000Z;2.0;200.0
                """;
            var stream = CreateStream(csv);

            var result = await _service.ParseCsvAsync(stream);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Count.Should().Be(2);

            var firstRow = result.Value![0];
            firstRow.Date.Kind.Should().Be(DateTimeKind.Utc);
            firstRow.Date.Should().Be(new DateTime(2024, 1, 1));
            firstRow.ExecutionTime.Should().Be(1.5d);
            firstRow.Value.Should().Be(100d);

            var secondRow = result.Value![1];
            secondRow.Date.Kind.Should().Be(DateTimeKind.Utc);
            secondRow.Date.Should().Be(new DateTime(2024, 1, 2));
            secondRow.ExecutionTime.Should().Be(2d);
            secondRow.Value.Should().Be(200d);
        }

        [Fact]
        public async Task ParseCsvAsync_WhenFileIsEmpty_ShouldReturnFaultInvalidCsvContent()
        {
            var csv = "Date;ExecutionTime;Value";
            var stream = CreateStream(csv);

            var result = await _service.ParseCsvAsync(stream);

            result.IsFailure.Should().BeTrue();
            result.Error!.Message.Should().Be("InvalidCsvContent");
            result.Error!.Details.Count.Should().Be(1);
            result.Error!.Details.First().Message.Should().Be("Файл пуст");
        }

        [Fact]
        public async Task ParseCsvAsync_WhenDateIsTooOld_ShouldReturnFaultInvalidCsvContent()
        {
            var csv = """
                Date;ExecutionTime;Value
                1999-01-01T00:00:00.0000Z;1.0;1.0
                """;
            var stream = CreateStream(csv);

            var result = await _service.ParseCsvAsync(stream);

            result.IsFailure.Should().BeTrue();
            result.Error!.Message.Should().Be("InvalidCsvContent");
            result.Error!.Details.Count.Should().Be(1);
            result.Error!.Details.First().Message.Should().Contain("Дата раньше 01.01.2000");
        }

        [Fact]
        public async Task ParseCsvAsync_WhenDateIsInFuture_ShouldReturnFaultInvalidCsvContent()
        {
            var futureDate = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-ddTHH:mm:ss.ffffZ");
            var csv = $"""
                Date;ExecutionTime;Value
                {futureDate};1.0;1.0
                """;
            var stream = CreateStream(csv);

            var result = await _service.ParseCsvAsync(stream);

            result.IsFailure.Should().BeTrue();
            result.Error!.Message.Should().Be("InvalidCsvContent");
            result.Error!.Details.Count.Should().Be(1);
            result.Error!.Details.First().Message.Should().Contain("Дата позже текущей");
        }

        [Fact]
        public async Task ParseCsvAsync_WhenValueAndExecutionTimeNegative_ShouldReturnFaultInvalidCsvContent()
        {
            var csv = """
                Date;ExecutionTime;Value
                2024-01-01T10:00:00.0000Z;-1.0;1.0
                2024-01-01T11:00:00.0000Z;1.0;-1.0
                """;
            var stream = CreateStream(csv);

            var result = await _service.ParseCsvAsync(stream);

            result.IsFailure.Should().BeTrue();
            result.Error!.Message.Should().Be("InvalidCsvContent");
            result.Error!.Details.Count.Should().Be(2);
            result.Error!.Details.Should().Contain(f => f.Message.Contains("Время выполнения не может быть меньше 0"));
            result.Error!.Details.Should().Contain(f => f.Message.Contains("Значение не может быть меньше 0"));
        }

        [Fact]
        public async Task ParseCsvAsync_WhenRowsExceedLimit_ShouldReturnFaultInvalidCsvContent()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Date;ExecutionTime;Value");
            for (int i = 0; i < 10001; i++)
            {
                sb.AppendLine($"2024-01-01T10:00:00.0000Z;1.0;1.0");
            }
            var stream = CreateStream(sb.ToString());

            var result = await _service.ParseCsvAsync(stream);

            result.IsFailure.Should().BeTrue();
            result.Error!.Message.Should().Be("InvalidCsvContent");
            result.Error!.Details.Count.Should().Be(1);
            result.Error!.Details.First().Message.Should().Contain("Превышен лимит строк");
        }

        [Fact]
        public async Task ParseCsvAsync_WhenCancelCancellation_ShouldReturnFaultOperationCancelled()
        {
            var csv = """
                Date;ExecutionTime;Value
                2024-01-01T00:00:00.0000Z;1.5;100.0
                2024-01-02T00:00:00.0000Z;2.0;200.0
                """;
            var stream = CreateStream(csv);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await _service.ParseCsvAsync(stream, cts.Token);

            result.IsFailure.Should().BeTrue();
            result.Error!.Message.Should().Be("OperationCancelled");
            result.Error!.Details.Count.Should().Be(1);
            result.Error!.Details.First().Message.Should().Contain("Операция чтения была отменена");
        }
    }
}
