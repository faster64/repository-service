using System.Linq;
using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingEmployeeToSyncEmployeeDto : Profile
    {
        public MappingEmployeeToSyncEmployeeDto()
        {
            RegisterMapping();
        }

        private void RegisterMapping()
        {
            CreateMap<Employee, SyncEmployeeDto>()
                .ForMember(dto => dto.EmployeeBranches,
                opt => opt.MapFrom(x => x.EmployeeBranches.Select(y => new EmployeeBranch(y.BranchId, y.EmployeeId)
                {
                    Id = y.Id,
                    TenantId = y.TenantId
                }).ToList()));
            CreateMap<SyncEmployeeDto, Employee>();
        }
    }
}