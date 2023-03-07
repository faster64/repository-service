using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingEmployeeToEmployeeDto : Profile
    {
        public MappingEmployeeToEmployeeDto()
        {
            RegisterMapping();
        }

        private void RegisterMapping()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.ProfilePictures, opt => opt.MapFrom(x => x.ProfilePictures.Select(pp => new EmployeeProfilePicture()
                {
                    Id = pp.Id,
                    EmployeeId = pp.EmployeeId,
                    ImageUrl = pp.ImageUrl,
                    IsMainImage = pp.IsMainImage,
                    TenantId = pp.TenantId

                }).ToList()))
                .ForMember(d => d.Clockings, opt => opt.MapFrom<List<Clocking>>(s => null))
                .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department.Name))
                .ForMember(d => d.JobTitleName, opt => opt.MapFrom(s => s.JobTitle.Name))
                .ForMember(dto => dto.EmployeeBranches,
                    opt => opt.MapFrom(x => x.EmployeeBranches.Select(y => new EmployeeBranch(y.BranchId, y.EmployeeId)
                    {
                        Id = y.Id,
                        TenantId = y.TenantId
                    }).ToList()));
            
            CreateMap<EmployeeDto, Employee>();
        }
    }
}
