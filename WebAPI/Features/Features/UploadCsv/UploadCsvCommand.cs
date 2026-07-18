using MediatR;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.Features.UploadCsv
{
    public record UploadCsvCommand(string FileName, Stream FileStream) : IRequest<Result>;
}
