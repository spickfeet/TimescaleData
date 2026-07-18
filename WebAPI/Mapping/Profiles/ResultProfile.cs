using AutoMapper;
using WebAPI.Database.Models;
using WebAPI.DTOs.ResultDTOs;

namespace WebAPI.Mapping.Profiles
{
    public class ResultProfile : Profile
    {
        public ResultProfile()
        {
            CreateMap<ResultEntity, ResultDtoResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.TimeDeltaSeconds, opt => opt.MapFrom(src => src.TimeDeltaSeconds))
                .ForMember(dest => dest.FirstOperationDate, opt => opt.MapFrom(src => src.FirstOperationDate))
                .ForMember(dest => dest.AverageExecutionTime, opt => opt.MapFrom(src => src.AverageExecutionTime))
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.AverageValue))
                .ForMember(dest => dest.MedianValue, opt => opt.MapFrom(src => src.MedianValue))
                .ForMember(dest => dest.MaxValue, opt => opt.MapFrom(src => src.MaxValue))
                .ForMember(dest => dest.MinValue, opt => opt.MapFrom(src => src.MinValue));
        }
    }
}
