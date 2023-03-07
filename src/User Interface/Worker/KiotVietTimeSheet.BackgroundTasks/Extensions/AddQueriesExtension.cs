using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetAllCommissionDetailsByCommissionIds;
using KiotVietTimeSheet.Application.Queries.GetAllDeductionByTenantD;
using KiotVietTimeSheet.Application.Queries.GetAllowanceByTenantId;
using KiotVietTimeSheet.Application.Queries.GetClockingForPaySheet;
using KiotVietTimeSheet.Application.Queries.GetClockingPenalizeForPaySheet;
using KiotVietTimeSheet.Application.Queries.GetCommissionByIds;
using KiotVietTimeSheet.Application.Queries.GetConfirmClockingsByBranchId;
using KiotVietTimeSheet.Application.Queries.GetEmployeeBranchByEmployeeIds;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByBranchId;
using KiotVietTimeSheet.Application.Queries.GetHolidayForPaySheet;
using KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeIds;
using KiotVietTimeSheet.Application.Queries.GetPaysheetById;
using KiotVietTimeSheet.Application.Queries.GetPaysheetByIdForBgTask;
using KiotVietTimeSheet.Application.Queries.GetPaysheetByIds;
using KiotVietTimeSheet.Application.Queries.GetPayslipById;
using KiotVietTimeSheet.Application.Queries.GetPayslipDetailByTenantId;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Queries.GetShiftByBranchId;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class AddQueriesExtension
    {
        public static void AddQueries(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<GetAllCommissionDetailsByCommissionIdsQuery, List<CommissionDetailDto>>, GetAllCommissionDetailsByCommissionIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionByIdsQuery, List<CommissionDto>>, GetCommissionByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayslipDetailByTenantIdQuery, List<PayslipDetailDto>>, GetPayslipDetailByTenantIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayslipByIdsQuery, List<long>>, GetPayslipByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetByIdsQuery, List<Paysheet>>, GetPaysheetByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayslipByIdQuery, PayslipDto>, GetPayslipByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetByIdQuery, PaysheetDto>, GetPaysheetByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetByIdForBgTaskQuery, PaysheetDto>, GetPaysheetByIdForBgTaskQueryHandler>();
            services.AddScoped<IRequestHandler<GetHolidayForPaySheetQuery, List<HolidayDto>>, GetHolidayForPaySheetQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayRateByEmployeeIdsQuery, List<PayRate>>, GetPayRateByEmployeeIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllDeductionByTenantDQuery, List<DeductionDto>>, GetAllDeductionByTenantDQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllowanceByTenantIdQuery, List<AllowanceDto>>, GetAllowanceByTenantIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingForPaySheetQuery, List<ClockingDto>>, GetClockingForPaySheetQueryHandler>();
            services.AddScoped<IRequestHandler<GetShiftByBranchIdQuery, List<ShiftDto>>, GetShiftByBranchIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeBranchByEmployeeIdsQuery, List<EmployeeBranchDto>>, GetEmployeeBranchByEmployeeIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeByBranchIdQuery, List<EmployeeDto>>, GetEmployeeByBranchIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingPenalizeForPaySheetQuery, List<ClockingPenalizeDto>>, GetClockingPenalizeForPaySheetQueryHandler>();
            services.AddScoped<IRequestHandler<GetSettingQuery, SettingObjectDto>, GetSettingQueryHandler>();

            services.AddScoped<IRequestHandler<GetConfirmClockingsByBranchIdQuery, List<ConfirmClockingDto>>, GetConfirmClockingsByBranchIdQueryHandler>();
        }
    }
}
