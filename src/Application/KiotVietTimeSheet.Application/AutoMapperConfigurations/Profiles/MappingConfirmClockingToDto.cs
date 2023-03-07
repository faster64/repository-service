using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingConfirmClockingToDto : Profile
    {
        public MappingConfirmClockingToDto()
        {
            RegisterMapping();
        }

        private void RegisterMapping()
        {
            CreateMap<ConfirmClocking, ConfirmClockingDto>();
        }
    }
}
