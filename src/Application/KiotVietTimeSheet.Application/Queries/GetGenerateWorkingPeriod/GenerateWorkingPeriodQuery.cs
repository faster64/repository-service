using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetGenerateWorkingPeriod
{
    public class GenerateWorkingPeriodQuery : QueryBase<List<PaySheetWorkingPeriodDto>>
    {
        public int SalaryPeriodType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsUpdatePaysheet { get; set; }

        public GenerateWorkingPeriodQuery(int salaryPeriodType, DateTime startDate, DateTime endDate, bool isUpdatePaysheet)
        {
            SalaryPeriodType = salaryPeriodType;
            StartDate = startDate;
            EndDate = endDate;
            IsUpdatePaysheet = isUpdatePaysheet;
        }
    }
}
