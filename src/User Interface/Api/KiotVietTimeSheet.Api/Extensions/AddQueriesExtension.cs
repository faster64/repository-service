using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.CheckEmployeeAssignUserId;
using KiotVietTimeSheet.Application.Queries.CheckEmployeeTotalWithBlock;
using KiotVietTimeSheet.Application.Queries.GetAllowances;
using KiotVietTimeSheet.Application.Queries.GetBranchSetting;
using KiotVietTimeSheet.Application.Queries.GetAllClocking;
using KiotVietTimeSheet.Application.Queries.GetClockingById;
using KiotVietTimeSheet.Application.Queries.GetClockingsForCalendar;
using KiotVietTimeSheet.Application.Queries.GetClockingsMultipleBranch;
using KiotVietTimeSheet.Application.Queries.GetClockingsByTimeSheetId;
using KiotVietTimeSheet.Application.Queries.GetClockingsByShiftId;
using KiotVietTimeSheet.Application.Queries.GetClockingForPaySlipClockingDetail;
using KiotVietTimeSheet.Application.Queries.GetClockingsByBranchId;
using KiotVietTimeSheet.Application.Queries.GetAllClockingHistory;
using KiotVietTimeSheet.Application.Queries.GetComissionAll;
using KiotVietTimeSheet.Application.Queries.GetCommissionByIds;
using KiotVietTimeSheet.Application.Queries.GetCommissionInActiveByIds;
using KiotVietTimeSheet.Application.Queries.GetCommissionById;
using KiotVietTimeSheet.Application.Queries.GetByCommissionCurrentBranch;
using KiotVietTimeSheet.Application.Queries.GetCommissionDetailsByCommissionIds;
using KiotVietTimeSheet.Application.Queries.GetCommissionDetailsByProductId;
using KiotVietTimeSheet.Application.Queries.GetAllCommissionDetailsByCommissionIds;
using KiotVietTimeSheet.Application.Queries.GetHasAnyCommission;
using KiotVietTimeSheet.Application.Queries.GetAllDeduction;
using KiotVietTimeSheet.Application.Queries.GetDeductionByFilter;
using KiotVietTimeSheet.Application.Queries.GetDeductionById;
using KiotVietTimeSheet.Application.Queries.GetDeductionByIds;
using KiotVietTimeSheet.Application.Queries.GetAllowanceById;
using KiotVietTimeSheet.Application.Queries.GetAllowancesByIds;
using KiotVietTimeSheet.Application.Queries.GetBranchSettingByIds;
using KiotVietTimeSheet.Application.Queries.CheckCanUpdateBranchSetting;
using KiotVietTimeSheet.Application.Queries.GetAllPenalizeByTenantId;
using KiotVietTimeSheet.Application.Queries.GetAllPenalizes;
using KiotVietTimeSheet.Application.Queries.GetClockingForSwap;
using KiotVietTimeSheet.Application.Queries.GetCommissionByName;
using KiotVietTimeSheet.Application.Queries.GetDepartment;
using KiotVietTimeSheet.Application.Queries.GetEmployee;
using KiotVietTimeSheet.Application.Queries.GetEmployeeAvailable;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByBranchId;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByCurrentUserId;
using KiotVietTimeSheet.Application.Queries.GetEmployeeById;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByUserId;
using KiotVietTimeSheet.Application.Queries.GetEmployeeForPaysheet;
using KiotVietTimeSheet.Application.Queries.GetEmployeeMultipleBranch;
using KiotVietTimeSheet.Application.Queries.GetFingerMachine;
using KiotVietTimeSheet.Application.Queries.GetFingerPrint;
using KiotVietTimeSheet.Application.Queries.GetFingerPrintByEmployeeId;
using KiotVietTimeSheet.Application.Queries.GetFingerPrintByFingerCode;
using KiotVietTimeSheet.Application.Queries.GetHoliday;
using KiotVietTimeSheet.Application.Queries.GetHolidayById;
using KiotVietTimeSheet.Application.Queries.GetHolidayTotalDays;
using KiotVietTimeSheet.Application.Queries.GetJobTitle;
using KiotVietTimeSheet.Application.Queries.GetPayRate;
using KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeId;
using KiotVietTimeSheet.Application.Queries.GetPayRateById;
using KiotVietTimeSheet.Application.Queries.GetDraftPaysheet;
using KiotVietTimeSheet.Application.Queries.GetExportPayslipData;
using KiotVietTimeSheet.Application.Queries.GetGenerateWorkingPeriod;
using KiotVietTimeSheet.Application.Queries.GetHolidayInDateRange;
using KiotVietTimeSheet.Application.Queries.GetListShift;
using KiotVietTimeSheet.Application.Queries.GetListTimesheet;
using KiotVietTimeSheet.Application.Queries.GetPayrateTemplate;
using KiotVietTimeSheet.Application.Queries.GetPayrateTemplateById;
using KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeById;
using KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeVersion;
using KiotVietTimeSheet.Application.Queries.GetPaysheetById;
using KiotVietTimeSheet.Application.Queries.GetPaysheets;
using KiotVietTimeSheet.Application.Queries.GetPaysheetsOldVersionByIds;
using KiotVietTimeSheet.Application.Queries.GetPaysheetsWorkingDays;
using KiotVietTimeSheet.Application.Queries.GetPayslipById;
using KiotVietTimeSheet.Application.Queries.GetPayslipsByFilter;
using KiotVietTimeSheet.Application.Queries.GetPayslipsByPaysheetId;
using KiotVietTimeSheet.Application.Queries.GetPaySlipsByQueryFilter;
using KiotVietTimeSheet.Application.Queries.GetPayslipsClockingByPayslipId;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Queries.GetShiftById;
using KiotVietTimeSheet.Application.Queries.GetShiftByWorkingDayEmployee;
using KiotVietTimeSheet.Application.Queries.GetShiftMultipleBranchOrderByFromAndTo;
using KiotVietTimeSheet.Application.Queries.GetShiftOrderByFromAndTo;
using KiotVietTimeSheet.Application.Queries.GetTimeSheetById;
using KiotVietTimeSheet.Application.Queries.GetUnPaidPayslipByEmployeeId;
using KiotVietTimeSheet.SharedKernel.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using GetExportPayslipDataQueryHandler = KiotVietTimeSheet.Application.Queries.GetExportPayslipData.GetExportPayslipDataQueryHandler;
using GetPayrateTemplateQueryHandler = KiotVietTimeSheet.Application.Queries.GetPayrateTemplate.GetPayrateTemplateQueryHandler;
using GetPaysheetAndCheckChangeByIdQueryHandler = KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeById.GetPaysheetAndCheckChangeByIdQueryHandler;
using GetPayslipsByFilterQueryHandler = KiotVietTimeSheet.Application.Queries.GetPayslipsByFilter.GetPayslipsByFilterQueryHandler;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByIds;
using KiotVietTimeSheet.Application.Queries.GetFingerMachineByMachineId;
using KiotVietTimeSheet.Application.Queries.GetListGpsInfo;
using KiotVietTimeSheet.Application.Queries.GetShiftByIds;
using KiotVietTimeSheet.Application.Queries.GetPaysheetExistsPayslips;
using KiotVietTimeSheet.Application.Queries.GetGpsInfoById;
using KiotVietTimeSheet.Application.Queries.GetAndCheckTwoFaPin;
using KiotVietTimeSheet.Application.Queries.GetListConfirmClocking;
using KiotVietTimeSheet.Application.Queries.GetListConfirmClockingHistory;
using KiotVietTimeSheet.Application.Queries.GetClockingsForClockingGps;
using KiotVietTimeSheet.Application.Queries.GetEmployeeForClockingGps;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByIdentityKeyForClockingGps;
using KiotVietTimeSheet.Application.Queries.GetTimesheetConfigForMobile;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Application.Queries.GetBranchsForMobile;
using KiotVietTimeSheet.Application.Queries.GetUserAccount;
using KiotVietTimeSheet.Application.Queries.GetConfirmClockingsByBranchId;
using KiotVietTimeSheet.Application.Queries.GetTotalGprsQuery;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class AddQueriesExtension
    {
        public static void AddQueries(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<GetAllowancesQuery, PagingDataSource<AllowanceDto>>, GetAllowancesQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllowanceByIdQuery, AllowanceDto>, GetAllowanceByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllowancesByIdsQuery, PagingDataSource<AllowanceDto>>, GetAllowancesByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetBranchSettingByIdQuery, BranchSettingDto>, GetBranchSettingByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetBranchSettingByIdsQuery, List<BranchSettingDto>>, GetBranchSettingByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<CheckCanUpdateBranchSettingQuery, bool>, CheckCanUpdateBranchSettingQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllClockingQuery, PagingDataSource<ClockingDto>>, GetAllClockingQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingByIdQuery, ClockingDto>, GetClockingByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingForSwapQuery, List<ClockingDto>>, GetClockingForSwapQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingsForCalendarQuery, PagingDataSource<ClockingDto>>, GetClockingsForCalendarQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingsMultipleBranchForCalendarQuery, PagingDataSource<ClockingDto>>, GetClockingsMultipleBranchForCalendarQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingsByTimeSheetIdQuery, List<ClockingDto>>, GetClockingsByTimeSheetIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingsByShiftIdQuery, List<ClockingDto>>, GetClockingsByShiftIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingsByBranchIdQuery, List<ClockingDto>>, GetClockingsByBranchIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetClockingForPaySlipClockingDetailQuery, PayslipClockingDetailPagingDataSource>, GetClockingForPaySlipClockingDetailQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllClockingHistoryQuery, PagingDataSource<ClockingHistoryDto>>, GetAllClockingHistoryQueryHandler>();
            services.AddScoped<IRequestHandler<GetComissionAllQuery, PagingDataSource<CommissionDto>>, GetComissionAllQueryHandler>();
            services.AddScoped<IRequestHandler<GetComissionAllDataTrialQuery, PagingDataSource<CommissionDto>>, GetComissionAllDataTrialQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionByIdsQuery, List<CommissionDto>>, GetCommissionByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionInActiveByIdsQuery, List<CommissionDto>>, GetCommissionInActiveByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionByIdQuery, CommissionDto>, GetCommissionByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetByCommissionCurrentBranchQuery, List<CommissionDto>>, GetByCommissionCurrentBranchQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionDetailsByCommissionIdsQuery, List<CommissionDetailDto>>, GetCommissionDetailsByCommissionIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionDetailsByProductIdQuery, PagingDataSource<CommissionDetailDto>>, GetCommissionDetailsByProductIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllCommissionDetailsByCommissionIdsQuery, List<CommissionDetailDto>>, GetAllCommissionDetailsByCommissionIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetCommissionByNameQuery, List<CommissionDto>>, GetCommissionByNameQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllDeductionQuery, PagingDataSource<DeductionDto>>, GetAllDeductionQueryHandler>();
            services.AddScoped<IRequestHandler<GetDeductionByFilterQuery, object>, GetDeductionByFilterQueryHandler>();
            services.AddScoped<IRequestHandler<GetDeductionByIdQuery, DeductionDto>, GetDeductionByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetDeductionByIdsQuery, PagingDataSource<DeductionDto>>, GetDeductionByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetHasAnyCommissionQuery, bool>, GetHasAnyCommissionQueryHandler>();
            services.AddScoped<IRequestHandler<CheckCanUpdateBranchSettingQuery, bool>, CheckCanUpdateBranchSettingQueryHandler>();
            services.AddScoped<IRequestHandler<CheckEmployeeAssignUserIdQuery, bool>, CheckEmployeeAssignUserIdQueryHandler>();
            services.AddScoped<IRequestHandler<CheckEmployeeTotalWithBlockQuery, bool>, CheckEmployeeTotalWithBlockQueryHandler>();
            services.AddScoped<IRequestHandler<GetDepartmentQuery, PagingDataSource<DepartmentDto>>, GetDepartmentQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeQuery, PagingDataSource<EmployeeDto>>, GetEmployeeQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeAvailableQuery, PagingDataSource<EmployeeDto>>, GetEmployeeAvailableQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeByBranchIdQuery, List<EmployeeDto>>, GetEmployeeByBranchIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeByCurrentUserIdQuery, EmployeeDto>, GetEmployeeByCurrentUserIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>, GetEmployeeByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeByUserIdQuery, EmployeeDto>, GetEmployeeByUserIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeForPaysheetQuery, PagingDataSource<EmployeeDto>>, GetEmployeeForPaysheetQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeMultipleBranchQuery, PagingDataSource<EmployeeDto>>, GetEmployeeMultipleBranchQueryHandler>();
            services.AddScoped<IRequestHandler<GetFingerMachineQuery, PagingDataSource<FingerMachineDto>>, GetFingerMachineQueryHandler>();
            services.AddScoped<IRequestHandler<GetFingerPrintQuery, PagingDataSource<FingerPrintDto>>, GetFingerPrintQueryHandler>();
            services.AddScoped<IRequestHandler<GetFingerPrintByEmployeeIdQuery, FingerPrintDto>, GetFingerPrintByEmployeeIdHandler>();
            services.AddScoped<IRequestHandler<GetFingerPrintByFingerCodeQuery, FingerPrintDto>, GetFingerPrintByFingerCodeHandler>();
            services.AddScoped<IRequestHandler<GetHolidayQuery, HolidayPagingDataSource>, GetHolidayQueryHandler>();
            services.AddScoped<IRequestHandler<GetHolidayByIdQuery, HolidayDto>, GetHolidayByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetHolidayTotalDaysQuery, int>, GetHolidayTotalDaysQueryHandler>();
            services.AddScoped<IRequestHandler<GetJobTitleQuery, PagingDataSource<JobTitleDto>>, GetJobTitleQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayRateQuery, PagingDataSource<PayRateDto>>, GetPayRateQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayRateByEmployeeIdQuery, PayRateDto>, GetPayRateByEmployeeIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayRateByIdQuery, PayRateDto>, GetPayRateByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetHasAnyCommissionQuery, bool>, GetHasAnyCommissionQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllPenalizesQuery, PagingDataSource<PenalizeDto>>, GetAllPenalizesQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllPenalizeByTenantIdQuery, List<PenalizeDto>>, GetAllPenalizeByTenantIdQueryHandler>();
            services
                .AddScoped<IRequestHandler<SyncEmployeeListQuery, PagingDataSource<SyncEmployeeDto>>,
                    SyncEmployeeListQueryHandler>();

            // Query payrateTemplate
            services.AddScoped<IRequestHandler<GetPayrateTemplateQuery, PagingDataSource<PayRateFormDto>>, GetPayrateTemplateQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayrateTemplateByIdQuery, PayRateFormDto>, GetPayrateTemplateByIdQueryHandler>();

            //Query paysheet
            services.AddScoped<IRequestHandler<GetPaysheetsQuery, PagingDataSource<PaysheetDto>>, GetPaysheetsQueryHandler>();
            services.AddScoped<IRequestHandler<GetDraftPaysheetQuery, PaysheetDto>, GetDraftPaysheetQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetWorkingDaysQuery, object>, GetPaysheetWorkingDaysQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetByIdQuery, PaysheetDto>, GetPaysheetByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetAndCheckChangeByIdQuery, PaysheetDto>, GetPaysheetAndCheckChangeByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetAndCheckChangeVersionQuery, bool>, GetPaysheetAndCheckChangeVersionQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetsOldVersionByIdsQuery, List<PaysheetDto>>, GetPaysheetsOldVersionByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GenerateWorkingPeriodQuery, List<PaySheetWorkingPeriodDto>>, GenerateWorkingPeriodQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaysheetExistsPayslipsQuery, bool>, GetPaysheetExistsPayslipsQueryHandler>();

            //Query Payslip
            services.AddScoped<IRequestHandler<GetPayslipsByFilterQuery, PagingDataSource<PayslipDto>>, GetPayslipsByFilterQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayslipsByPaysheetIdQuery, PagingDataSource<PayslipDto>>, GetPayslipsByPaysheetIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetExportPayslipDataQuery, List<PayslipDto>>, GetExportPayslipDataQueryHandler>();
            services.AddScoped<IRequestHandler<GetPaySlipsByQueryFilterQuery, PagingDataSource<PayslipDto>>, GetPaySlipsByQueryFilterQueryHandler>();
            services.AddScoped<IRequestHandler<GetUnPaidPayslipByEmployeeIdQuery, object>, GetUnPaidPayslipByEmployeeIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetPayslipByIdQuery, PayslipDto>, GetPayslipByIdQueryHandler>();
            

            //Query Payslip Clocking
            services.AddScoped<IRequestHandler<GetPayslipsClockingByPayslipIdQuery, PagingDataSource<PayslipClockingDto>>, GetPayslipsClockingByPayslipIdQueryHandler>();

            //Query Setting
            services.AddScoped<IRequestHandler<GetSettingQuery, SettingObjectDto>, GetSettingQueryHandler>();

            //Query Shift
            services.AddScoped<IRequestHandler<GetListShiftQuery, PagingDataSource<ShiftDto>>, GetListShiftQueryHandler>();
            services.AddScoped<IRequestHandler<GetShiftByIdQuery, ShiftDto>, GetShiftByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetShiftByWorkingDayEmployeeQuery, List<ShiftDto>>, GetShiftByWorkingDayEmployeeQueryHandler>();
            services.AddScoped<IRequestHandler<GetShiftOrderByFromAndToQuery, List<ShiftDto>>, GetShiftOrderByFromAndToQueryHandler>();
            services.AddScoped<IRequestHandler<GetShiftMultipleBranchOrderByFromAndToQuery, List<ShiftDto>>, GetShiftMultipleBranchOrderByFromAndToQueryHandler>();

            //Query Timesheet
            services.AddScoped<IRequestHandler<GetListTimesheetQuery, PagingDataSource<TimeSheetDto>>, GetListTimesheetQueryHandler>();
            services.AddScoped<IRequestHandler<GetTimeSheetByIdQuery, TimeSheetDto>, GetTimeSheetByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetHolidayInDateRangeQuery, List<HolidayDto>>, GetHolidayInDateRangeQueryHandler>();

            services.AddScoped<IRequestHandler<GetEmployeeByIdsQuery, List<EmployeeDto>>, GetEmployeeByIdsQueryHandler>();
            services.AddScoped<IRequestHandler<GetFingerMachineByMachineIdQuery, FingerMachineDto>, GetFingerMachineByMachineIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetFingerMachineByMachineIdQuery, FingerMachineDto>, GetFingerMachineByMachineIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetFingerMachineByMachineIdQuery, FingerMachineDto>, GetFingerMachineByMachineIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetShiftByIdsQuery, List<ShiftDto>>, GetShiftByIdsQueryHandler>();

            services.AddScoped<IRequestHandler<GetListGpsInfoQuery, PagingDataSource<GpsInfoDto>>, GetListGpsInfoQueryHandler>();
            services.AddScoped<IRequestHandler<GetListConfirmClockingQuery, PagingDataSource<ConfirmClockingDto>>, GetListConfirmClockingHandler>();
            services.AddScoped<IRequestHandler<GetListConfirmClockingHistoryQuery, PagingDataSource<ConfirmClockingHistoryDto>>, GetListConfirmClockingHistoryHandler>();

            //add scope GpsInfo
            services.AddScoped<IRequestHandler<GetGpsInfoByIdQuery, GpsInfoDto>, GetGpsInfoByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetGpsInfoByQrCodeQuery, GpsInfoDto>, GetGpsInfoByQrCodeQueryHandler> ();
            services.AddScoped<IRequestHandler<GetTwoFaPinQuery, string>, GetTwoFaPinQueryHandler>();
            services.AddScoped<IRequestHandler<CheckTwoFaPinQuery, bool>, CheckTwoFaPinQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeByIdentityKeyForClockingGpsQuery, EmployeeDto>, GetEmployeeByIdentityKeyForClockingGpsQueryHandler>();
            services.AddScoped<IRequestHandler<GetGpsInfoForClockingGpsQuery, GpsInfoDto>, GetGpsInfoForClockingGpsQueryHandler>();
            services.AddScoped<IRequestHandler<GetTotalGprsQuery, long>, GetTotalGprsQueryHandler>();

            // GPS Timekeeping
            services.AddScoped<IRequestHandler<GetClockingsForClockingGpsQuery, ClockingGpsDto>, GetClockingsForClockingGpsQueryHandler>();
            services.AddScoped<IRequestHandler<GetEmployeeForClockingGpsQuery, object>, GetEmployeeForClockingGpsQueryHandler>();

            // Mobile 
            services.AddScoped<IRequestHandler<GetTimesheetConfigForMobileQuery, object>, GetTimesheetConfigForMobileQueryHandler>();
            services.AddScoped<IRequestHandler<GetBranchsForMobileQuery, List<BranchMobileDto>>, GetBranchsForMobileQueryHandler>();
            services.AddScoped<IRequestHandler<GetUserAccountQuery, UserAccountDto>, GetUserAccountQueryHandler>();

            services.AddScoped<IRequestHandler<GetConfirmClockingsByBranchIdQuery, List<ConfirmClockingDto>>, GetConfirmClockingsByBranchIdQueryHandler>();

        }
    }
}
