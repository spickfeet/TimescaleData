using AutoMapper;
using MediatR;
using System.Collections.Generic;
using WebAPI.Database.Repositories.ResultRepository;
using WebAPI.DTOs.ResultDTOs;
using WebAPI.Features.Features.UploadCsv;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.Features.GetResults
{
    public class GetResultsHandler : IRequestHandler<GetResultsQuery, Result<List<ResultDtoResponse>>>
    {
        private readonly IResultRepository _resultRepository;
        private readonly IMapper _mapper;
        public GetResultsHandler(IResultRepository resultRepository, IMapper mapper)
        {
            _resultRepository = resultRepository;
            _mapper = mapper;
        }
        public async Task<Result<List<ResultDtoResponse>>> Handle(GetResultsQuery query, CancellationToken cancellationToken)
        {
            return Result<List<ResultDtoResponse>>.Success(_mapper.Map<List<ResultDtoResponse>>(await _resultRepository.GetResultsAsync(query.ResultFilter)));
        }
    }
}
