using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.DomainService.Dto
{
    public class GetTimeSheetByBranchWorkingDaysDto
    {
        public int BranchId { get; set; }
        public List<byte> WorkingDays { get; set; }
        public DateTime ApplyFrom { get; set; }

    }
}