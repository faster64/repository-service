using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IAutoTimeKeepingDomainService
    {
        Task<List<Clocking>> GetClokingMultiple(DateTime checkDateTime, long employeeId, Clocking clockingTarget, bool isCheckIn, List<Clocking> lsClockingRelate = null);
        Task<List<AutoTimeKeepingResult>> AutoTimeKeepingAsync(List<FingerPrintLogDto> fingerPrintLogs);
    }
}
