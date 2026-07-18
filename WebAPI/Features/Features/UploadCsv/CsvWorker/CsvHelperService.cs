using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using WebAPI.DTOs.ValueDTOs;
using WebAPI.Validations.Primitives;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Features.Features.UploadCsv.CsvWorker
{
    public  class CsvHelperService : ICsvHelperService
    {
        private static readonly DateTime MinDate = new DateTime(2000, 1, 1);
        private const int MaxRows = 10000;

        public async Task<Result<List<ValueDto>>> ParseCsvAsync(Stream csvStream, CancellationToken cancellationToken = default)
        {
            var rows = new List<ValueDto>();
            var errors = new List<Fault>();

            csvStream.Position = 0;
            using var reader = new StreamReader(csvStream);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                
            };

            using var csv = new CsvReader(reader, config);
            int lineNumber = 1;
            try
            {
                await foreach (var record in csv.GetRecordsAsync<ValueDto>(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    lineNumber++;

                    if (rows.Count >= MaxRows)
                    {
                        errors.Add(new Fault($"Превышен лимит строк: {MaxRows}"));
                        break;
                    }

                    record.Date = DateTime.SpecifyKind(record.Date, DateTimeKind.Utc);
                    var recordErrors = ValidateRecord(record, lineNumber);
                    
                    errors.AddRange(recordErrors);

                    rows.Add(record);
                }

                if (errors.Any())
                {
                    var fault = new Fault("InvalidCsvContent");
                    fault.Details.AddRange(errors);
                    return Result<List<ValueDto>>.Failure(fault);
                }

                if (rows.Count == 0)
                {
                    return Result<List<ValueDto>>.Failure(new Fault("Файл пуст"));
                }

                return Result<List<ValueDto>>.Success(rows);
            }
            catch (OperationCanceledException)
            {
                var fault = new Fault("InvalidCsvContent");
                fault.Details.AddRange(errors);
                fault.Details.Add(new Fault("Операция чтения была отменена"));
                return Result<List<ValueDto>>.Failure(fault);
            }
            catch (Exception ex)
            {
                var fault = new Fault("InvalidCsvContent");
                fault.Details.AddRange(errors);
                fault.Details.Add(new Fault($"Ошибка чтения файла строка: {lineNumber}. Ошибка: {ex.Message}"));
                return Result<List<ValueDto>>.Failure(fault);
            }
        }

        private List<Fault> ValidateRecord(ValueDto record, int lineNumber)
        {
            var errors = new List<Fault>();

            if (record.Date < MinDate)
            {
                errors.Add(new Fault($"Строка {lineNumber}: Дата раньше 01.01.2000"));
            }

            if (record.Date > DateTime.UtcNow)
            {
                errors.Add(new Fault($"Строка {lineNumber}: Дата позже текущей"));
            }

            if (record.ExecutionTime < 0)
            {
                errors.Add(new Fault($"Строка {lineNumber}: Время выполнения не может быть меньше 0"));
            }

            if (record.Value < 0)
            {
                errors.Add(new Fault($"Строка {lineNumber}: Значение не может быть меньше 0"));
            }

            return errors;
        }
    }
}

