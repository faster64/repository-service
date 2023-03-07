using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingClockingPenalizeToDto : Profile
    {
        public MappingClockingPenalizeToDto()
        {
            CreateMap<ClockingPenalize, ClockingPenalizeDto>();
            CreateMap<ClockingPenalize, ClockingPenalizeDto>()
                .ReverseMap();
        }
    }
}