using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.Application.Commands.UpdatePaySheetPayslips
{
    public class UpdatePaySheetPayslipsCommand : BaseCommand<PaysheetDto>
    {
        public PaysheetDto PaySheetDto { get; set; }
        public List<EmployeeDto> EmployeesDto { get; set; }
        public List<UserByRevenueObject> UserRevenues { get; set; }
        public List<UserByRevenueObject> UserCounselorRevenues { get; set; }
        public List<DeductionDto> DeductionsDto { get; set; }
        public List<AllowanceDto> AllowancesDto { get; set; }
        public List<CommissionDto> CommissionsDto { get; set; }
        public List<ClockingDto> ClockingsDto { get; set; }
        public List<ClockingDto> ClockingsPaid { get; set; }
        public List<ClockingDto> ClockingsUnPaid { get; set; }
        public List<PayRate> PayRates { get; set; }
        public List<HolidayDto> HolidaysDto { get; set; }
        public List<ShiftDto> ShiftsDto { get; set; }
        public List<ProductRevenue> UserProductRevenues { get; set; }
        public List<ProductRevenue> UserCounselorProductRevenues { get; set; }
        public List<ProductRevenue> BranchProductRevenues { get; set; }
        public List<ClockingPenalizeDto> ClockingPenalizesDto { get; set; }
        public SettingObjectDto SettingObjectDto { get; set; }

        public UpdatePaySheetPayslipsCommand(
            PaysheetDto paySheetDto,
            List<EmployeeDto> listEmployeeDto,
            List<UserByRevenueObject> userRevenues,
            List<UserByRevenueObject> userCounselorRevenues,
            List<DeductionDto> listDeductionDto,
            List<AllowanceDto> listAllowanceDto,
            List<CommissionDto> listCommissionDto,
            List<ClockingDto> listClockingDto,
            List<ClockingDto> listClockingPaid,
            List<ClockingDto> listClockingUnPaid,
            List<PayRate> payRatesDto,
            List<HolidayDto> listHolidayDto,
            List<ShiftDto> listShiftDto,
            List<ProductRevenue> userProductRevenues,
            List<ProductRevenue> userCounselorProductRevenues,
            List<ProductRevenue> branchProductRevenues,
            List<ClockingPenalizeDto> clockingPenalizesDto,
            SettingObjectDto settingObjectDto)
        {
            PaySheetDto = paySheetDto;
            EmployeesDto = listEmployeeDto;
            UserRevenues = userRevenues;
            UserCounselorRevenues = userCounselorRevenues;
            DeductionsDto = listDeductionDto;
            AllowancesDto = listAllowanceDto;
            CommissionsDto = listCommissionDto;
            ClockingsDto = listClockingDto;
            ClockingsPaid = listClockingPaid;
            ClockingsUnPaid = listClockingUnPaid;
            PayRates = payRatesDto;
            HolidaysDto = listHolidayDto;
            ShiftsDto = listShiftDto;
            UserProductRevenues = userProductRevenues;
            UserCounselorProductRevenues = userCounselorProductRevenues;
            BranchProductRevenues = branchProductRevenues;
            ClockingPenalizesDto = clockingPenalizesDto;
            SettingObjectDto = settingObjectDto;
        }
    }
}
