using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPayslipPenalizeReadOnlyRepository : IBaseReadOnlyRepository<PayslipPenalize, long>
    {
        Task<List<PayslipPenalizeDto>> GetPayslipsPenalizes(List<long> payslipIds);
    }
}
