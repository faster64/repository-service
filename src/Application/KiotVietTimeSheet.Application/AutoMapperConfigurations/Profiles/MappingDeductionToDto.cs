using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingDeductionToDto : Profile
    {
        public MappingDeductionToDto()
        {
            CreateMap<Deduction, DeductionDto>();
            CreateMap<Deduction, DeductionDto>().ReverseMap();
        }
    }
}
