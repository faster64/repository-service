using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingPenalizeToDto : Profile
    {
        public MappingPenalizeToDto()
        {
            RegisterMappingPenalizeToDto();
        }

        private void RegisterMappingPenalizeToDto()
        {
            CreateMap<Penalize, PenalizeDto>();
            CreateMap<Penalize, PenalizeDto>().ReverseMap();
        }
    }
}
