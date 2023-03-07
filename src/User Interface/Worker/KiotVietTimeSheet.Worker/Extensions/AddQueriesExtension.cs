using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetAllowanceById;
using KiotVietTimeSheet.Application.Queries.GetCommissionById;
using KiotVietTimeSheet.Application.Queries.GetCommissionByIds;
using KiotVietTimeSheet.Application.Queries.GetDeductionById;
using KiotVietTimeSheet.Application.Queries.GetDepartmentById;
using KiotVietTimeSheet.Application.Queries.GetEmployeeById;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByIds;
using KiotVietTimeSheet.Application.Queries.GetFingerMachineByMachineId;
using KiotVietTimeSheet.Application.Queries.GetFingerPrintByFingerCode;
using KiotVietTimeSheet.Application.Queries.GetJobTitleById;
using KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeId;
using KiotVietTimeSheet.Application.Queries.GetPaysheetById;
using KiotVietTimeSheet.Application.Queries.GetPenalizesByTenantId;
using KiotVietTimeSheet.Application.Queries.GetShiftById;
using KiotVietTimeSheet.Application.Queries.GetShiftByIds;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace KiotVietTimeSheet.AuditTrailWorker.Extensions
{
    public static class AddQueriesExtension
    {
        public static void AddQueries(this IServiceCollection services)
        {
            //addScope Employee
            services.AddScoped<IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>, GetEmployeeByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeByIdsQuery, List<EmployeeDto>>, GetEmployeeByIdsQueryHandler>();

            //addScope Paysheet
            services.AddScoped<IRequestHandler<GetPaysheetByIdQuery, PaysheetDto>, GetPaysheetByIdQueryHandler>();

            //addScope Payrate
            services.AddScoped<IRequestHandler<GetPayRateByEmployeeIdQuery, PayRateDto>, GetPayRateByEmployeeIdQueryHandler>();

            //addScope shift
            services.AddScoped<IRequestHandler<GetShiftByIdsQuery, List<ShiftDto>>, GetShiftByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetShiftByIdQuery, ShiftDto>, GetShiftByIdQueryHandler>();

            //addScope Department
            services.AddScoped<IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>, GetDepartmentByIdQueryHandler>();

            //addScope Jobtitle
            services.AddScoped<IRequestHandler<GetJobTitleByIdQuery, JobTitleDto>, GetJobTitleByIdQueryHandler>();

            //addScope Allowance
            services.AddScoped<IRequestHandler<GetAllowanceByIdQuery, AllowanceDto>, GetAllowanceByIdQueryHandler>();

            //addScope Deduction
            services.AddScoped<IRequestHandler<GetDeductionByIdQuery, DeductionDto>, GetDeductionByIdQueryHandler>();

            //addScope Commission
            services.AddScoped<IRequestHandler<GetCommissionByIdQuery, CommissionDto>, GetCommissionByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionByIdsQuery, List<CommissionDto>>, GetCommissionByIdsQueryHandler>();

            //addScope FingerPrint
            services.AddScoped<IRequestHandler<GetFingerPrintByFingerCodeQuery, FingerPrintDto>, GetFingerPrintByFingerCodeHandler>();

            //addScope FingerMachine
            services.AddScoped<IRequestHandler<GetFingerMachineByMachineIdQuery, FingerMachineDto>, GetFingerMachineByMachineIdQueryHandler>();

            //add scope clocking
            services.AddScoped<IRequestHandler<GetPenalizesByIdsQuery, List<PenalizeDto>>, GetPenalizesByIdsQueryHandler>();

        }
    }
}
