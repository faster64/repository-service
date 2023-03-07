using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    class MappingCommissionDetailToCommissionDetailDto : Profile
    {
        public MappingCommissionDetailToCommissionDetailDto()
        {
            RegisterCommissionDetailToCommissionDetailDto();
        }

        private void RegisterCommissionDetailToCommissionDetailDto()
        {
            CreateMap<CommissionDetail, CommissionDetailDto>();
            CreateMap<CommissionDetail, CommissionDetailDto>().ReverseMap();
        }
    }
}
