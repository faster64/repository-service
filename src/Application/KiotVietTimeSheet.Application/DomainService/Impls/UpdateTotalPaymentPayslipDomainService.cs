using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class UpdateTotalPaymentPayslipDomainService : IUpdateTotalPaymentPayslipDomainService
    {
        private readonly IPayslipWriteOnlyRepository _payslipWriteOnlyRepository;

        public UpdateTotalPaymentPayslipDomainService(IPayslipWriteOnlyRepository payslipWriteOnlyRepository)
        {
            _payslipWriteOnlyRepository = payslipWriteOnlyRepository;
        }

        public async Task UpdateTotalPaymentWithAmoutForPayslipsAsync(Dictionary<long, decimal> dicUpdate)
        {
            var payslips =
                await _payslipWriteOnlyRepository.GetBySpecificationAsync(new FindPayslipByIdsSpec(dicUpdate.Keys));

            payslips.ForEach(payslip =>
            {
                if (dicUpdate.ContainsKey(payslip.Id))
                {
                    payslip.UpdateTotalPaymentWithAmout(dicUpdate[payslip.Id]);
                }
            });

            _payslipWriteOnlyRepository.BatchUpdate(payslips);
            await _payslipWriteOnlyRepository.UnitOfWork.CommitAsync();
        }

        public Task CancelPayslipsAndResetTotalPayment(List<Payslip> payslips)
        {
            throw new NotImplementedException();
        }
    }
}
