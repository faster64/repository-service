using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingGpsToDto : Profile
    {
        public MappingGpsToDto()
        {
            RegisterMapping();
        }

        private void RegisterMapping()
        {
            CreateMap<GpsInfo, GpsInfoDto>();
        }
    }
}
