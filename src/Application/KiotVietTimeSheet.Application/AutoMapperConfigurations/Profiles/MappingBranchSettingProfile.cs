using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using System.Collections.Generic;
using System.Linq;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    class MappingBranchSettingProfile : Profile
    {
        public MappingBranchSettingProfile()
        {
            RegisterMapping();
        }

        private void RegisterMapping()
        {
            CreateMap<BranchSetting, BranchSettingDto>()
                .ForMember(d => d.WorkingDays, opt => opt.MapFrom(b => b.WorkingDaysInArray))
                .ForMember(d => d.WorkingDays, opt => opt.MapFrom(b => ConvertStringToListByte(b.WorkingDays)));
        }

        private List<byte> ConvertStringToListByte(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new List<byte>();
            }
            var result = value.Split(',').Select(byte.Parse).ToList();
            return result;
        }
    }
}
