using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;

namespace KiotVietTimeSheet.Application.Commands.CreatePaysheet
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Create)]
    public class CreatePaysheetCommand : BaseCommand<PaysheetDto>
    {
        public byte SalaryPeriod { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<BranchDto> BranchesDto { get; set; }

        public CreatePaysheetCommand(byte salaryPeriod, DateTime startTime, DateTime endTime, List<BranchDto> branchesDto)
        {
            SalaryPeriod = salaryPeriod;
            StartTime = startTime;
            EndTime = endTime;
            BranchesDto = branchesDto;
        }
    }
}
