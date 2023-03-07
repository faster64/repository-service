using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IPayslipClockingWriteOnlyRepository : IBaseWriteOnlyRepository<PayslipClocking>
    {
        Task CreateOrUpdateAsync(List<Payslip> payslips, DateTime startTime, DateTime endTime);
    }
}
