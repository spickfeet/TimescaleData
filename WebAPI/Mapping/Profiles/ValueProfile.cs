using AutoMapper;
using WebAPI.Database.Models;
using WebAPI.DTOs.ResultDTOs;
using WebAPI.DTOs.ValueDTOs;

namespace WebAPI.Mapping.Profiles
{
    public class ValueProfile : Profile
    {
        public ValueProfile()
        {
            CreateMap<ValueEntity, ValueDtoResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Result.FileName))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.ExecutionTime, opt => opt.MapFrom(src => src.ExecutionTime))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
        }
    }
}
