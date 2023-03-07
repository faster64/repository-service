using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingClockingToPayslipClockingDto : Profile
    {
        public MappingClockingToPayslipClockingDto()
        {
            RegisterClockingToPayslipClockingDto();
        }

        private void RegisterClockingToPayslipClockingDto()
        {
            CreateMap<Clocking, PayslipClockingDto>();

        }
    }
}
