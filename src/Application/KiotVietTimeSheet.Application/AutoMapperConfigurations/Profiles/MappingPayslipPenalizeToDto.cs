using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingPayslipPenalizeToDto : Profile
    {
        public MappingPayslipPenalizeToDto()
        {
            RegisterMappingPayslipPenalizeToDto();
        }

        private void RegisterMappingPayslipPenalizeToDto()
        {
            CreateMap<PayslipPenalize, PayslipPenalizeDto>();
            CreateMap<PayslipPenalize, PayslipPenalizeDto>().ReverseMap();
        }
    }
}
