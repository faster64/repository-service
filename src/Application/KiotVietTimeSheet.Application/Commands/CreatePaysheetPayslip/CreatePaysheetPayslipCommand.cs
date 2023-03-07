using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.Application.Commands.CreatePaysheetPayslip
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Create)]
    public class CreatePaySheetPayslipCommand : BaseCommand<PaysheetDto>
    {
        public PaysheetDto PaySheetDto { get; set; }
        public List<EmployeeDto> ListEmployeeDto { get; set; }
        public List<UserByRevenueObject> UserRevenues { get; set; }
        public List<UserByRevenueObject> UserCounselorRevenues { get; set; }
        public List<DeductionDto> ListDeductionDto { get; set; }
        public List<AllowanceDto> ListAllowanceDto { get; set; }
        public List<CommissionDto> ListCommissionDto { get; set; }
        public List<ClockingDto> ListClockingDto { get; set; }
        public List<ClockingDto> ListClockingPaid { get; set; }
        public List<ClockingDto> ListClockingUnPaid { get; set; }
        public List<PayRate> PayRates { get; set; }
        public List<HolidayDto> ListHolidayDto { get; set; }
        public List<ShiftDto> ListShiftDto { get; set; }
        public List<ProductRevenue> UserProductRevenues { get; set; }
        public List<ProductRevenue> UserCounselorProductRevenues { get; set; }
        public List<ProductRevenue> BranchProductRevenues { get; set; }
        public List<ClockingPenalizeDto> ClockingPenalizesDto { get; set; }
        public SettingObjectDto SettingObjectDto { get; set; }

        public CreatePaySheetPayslipCommand(
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
            ListEmployeeDto = listEmployeeDto;
            UserRevenues = userRevenues;
            UserCounselorRevenues = userCounselorRevenues;
            ListDeductionDto = listDeductionDto;
            ListAllowanceDto = listAllowanceDto;
            ListCommissionDto = listCommissionDto;
            ListClockingDto = listClockingDto;
            ListClockingPaid = listClockingPaid;
            ListClockingUnPaid = listClockingUnPaid;
            PayRates = payRatesDto;
            ListHolidayDto = listHolidayDto;
            ListShiftDto = listShiftDto;
            UserProductRevenues = userProductRevenues;
            UserCounselorProductRevenues = userCounselorProductRevenues;
            BranchProductRevenues = branchProductRevenues;
            ClockingPenalizesDto = clockingPenalizesDto;
            SettingObjectDto = settingObjectDto;
        }
    }
}
