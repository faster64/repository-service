using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingTimeSheetShiftToTimeSheetShiftDto : Profile
    {
        public MappingTimeSheetShiftToTimeSheetShiftDto()
        {
            RegisterTimeSheetShiftToTimeSheetShiftDto();
        }

        private void RegisterTimeSheetShiftToTimeSheetShiftDto()
        {
            CreateMap<TimeSheetShift, TimeSheetShiftDto>();
            CreateMap<TimeSheetShift, TimeSheetShiftDto>().ReverseMap();
        }
    }
}
