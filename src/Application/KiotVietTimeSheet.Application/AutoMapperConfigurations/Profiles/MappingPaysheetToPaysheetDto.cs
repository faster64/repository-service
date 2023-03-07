using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingPaysheetToPaysheetDto : Profile
    {
        public MappingPaysheetToPaysheetDto()
        {
            CreateMap<Paysheet, PaysheetDto>();
        }
    }
}
