using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingFingerPrintToDto : Profile
    {
        public MappingFingerPrintToDto()
        {
            CreateMap<FingerPrint, FingerPrintDto>();
            CreateMap<FingerPrint, FingerPrintDto>().ReverseMap();
        }
    }
}
