using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingConfirmClockingHistoryToDto : Profile
    {
        public MappingConfirmClockingHistoryToDto()
        {
            RegisterMapping();
        }

        private void RegisterMapping()
        {
            CreateMap<ConfirmClockingHistory, ConfirmClockingHistoryDto>();
        }
    }
}
