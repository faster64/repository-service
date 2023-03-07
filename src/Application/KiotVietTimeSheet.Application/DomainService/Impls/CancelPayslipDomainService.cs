using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class CancelPayslipDomainService : ICancelPayslipDomainService
    {
        #region PROPERTIES
        private readonly IPayslipWriteOnlyRepository _payslipWriteOnlyRepository;
        private readonly IPayslipClockingWriteOnlyRepository _payslipClockingWriteOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IPayslipClockingPenalizeWriteOnlyRepository _payslipClockingPenalizeWriteOnlyRepository;
        private readonly IClockingPenalizeWriteOnlyRepository _clockingPenalizeWriteOnly;
        #endregion

        #region CONSTRUCTORS

        public CancelPayslipDomainService(
            IPayslipWriteOnlyRepository payslipWriteOnlyRepository,
            IPayslipClockingWriteOnlyRepository payslipClockingWriteOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IPayslipClockingPenalizeWriteOnlyRepository payslipClockingPenalizeWriteOnlyRepository,
            IClockingPenalizeWriteOnlyRepository clockingPenalizeWriteOnly)
        {
            _payslipWriteOnlyRepository = payslipWriteOnlyRepository;
            _payslipClockingWriteOnlyRepository = payslipClockingWriteOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _payslipClockingPenalizeWriteOnlyRepository = payslipClockingPenalizeWriteOnlyRepository;
            _clockingPenalizeWriteOnly = clockingPenalizeWriteOnly;
        }

        #endregion

        #region PUBLIC METHODS

        public async Task CancelAsync(List<long> ids, bool withoutEvent = false)
        {
            var spec = new FindPayslipByIdsSpec(ids).Not(new FindPayslipByStatusSpec((byte)PayslipStatuses.Void));
            var payslips = await _payslipWriteOnlyRepository.GetBySpecificationAsync(spec);
            var paidPayslipIds = payslips.Where(x => x.PayslipStatus == (byte)PayslipStatuses.PaidSalary).Select(x => x.Id).ToList();

            payslips.ForEach(payslip =>
            {
                if (withoutEvent)
                    payslip.CancelWithoutEvent();
                else
                    payslip.Cancel();
            });

            var payslipIds = payslips.Select(p => p.Id).ToList();


            var payslipClockings = await _payslipClockingWriteOnlyRepository
                .GetBySpecificationAsync(new FindPayslipClockingByPayslipIds(payslipIds));


            var paidPayslipClocking = payslipClockings.Where(x => paidPayslipIds.Contains(x.PayslipId)).ToList();
            if (paidPayslipClocking.Any())
            {
                var clockings = await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByIdsSpec(paidPayslipClocking.Select(c => c.ClockingId).ToList()));

                // Chuyển trạng thái lịch làm việc trong phiếu lương về trạng thái chưa thanh toán
                clockings.ForEach(c =>
                {
                    c.UpdateClockingPaymentStatus((byte)ClockingPaymentStatuses.UnPaid);
                });

                _clockingWriteOnlyRepository.BatchUpdate(clockings);
            }

            var payslipClockingPenalizes =
                await _payslipClockingPenalizeWriteOnlyRepository.GetBySpecificationAsync(
                    new FindPayslipClockingPenalizeByPayslipIds(ids));

            if (payslipClockingPenalizes != null && payslipClockingPenalizes.Any())
            {
                foreach (var pcp in payslipClockingPenalizes)
                {
                    pcp.Delete();
                }

                var clockingIdsForPenalizes = payslipClockingPenalizes.Select(x => x.ClockingId).ToList();

                var clockingForPenalizes = await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByIdsSpec(clockingIdsForPenalizes), "ClockingPenalizes");
                foreach (var clocking in clockingForPenalizes)
                {
                    clocking.ClockingPenalizes?.ForEach(item =>
                    {
                        item.UpdateClockingPaymentStatus((byte)ClockingPaymentStatuses.UnPaid);
                    });
                }

                var clockingPenalizeReadyToUpdate = clockingForPenalizes.Where(x => x.ClockingPenalizes != null).SelectMany(x => x.ClockingPenalizes).ToList();

                _payslipClockingPenalizeWriteOnlyRepository.BatchUpdate(payslipClockingPenalizes);
                _clockingPenalizeWriteOnly.BatchUpdate(clockingPenalizeReadyToUpdate);
            }
            
            _payslipWriteOnlyRepository.RemovePayslipClockings(payslipClockings);
            _payslipWriteOnlyRepository.BatchUpdate(payslips);
            await _payslipWriteOnlyRepository.UnitOfWork.CommitAsync();
        }

        #endregion
    }
}
