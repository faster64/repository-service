using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingPayslipClockingPenalizeToDto : Profile
    {
        public MappingPayslipClockingPenalizeToDto()
        {
            CreateMap<PayslipClockingPenalize, PayslipClockingPenalizeDto>();
            CreateMap<PayslipClockingPenalize, PayslipClockingPenalizeDto>().ReverseMap();
        }
    }
}