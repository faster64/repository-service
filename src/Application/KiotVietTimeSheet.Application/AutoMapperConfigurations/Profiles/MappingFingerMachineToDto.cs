using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingFingerMachineToDto : Profile
    {
        public MappingFingerMachineToDto()
        {
            CreateMap<FingerMachine, FingerMachineDto>();
            CreateMap<FingerMachine, FingerMachineDto>()
                .ReverseMap();

        }
    }
}
