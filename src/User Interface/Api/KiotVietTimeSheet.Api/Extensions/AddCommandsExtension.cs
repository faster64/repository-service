using System.Collections.Generic;
using KiotVietTimeSheet.Application.Commands.AssignUserIdToEmployee;
using KiotVietTimeSheet.Application.Commands.AutoLoadingAndUpdatePaySheet;
using KiotVietTimeSheet.Application.Commands.BatchAddTimeSheetWhenCreateMultipleTimeSheet;
using KiotVietTimeSheet.Application.Commands.CancelPaysheet;
using KiotVietTimeSheet.Application.Commands.CancelPayslip;
using KiotVietTimeSheet.Application.Commands.CancelTimeSheet;
using KiotVietTimeSheet.Application.Commands.ChangeVersionPaysheet;
using KiotVietTimeSheet.Application.Commands.CompletePaysheet;
using KiotVietTimeSheet.Application.Commands.CopyListTimeSheet;
using KiotVietTimeSheet.Application.Commands.CreateAllowance;
using KiotVietTimeSheet.Application.Commands.CreateOrUpdateSetting;
using KiotVietTimeSheet.Application.Commands.CreatePayrateTemplate;
using KiotVietTimeSheet.Application.Commands.CreatePaysheet;
using KiotVietTimeSheet.Application.Commands.CreateShift;
using KiotVietTimeSheet.Application.Commands.CreateTimesheet;
using KiotVietTimeSheet.Application.Commands.CreateDepartment;
using KiotVietTimeSheet.Application.Commands.CreateEmployee;
using KiotVietTimeSheet.Application.Commands.CreateFingerMachine;
using KiotVietTimeSheet.Application.Commands.CreateFingerPrintLog;
using KiotVietTimeSheet.Application.Commands.CreateHoliday;
using KiotVietTimeSheet.Application.Commands.CreateJobTitle;
using KiotVietTimeSheet.Application.Commands.CreateNationalHoliday;
using KiotVietTimeSheet.Application.Commands.CreateCommission;
using KiotVietTimeSheet.Application.Commands.CreateDeduction;
using KiotVietTimeSheet.Application.Commands.DeleteAllowance;
using KiotVietTimeSheet.Application.Commands.UpdateAllowance;
using KiotVietTimeSheet.Application.Commands.CreateUpdateBranchSetting;
using KiotVietTimeSheet.Application.Commands.UpdateClockingShiftAndDateTime;
using KiotVietTimeSheet.Application.Commands.UpdateClockingMultipleBranchShiftAndDateTime;
using KiotVietTimeSheet.Application.Commands.SwapClocking;
using KiotVietTimeSheet.Application.Commands.UpdateClocking;
using KiotVietTimeSheet.Application.Commands.BatchUpdateClocking;
using KiotVietTimeSheet.Application.Commands.RejectClockingByFilter;
using KiotVietTimeSheet.Application.Commands.RejectClockingByBranches;
using KiotVietTimeSheet.Application.Commands.RejectClockingMultiple;
using KiotVietTimeSheet.Application.Commands.UpdateWhenUseAutomatedTimeKeeping;
using KiotVietTimeSheet.Application.Commands.BatchUpdateWhenUseAutomatedTimeKeeping;
using KiotVietTimeSheet.Application.Commands.CopyPayrateTemplate;
using KiotVietTimeSheet.Application.Commands.GetBranchesWhenHaveAnyClocking;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetail;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategory;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProduct;
using KiotVietTimeSheet.Application.Commands.CreateMultipleCommissionDetail;
using KiotVietTimeSheet.Application.Commands.UpdateCommissionDetail;
using KiotVietTimeSheet.Application.Commands.DeleteCommissionDetail;
using KiotVietTimeSheet.Application.Commands.UpdateValueOfCommissionDetail;
using KiotVietTimeSheet.Application.Commands.InsertCommissionDetails;
using KiotVietTimeSheet.Application.Commands.CreateUpdateFingerPrint;
using KiotVietTimeSheet.Application.Commands.DeleteDepartment;
using KiotVietTimeSheet.Application.Commands.DeleteEmployee;
using KiotVietTimeSheet.Application.Commands.DeleteFingerMachine;
using KiotVietTimeSheet.Application.Commands.DeleteHoliday;
using KiotVietTimeSheet.Application.Commands.DeleteJobTitle;
using KiotVietTimeSheet.Application.Commands.DeleteMultipleEmployee;
using KiotVietTimeSheet.Application.Commands.RemoveEmployeePictureId;
using KiotVietTimeSheet.Application.Commands.UnAssignUserEmployee;
using KiotVietTimeSheet.Application.Commands.UpdateDepartment;
using KiotVietTimeSheet.Application.Commands.UpdateEmployee;
using KiotVietTimeSheet.Application.Commands.UpdateFingerMachine;
using KiotVietTimeSheet.Application.Commands.UpdateHoliday;
using KiotVietTimeSheet.Application.Commands.UpdateJobTitle;
using KiotVietTimeSheet.Application.Commands.DeleteCommission;
using KiotVietTimeSheet.Application.Commands.DeleteDeduction;
using KiotVietTimeSheet.Application.Commands.UpdateCommission;
using KiotVietTimeSheet.Application.Commands.UpdateDeduction;
using KiotVietTimeSheet.Application.Commands.DeletePayrateTemplate;
using KiotVietTimeSheet.Application.Commands.DeleteShift;
using KiotVietTimeSheet.Application.Commands.ExportEmployeePayslipData;
using KiotVietTimeSheet.Application.Commands.ExportPaySheet;
using KiotVietTimeSheet.Application.Commands.ExportPaySheetCommissionDetailData;
using KiotVietTimeSheet.Application.Commands.ExportPaySheetData;
using KiotVietTimeSheet.Application.Commands.ExportPayslipData;
using KiotVietTimeSheet.Application.Commands.PaysheetWhenChangeWorkingPeriod;
using KiotVietTimeSheet.Application.Commands.SendEmailActiveTrial;
using KiotVietTimeSheet.Application.Commands.UpdatePayrateTemplate;
using KiotVietTimeSheet.Application.Commands.UpdatePaysheet;
using KiotVietTimeSheet.Application.Commands.UpdateShift;
using KiotVietTimeSheet.Application.Commands.UpdateTimesheet;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CopyPayrateTemplateCommandHandler = KiotVietTimeSheet.Application.Commands.CopyPayrateTemplate.CopyPayrateTemplateCommandHandler;
using CreateAllowanceCommandHandler = KiotVietTimeSheet.Application.Commands.CreateAllowance.CreateAllowanceCommandHandler;
using UpdatePayrateTemplateCommandHandler = KiotVietTimeSheet.Application.Commands.UpdatePayrateTemplate.UpdatePayrateTemplateCommandHandler;
using UpdatePaysheetCommandHandler = KiotVietTimeSheet.Application.Commands.UpdatePaysheet.UpdatePaysheetCommandHandler;
using CreateFingerMachineCommandHandler = KiotVietTimeSheet.Application.Commands.CreateFingerMachine.CreateFingerMachineCommandHandler;
using CreateHolidayCommandHandler = KiotVietTimeSheet.Application.Commands.CreateHoliday.CreateHolidayCommandHandler;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategoryAsync;
using KiotVietTimeSheet.Application.Commands.CreatePenalize;
using KiotVietTimeSheet.Application.Commands.DeletePenalizes;
using KiotVietTimeSheet.Application.Commands.ImportExport;
using KiotVietTimeSheet.Application.Commands.UpdatePenalize;
using KiotVietTimeSheet.Application.Commands.DeleteMultipleEmployeeByBranchId;
using KiotVietTimeSheet.Application.Commands.UpdateSettingClockingGps;
using KiotVietTimeSheet.Application.Commands.CreateGpsInfo;
using KiotVietTimeSheet.Application.Commands.DeleteGpsInfo;
using KiotVietTimeSheet.Application.Commands.UpdateGpsInfo;
using KiotVietTimeSheet.Application.Commands.UpdateQrKey;
using KiotVietTimeSheet.Application.Commands.UpdateConfirmClocking;
using KiotVietTimeSheet.Application.Commands.UpdateDeviceForEmployeeClockingGps;
using KiotVietTimeSheet.Application.Commands.UpdateClockingForClockingGps;
using KiotVietTimeSheet.Application.Commands.CreateTrialData;
using KiotVietTimeSheet.Application.Commands.DeleteTrialData;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDataTrial;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategoryDataTrial;
using KiotVietTimeSheet.Application.Commands.DeleteCommissionDataTrial;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailCategory;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class AddCommandsExtension
    {
        public static void AddCommands(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CreateAllowanceCommand, AllowanceDto>, CreateAllowanceCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateAllowanceCommand, AllowanceDto>, UpdateAllowanceCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteAllowanceCommand, Unit>, DeleteAllowanceCommandHandler>();
            services.AddScoped<IRequestHandler<CreateOrUpdateBranchSettingCommand, BranchSettingDto>, CreateOrUpdateBranchSettingCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateClockingShiftAndDateTimeCommand, ClockingDto>, UpdateClockingShiftAndDateTimeCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateClockingMultipleBranchShiftAndDateTimeCommand, ClockingDto>, UpdateClockingMultipleBranchShiftAndDateTimeCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateClockingCommand, ClockingDto>, UpdateClockingCommandHandler>();
            services.AddScoped<IRequestHandler<SwapClockingCommand, SwapClockingResultDto>, SwapClockingCommandHandler>();
            services.AddScoped<IRequestHandler<BatchUpdateClockingCommand, List<ClockingDto>>, BatchUpdateClockingCommandHandler>();
            services.AddScoped<IRequestHandler<RejectClockingByFilterCommand, List<ClockingDto>>, RejectClockingByFilterCommandHandler>();
            services.AddScoped<IRequestHandler<RejectClockingByBranchesCommand, List<ClockingDto>>, RejectClockingByBranchesCommandHandler>();
            services.AddScoped<IRequestHandler<RejectClockingMultipleCommand, List<ClockingDto>>, RejectClockingMultipleCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateWhenUseAutomatedTimeKeepingCommand, ClockingDto>, UpdateWhenUseAutomatedTimeKeepingCommandHandler>();
            services.AddScoped<IRequestHandler<BatchUpdateWhenUseAutomatedTimeKeepingCommand, List<ClockingDto>>, BatchUpdateWhenUseAutomatedTimeKeepingCommandHandler>();
            services.AddScoped<IRequestHandler<GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommand, PagingDataSource<BranchDto>>, GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionDetailCommand, List<CommissionDetailDto>>, CreateCommissionDetailCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionDetailByProductCategoryCommand, List<CommissionDetailDto>>, CreateCommissionDetailByProductCategoryCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionDetailByProductCategoryDataTrialCommand, List<CommissionDetailDto>>, CreateCommissionDetailByProductCategoryDataTrialCommandHandler>();
            services.AddScoped<IRequestHandler<CreateMultipleCommissionDetailCommand, List<CommissionDetailDto>>, CreateMultipleCommissionDetailCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateCommissionDetailCommand, List<CommissionDetailDto>>, UpdateCommissionDetailCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionDetailByProductCommand, List<CommissionDetailDto>>, CreateCommissionDetailByProductCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionDetailByProductCategoryAsyncCommand, Unit>, CreateCommissionDetailByProductCategoryAsyncCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionDetailCategoryCommand, List<CommissionDetailDto>>, CreateCommissionDetailCategoryCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteCommissionDetailCommand, List<CommissionDetailDto>>, DeleteCommissionDetailCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateValueOfCommissionDetailCommand, List<CommissionDetailDto>>, UpdateValueOfCommissionDetailCommandHandler>();
            services.AddScoped<IRequestHandler<InsertCommissionDetailsStatusCommand, InsertCommissionDetailsStatus>, InsertCommissionDetailsStatusCommandHandler>();
            services.AddScoped<IRequestHandler<CreateDepartmentCommand, DepartmentDto>, CreateDepartmentCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateDepartmentCommand, Unit>, UpdateDepartmentCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteDepartmentCommand, Unit>, DeleteDepartmentCommandHandler>();
            services.AddScoped<IRequestHandler<AssignUserIdToEmployeeCommand, Unit>, AssignUserIdToEmployeeCommandHandler>();
            services.AddScoped<IRequestHandler<CreateEmployeeCommand, EmployeeDto>, CreateEmployeeCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateEmployeeCommand, EmployeeDto>, UpdateEmployeeCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteEmployeeCommand, Unit>, DeleteEmployeeCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteMultipleEmployeeCommand, Unit>, DeleteMultipleEmployeeCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveEmployeePictureIdCommand, Unit>, RemoveEmployeePictureIdCommandHandler>();
            services.AddScoped<IRequestHandler<UnAssignUserEmployeeCommand, Unit>, UnAssignUserEmployeeCommandHandler>();
            services.AddScoped<IRequestHandler<CreateFingerMachineCommand, FingerMachineDto>, CreateFingerMachineCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateFingerMachineCommand, FingerMachineDto>, UpdateFingerMachineCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteFingerMachineCommand, Unit>, DeleteFingerMachineCommandHandler>();
            services.AddScoped<IRequestHandler<CreateFingerPrintLogCommand, List<AutoTimeKeepingResult>>, CreateFingerPrintLogCommandHandler>();
            services.AddScoped<IRequestHandler<CreateHolidayCommand, HolidayDto>, CreateHolidayCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateHolidayCommand, HolidayDto>, UpdateHolidayCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteHolidayCommand, HolidayDto>, DeleteHolidayCommandHandler>();
            services.AddScoped<IRequestHandler<CreateNationalHolidayCommand, Unit>, CreateNationalHolidayCommandHandler>();
            services.AddScoped<IRequestHandler<CreateJobTitleCommand, JobTitleDto>, CreateJobTitleCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateJobTitleCommand, Unit>, UpdateJobTitleCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteJobTitleCommand, Unit>, DeleteJobTitleCommandHandler>();
            services.AddScoped<IRequestHandler<CreateUpdateFingerPrintCommand, FingerPrintDto>, CreateUpdateFingerPrintCommandHandler>();
            services.AddScoped<IRequestHandler<CreateDeductionCommand, DeductionDto>, CreateDeductionCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateDeductionCommand, DeductionDto>, UpdateDeductionCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteDeductionCommand, Unit>, DeleteDeductionCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionCommand, CommissionDto>, CreateCommissionCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCommissionDataTrialCommand, CommissionDto>, CreateCommissionDataTrialCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateCommissionCommand, CommissionDto>, UpdateCommissionCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteCommissionCommand, CommissionDto>, DeleteCommissionCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteCommissionDataTrialCommand, CommissionDto>, DeleteCommissionDataTrialCommandHandler>();

            services.AddScoped<IRequestHandler<DeleteMultipleEmployeeByBranchIdCommand, Unit>, DeleteMultipleEmployeeByBranchIdCommandHandler>();
            services.AddScoped<IRequestHandler<CreateSyncEmployeeCommand, SyncEmployeeDto>, CreateSyncEmployeeCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteSyncEmployeeCommand, Unit>, DeleteSyncEmployeeCommandHandler>();

            //addscope payrateTemplate
            services.AddScoped<IRequestHandler<CreatePayrateTemplateCommand, PayRateFormDto>, CreatePayrateTemplateCommandHandler>();
            services.AddScoped<IRequestHandler<UpdatePayrateTemplateCommand, PayRateFormDto>, UpdatePayrateTemplateCommandHandler>();
            services.AddScoped<IRequestHandler<DeletePayrateTemplateCommand, Unit>, DeletePayrateTemplateCommandHandler>();
            services.AddScoped<IRequestHandler<CopyPayrateTemplateCommand, PayRateFormDto>, CopyPayrateTemplateCommandHandler>();

            //addscope paysheet
            services.AddScoped<IRequestHandler<CreatePaysheetCommand, PaysheetDto>, CreatePaysheetCommandHandler>();
            services.AddScoped<IRequestHandler<CompletePaysheetCommand, PaysheetDto>, CompletePaysheetCommandHandler>();
            services.AddScoped<IRequestHandler<ChangeVersionPaysheetCommand, Unit>, ChangeVersionPaysheetCommandHandler>();
            services.AddScoped<IRequestHandler<PaysheetWhenChangeWorkingPeriodCommand, PaysheetDto>, PaysheetWhenChangeWorkingPeriodCommandHandler>();
            services.AddScoped<IRequestHandler<ExportPaySheetCommand, PagingDataSource<PaysheetDto>>, ExportPaySheetCommandHandler>();
            services.AddScoped<IRequestHandler<ExportPaySheetDataCommand, PagingDataSource<PaysheetDto>>, ExportPaySheetDataCommandHandler>();
            services.AddScoped<IRequestHandler<UpdatePaysheetCommand, PaysheetDto>, UpdatePaysheetCommandHandler>();
            services.AddScoped<IRequestHandler<CancelPaysheetCommand, PaysheetDto>, CancelPaysheetCommandHandler>();
            services.AddScoped<IRequestHandler<ExportEmployeePayslipCommand, PayslipDto>, ExportEmployeePayslipCommandHandler>();
            services.AddScoped<IRequestHandler<ExportPaySheetCommissionDetailCommand, PaysheetDto>, ExportPaySheetCommissionDetailCommandHandler>();
            services.AddScoped<IRequestHandler<AutoLoadingAndUpdatePaysheetCommand, PaysheetDto>, AutoLoadingAndUpdatePaysheetCommandHandler>();

            //addscope payslip
            services.AddScoped<IRequestHandler<CancelPayslipCommand, PayslipDto>, CancelPayslipCommandHandler>();
            services.AddScoped<IRequestHandler<ExportPayslipDataCommand, List<PayslipDto>>, ExportPayslipDataCommandHandler>();

            //addscope settings
            services.AddScoped<IRequestHandler<CreateOrUpdateSettingCommand, SettingsDto>, CreateOrUpdateSettingCommandHandler>();
            services.AddScoped<IRequestHandler<CreateOrUpdateTimeSheetSettingCommand, SettingsDto>, CreateOrUpdateSettingCommandHandler>();
            services.AddScoped<IRequestHandler<CreateOrUpdateClockingSettingCommand, SettingsDto>, CreateOrUpdateSettingCommandHandler>();
            services.AddScoped<IRequestHandler<CreateOrUpdateCommissionSettingCommand, SettingsDto>, CreateOrUpdateSettingCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateSettingClockingGpsCommand, bool>, UpdateSettingClockingGpsCommandHandler>();

            //addscope shift
            services.AddScoped<IRequestHandler<CreateShiftCommand, ShiftDto>, CreateShiftCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateShiftCommand, ShiftDto>, UpdateShiftCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteShiftCommand, ShiftDto>, DeleteShiftCommandHandler>();

            //addscope timesheet
            services.AddScoped<IRequestHandler<CreateTimesheetCommand, TimeSheetDto>, CreateTimesheetCommandHandler>();
            services.AddScoped<IRequestHandler<BatchAddTimeSheetWhenCreateMultipleTimeSheetCommand, List<TimeSheetDto>>, BatchAddTimeSheetWhenCreateMultipleTimeSheetCommandHandler>();
            services.AddScoped<IRequestHandler<CopyListTimeSheetCommand, List<TimeSheetDto>>, CopyListTimeSheetCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateTimesheetCommand, TimeSheetDto>, UpdateTimesheetCommandHandler>();
            services.AddScoped<IRequestHandler<CancelTimeSheetCommand, Unit>, CancelTimeSheetCommandHandler>();
            services.AddScoped<IRequestHandler<SendEmailActiveTrialCommand, Unit>, SendEmailActiveTrialCommandHandler>();

            //Penalize
            services.AddScoped<IRequestHandler<CreatePenalizeCommand, PenalizeDto>, CreatePenalizeCommandHandler>();
            services.AddScoped<IRequestHandler<UpdatePenalizeCommand, PenalizeDto>, UpdatePenalizeCommandHandler>();
            services.AddScoped<IRequestHandler<DeletePenalizesCommand, Unit>, DeletePenalizesCommandHandler>();

            //GpsInfo
            services.AddScoped<IRequestHandler<CreateGpsInfoCommand, GpsInfoDto>, CreateGpsInfoCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteGpsInfoCommand, Unit>, DeleteGpsInfoCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateGpsInfoCommand, GpsInfoDto>, UpdateGpsInfoCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateQrkeyCommand, string>, UpdateQrkeyCommandHandler>();

            //ConfirmClockingGPs
            services.AddScoped<IRequestHandler<UpdateConfirmClockingCommand, List<ConfirmClockingDto>>, UpdateConfirmClockingCommandHandle>();

            // Clocking
            services.AddScoped<IRequestHandler<UpdateClockingTimeCommand, ClockingDto>, UpdateClockingTimeCommandHandler>();

            // Import Export
            services.AddScoped<IRequestHandler<ExportCommand, ImportExportFileDto>, ExportCommandHandler>();
            services.AddScoped<IRequestHandler<ImportCommissionCommand, ImportExportFileDto>, ImportCommissionCommandHandler>();

            // Clocking GPS
            services.AddScoped<IRequestHandler<UpdateDeviceForEmployeeClockingGpsCommand, EmployeeDto>, UpdateDeviceForEmployeeClockingGpsCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateClockingForClockingGpsCommand, ClockingDto>, UpdateClockingForClockingGpsCommandHandler>();

            services.AddScoped<IRequestHandler<CreateTrialDataCommand, object>, CreateTrialDataCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteTrialDataCommand, object>, DeleteTrialDataCommandHandler>();
        }
    }
}
