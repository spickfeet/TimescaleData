using MediatR;
using WebAPI.DTOs.ResultDTOs;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.GetResults
{
    public record GetResultsQuery(ResultFilter ResultFilter) : IRequest<Result<List<ResultDtoResponse>>>;
}
