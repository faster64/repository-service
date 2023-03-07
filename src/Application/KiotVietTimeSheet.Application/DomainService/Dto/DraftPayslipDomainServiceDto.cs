using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Dto
{
    public class DraftPayslipDomainServiceDto
    {
        public Payslip Payslip { get; set; }
        public Employee Employee { get; set; }
    }
}
