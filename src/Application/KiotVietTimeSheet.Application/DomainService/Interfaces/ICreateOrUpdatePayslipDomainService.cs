using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface ICreateOrUpdatePayslipDomainService
    {
        Task<PayslipDomainServiceDto> BatchCreateAsync(List<Payslip> payslips, DateTime startTime, DateTime endTime);
    }
}
