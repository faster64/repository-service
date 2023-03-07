using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingShiftToShiftDto : Profile
    {
        public MappingShiftToShiftDto()
        {
            RegisterShiftDtoToModel();
        }

        private void RegisterShiftDtoToModel()
        {
            CreateMap<Shift, ShiftDto>();
            CreateMap<Shift, ShiftDto>().ReverseMap().ConstructUsing(c => new Shift(
                c.Id,
                c.Name,
                c.From,
                c.To,
                c.IsActive,
                c.BranchId)
            );
        }
    }
}
