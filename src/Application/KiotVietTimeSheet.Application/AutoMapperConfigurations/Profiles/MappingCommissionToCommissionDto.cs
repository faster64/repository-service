using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingCommissionToCommissionDto : Profile
    {
        public MappingCommissionToCommissionDto()
        {
            RegisterCommissionToCommissionDto();
        }

        private void RegisterCommissionToCommissionDto()
        {
            CreateMap<Commission, CommissionDto>()
                .ForMember(dto => dto.CommissionDetails, opt => opt.MapFrom(x => x.CommissionDetails))
                .ForMember(dto => dto.CommissionBranches, opt => opt.MapFrom(x => x.CommissionBranches));

            CreateMap<Commission, CommissionDto>()
                .ForMember(dto => dto.CommissionDetails, opt => opt.MapFrom(x => x.CommissionDetails))
                .ForMember(dto => dto.CommissionBranches, opt => opt.MapFrom(x => x.CommissionBranches))
                .ReverseMap();
        }
    }
}
