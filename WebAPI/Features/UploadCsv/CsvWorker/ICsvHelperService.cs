using WebAPI.DTOs.ValueDTOs;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.UploadCsv.CsvWorker
{
    public interface ICsvHelperService
    {
        Task<Result<List<ValueDto>>> ParseCsvAsync(Stream csvStream, CancellationToken cancellationToken = default);
    }
}
