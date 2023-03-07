using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.BatchAddTimeSheetWhenCreateMultipleTimeSheet
{
    public class BatchAddTimeSheetWhenCreateMultipleTimeSheetCommand : BaseCommand<List<TimeSheetDto>>
    {
        public TimeSheetDto TimeSheet { get; set; }
        public List<long> EmployeeIds { get; set; }
        public bool IsAuto { get; set; }

        public BatchAddTimeSheetWhenCreateMultipleTimeSheetCommand(TimeSheetDto timeSheet, List<long> employeeIds, bool isAuto)
        {
            TimeSheet = timeSheet;
            EmployeeIds = employeeIds;
            IsAuto = isAuto;
        }
    }
}
