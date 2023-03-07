using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;

namespace KiotVietTimeSheet.Application.Abstractions
{
    public abstract class QueryHandlerBase
    {
        protected IAuthService AuthService { get; }

        protected QueryHandlerBase(IAuthService authService)
        {
            AuthService = authService;
        }

        public List<PayslipPenalizeDto> GetAndSetNamePayslipPenalizeDto(List<PayslipPenalizeDto> payslipPenalizesDto, List<Penalize> penalizeList)
        {
            var result = payslipPenalizesDto?.Select(x =>
            {
                x.PenalizeName = penalizeList?.FirstOrDefault(p => p.Id == x.PenalizeId)?.Name;
                return x;
            }).OrderByDescending(x => x.TimesCount).ToList();

            return result ?? new List<PayslipPenalizeDto>();
        }

        public List<PayslipClockingPenalizeDto> GetAndSetNamePayslipClockingPenalizeDto(List<PayslipClockingPenalizeDto> payslipClockingPenalizes, List<Shift> shifts)
        {
            var result = payslipClockingPenalizes?.Select(p =>
            {
                p.ShiftName = shifts.FirstOrDefault(shift => shift.Id == p.ShiftId)?.Name;

                return p;
            }).ToList();
            return result ?? new List<PayslipClockingPenalizeDto>();
        }
    }
}
