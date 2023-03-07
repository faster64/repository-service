using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Utilities;
using ServiceStack;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingTimeSheetToTimeSheetDto : Profile
    {
        public MappingTimeSheetToTimeSheetDto()
        {
            RegisterTimeSheetToTimeSheetDto();
        }

        private void RegisterTimeSheetToTimeSheetDto()
        {
            CreateMap<TimeSheet, TimeSheetDto>().AfterMap((src, dest) =>
            {
                dest.TimeSheetShifts = dest.TimeSheetShifts.CreateCopy();
                dest.TimeSheetShifts.Map(x => x.TimeSheet = null);
                dest.HasEndDate = src.AutoGenerateClockingStatus != (byte) AutoGenerateClockingStatus.Auto;
            });


            CreateMap<Clocking, Clocking>();
        }
    }
}
