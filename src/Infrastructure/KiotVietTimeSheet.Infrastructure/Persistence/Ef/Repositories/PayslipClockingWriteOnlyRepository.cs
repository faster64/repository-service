using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PayslipClockingWriteOnlyRepository : EfBaseWriteOnlyRepository<PayslipClocking>, IPayslipClockingWriteOnlyRepository
    {
        #region Properties
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;

        #endregion
        public PayslipClockingWriteOnlyRepository(
            EfDbContext db,
            IAuthService authService,
            ILogger<PayslipClockingWriteOnlyRepository> logger,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository
        ) : base(db, authService, logger)
        {
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
        }
        public async Task CreateOrUpdateAsync(List<Payslip> payslips, DateTime startTime, DateTime endTime)
        {
            var payslipClockingsTemp = new List<PayslipClocking>();
            var isUpdate = false;
            foreach (var p in payslips)
            {
                if (p.PayslipClockings == null)
                {
                    isUpdate = true;
                    continue;
                }

                p.PayslipClockings.ForEach(c =>
                {
                    var payslipClockingTemp = new PayslipClocking(
                        p.Id,
                        c.ClockingId,
                        c.CheckInDate,
                        c.CheckOutDate,
                        c.TimeIsLate,
                        c.OverTimeBeforeShiftWork,
                        c.TimeIsLeaveWorkEarly,
                        c.OverTimeAfterShiftWork,
                        c.AbsenceType,
                        c.ClockingStatus,
                        c.StartTime,
                        c.EndTime,
                        c.ShiftId
                    );
                    payslipClockingsTemp.Add(payslipClockingTemp);
                });
            }

            var payslipIds = payslips.Select(p => p.Id).Distinct().ToList();
            var payslipClockings = await GetBySpecificationAsync(new FindPayslipClockingByPayslipIds(payslipIds));
            
            if (payslipClockings != null && payslipClockings.Any() && !isUpdate)
            {
                BatchDelete(payslipClockings);

                var employeeIds = payslips.Select(payslip => payslip.EmployeeId).ToList();
                var clockings = await _clockingWriteOnlyRepository.GetClockingsForPaysheet(startTime, endTime, employeeIds);

                if (clockings == null || !clockings.Any())
                {
                    return;
                }

                payslips.ForEach(payslip =>
                {
                    payslip.PayslipClockings = new List<PayslipClocking>();
                    var clockingOfPayslip = clockings.Where(clocking => clocking.EmployeeId == payslip.EmployeeId && clocking.ClockingPaymentStatus == (byte)ClockingPaymentStatuses.UnPaid).ToList();
                    clockingOfPayslip.ForEach(c =>
                    {
                        var payslipClocking = new PayslipClocking(
                            payslip.Id,
                            c.Id,
                            c.CheckInDate,
                            c.CheckOutDate,
                            payslipClockingsTemp.Where(x => x.ClockingId == c.Id).Select(x => x.TimeIsLate).FirstOrDefault(),
                            c.OverTimeBeforeShiftWork,
                            payslipClockingsTemp.Where(x => x.ClockingId == c.Id).Select(x => x.TimeIsLeaveWorkEarly).FirstOrDefault(),
                            c.OverTimeAfterShiftWork,
                            c.AbsenceType,
                            c.ClockingStatus,
                            c.StartTime,
                            c.EndTime,
                            c.ShiftId
                        );
                        payslip.PayslipClockings.Add(payslipClocking);
                    });

                    BatchAdd(payslip.PayslipClockings);
                });
            }
        }
    }
}
