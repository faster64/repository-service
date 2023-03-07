using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingAllowanceToDto : Profile
    {
        public MappingAllowanceToDto()
        {
            CreateMap<Allowance, AllowanceDto>();
            CreateMap<Allowance, AllowanceDto>()
                .ReverseMap();

        }
    }
}
