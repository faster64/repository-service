using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingHolidayToDto : Profile
    {
        public MappingHolidayToDto()
        {
            CreateMap<Holiday, HolidayDto>();
            CreateMap<Holiday, HolidayDto>().ReverseMap();
        }
    }
}
