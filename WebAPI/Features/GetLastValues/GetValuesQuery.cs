using MediatR;
using WebAPI.DTOs.ValueDTOs;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.GetLastValues
{
    public record GetValuesQuery(string fileName) : IRequest<Result<List<ValueDtoResponse>>>;
}
