using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using System;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class CreatePaySheetWorkerIntegrationEvent : IntegrationEvent
    {
        public List<Payslip> Payslips { get; set; }
        public int TenantId { get; set; }
        public string TenantCode { get; set; }
        public int GroupId { get; set; }
        public List<long> EmployeeIds { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public byte SalaryPeriod { get; set; }
        public int StandardWorkingDayNumber { get; set; }
        public int TimeOfStandardWorkingDay { get; set; }
        public int BranchId { get; set; }
        public long? PayslipCreatedBy { get; set; }
        public DateTime? PayslipCreatedDate { get; set; }
        public AuthService AuthService { get; set; }
        public bool IsUpdatePaySheet { get; set; }
        public IList<BranchDto> AllBranchs { get; set; }
        public CreatePaySheetWorkerIntegrationEvent(int tenantId, string tenantCode, int groupId, IList<BranchDto> allBranchs, List<Payslip> payslips, List<long> employeeIds, DateTime from,
            DateTime to, byte salaryPeriod, int standardWorkingDayNumber, int timeOfStandardWorkingDay, int branchId, bool isUpdatePaySheet, long? payslipCreatedBy = null,
            DateTime? payslipCreatedDate = null, AuthService authService = null)
        {
            TenantId = tenantId;
            TenantCode = tenantCode;
            GroupId = groupId;
            Payslips = payslips;
            EmployeeIds = employeeIds;
            From = from;
            To = to;
            TimeOfStandardWorkingDay = timeOfStandardWorkingDay;
            BranchId = branchId;
            PayslipCreatedBy = payslipCreatedBy;
            PayslipCreatedDate = payslipCreatedDate;
            SalaryPeriod = salaryPeriod;
            StandardWorkingDayNumber = standardWorkingDayNumber;
            AuthService = authService;
            IsUpdatePaySheet = isUpdatePaySheet;
            AllBranchs = allBranchs;
        }

        [JsonConstructor]
        public CreatePaySheetWorkerIntegrationEvent()
        {

        }
    }
}
