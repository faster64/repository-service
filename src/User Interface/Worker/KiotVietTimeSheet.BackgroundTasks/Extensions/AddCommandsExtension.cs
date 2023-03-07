using System.Collections.Generic;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByCategory;
using KiotVietTimeSheet.Application.Commands.CreatePaysheetPayslip;
using KiotVietTimeSheet.Application.Commands.CreateTenantNationHoliday;
using KiotVietTimeSheet.Application.Commands.UpdatePaysheetFailed;
using KiotVietTimeSheet.Application.Commands.UpdatePaySheetPayslips;
using KiotVietTimeSheet.Application.Commands.UpdatePaysheets;
using KiotVietTimeSheet.Application.Commands.UpdatePaysheetTemporaryStatus;
using KiotVietTimeSheet.Application.Commands.UpdatePayslip;
using KiotVietTimeSheet.Application.Commands.UpdateTimesheet;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class AddCommandsExtension
    {
        public static void AddCommands(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<UpdatePaysheetsCommand, Unit>, UpdatePaysheetsCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionDetailByCategoryCommand, Unit>, CreateCommissionDetailByCategoryCommandHandler>();
            services.AddScoped<IRequestHandler<UpdatePayslipCommand, Unit>, UpdatePayslipCommandHandler>();
            services.AddScoped<IRequestHandler<CreateTenantNationHolidayCommand, List<Holiday>>, CreateTenantNationHolidayCommandHandler>();
            services.AddScoped<IRequestHandler<CreatePaySheetPayslipCommand, PaysheetDto>, CreatePaySheetPayslipCommandHandler>();
            services.AddScoped<IRequestHandler<UpdatePaySheetPayslipsCommand, PaysheetDto>, UpdatePaySheetPayslipsCommandHandler>();
            services.AddScoped<IRequestHandler<UpdatePaysheetFailedCommand, Unit>, UpdatePaysheetFailedCommandHandler>();
            services.AddScoped<IRequestHandler<UpdatePaysheetTemporaryStatusCommand, PaysheetDto>, UpdatePaysheetTemporaryStatusCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateTimesheetCommand, TimeSheetDto>, UpdateTimesheetCommandHandler>();
        }
    }
}
