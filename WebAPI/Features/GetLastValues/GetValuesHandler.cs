using AutoMapper;
using MediatR;
using WebAPI.Database.Repositories.ValueRepositories;
using WebAPI.DTOs.PageDTOs;
using WebAPI.DTOs.SortDTOs;
using WebAPI.DTOs.ValueDTOs;
using WebAPI.Validations.Primitives;

namespace WebAPI.Features.GetLastValues
{
    public class GetValuesHandler : IRequestHandler<GetValuesQuery, Result<List<ValueDtoResponse>>>
    {
        private readonly IValueRepository _valueRepository;
        private readonly IMapper _mapper;
        public GetValuesHandler(IValueRepository valueRepository, IMapper mapper)
        {
            _valueRepository = valueRepository;
            _mapper = mapper;
        }
        public async Task<Result<List<ValueDtoResponse>>> Handle(GetValuesQuery query, CancellationToken cancellationToken)
        {
            var pageParams = new PageParams()
            {
                Page = 1,
                PageSize = 10
            };
            var sortParams = new SortParams()
            {
                OrderBy = "date",
                IsDescending = true
            };

            var valueFilter = new ValueFilter()
            {
                FileName = query.fileName
            };
            var page = await _valueRepository.GetPagedValuesAsync(pageParams, valueFilter, sortParams);
            return Result<List<ValueDtoResponse>>.Success(_mapper.Map<List<ValueDtoResponse>>(page.Data));
        }
    }
}
