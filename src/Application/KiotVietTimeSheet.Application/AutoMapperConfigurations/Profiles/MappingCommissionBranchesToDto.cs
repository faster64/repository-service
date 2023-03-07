using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingCommissionBranchesToDto : Profile
    {
        //CommissionBranch

        public MappingCommissionBranchesToDto()
        {
            RegisterCommissionBranchesToDto();
        }

        private void RegisterCommissionBranchesToDto()
        {
            CreateMap<CommissionBranch, CommissionBranchDto>();
            CreateMap<CommissionBranch, CommissionBranchDto>().ReverseMap();
        }
    }
}
