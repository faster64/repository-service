using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingClockingToClockingDto : Profile
    {
        public MappingClockingToClockingDto()
        {
            RegisterClockingToClockingDto();
        }

        private void RegisterClockingToClockingDto()
        {
            CreateMap<Clocking, ClockingDto>()
                .ForMember(d => d.ClockingPenalizesDto, opt => opt.MapFrom(x => x.ClockingPenalizes));

            CreateMap<Clocking, ClockingDto>()
                .ForMember(d => d.ClockingPenalizesDto, opt => opt.MapFrom(x => x.ClockingPenalizes))
                .ReverseMap();
        }
    }
}
