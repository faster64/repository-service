using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class CompleteSalaryClockingDomainService : ICompleteSalaryClockingDomainService
    {
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly IClockingPenalizeWriteOnlyRepository _clockingPenalizeWriteOnly;
        private readonly IPayslipClockingPenalizeReadOnlyRepository _payslipClockingPenalizeReadOnlyRepository;
        private readonly IPayslipClockingReadOnlyRepository _payslipClockingReadOnlyRepository;

        public CompleteSalaryClockingDomainService(
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            IClockingPenalizeWriteOnlyRepository clockingPenalizeWriteOnly,
            IPayslipClockingPenalizeReadOnlyRepository payslipClockingPenalizeReadOnlyRepository,
            IPayslipClockingReadOnlyRepository payslipClockingReadOnlyRepository)
        {
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _clockingPenalizeWriteOnly = clockingPenalizeWriteOnly;
            _payslipClockingPenalizeReadOnlyRepository = payslipClockingPenalizeReadOnlyRepository;
            _payslipClockingReadOnlyRepository = payslipClockingReadOnlyRepository;
        }
        public async Task CompletePaysheetForClockingsAsync(Paysheet paysheet)
        {
            var payslipIds = paysheet.Payslips?.Where(x => x.PayslipStatus != (byte)PayslipStatuses.Void).Select(x => x.Id).ToList();
            var payslipClockings = await _payslipClockingReadOnlyRepository.GetBySpecificationAsync(new FindPayslipClockingByPayslipIds(payslipIds));

            var clockingIds = payslipClockings.Select(x => x.ClockingId).ToList();

            var clockings = await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByIdsSpec(clockingIds));
            foreach (var clocking in clockings)
            {
                clocking.UpdateClockingPaymentStatus((byte)ClockingPaymentStatuses.Paid);
                clocking.ClockingPenalizes?.ForEach(item =>
                {
                    item.UpdateClockingPaymentStatus((byte)ClockingPaymentStatuses.Paid);
                });
            }

            var payslipClockingPenalizeForPenalizes =
                await _payslipClockingPenalizeReadOnlyRepository.GetBySpecificationAsync(
                    new FindPayslipClockingPenalizeByPayslipIds(payslipIds));

            var clockingIdsForPenalizes = payslipClockingPenalizeForPenalizes
                    .Select(x => x.ClockingId)
                    .ToList();

            var clockingForPenalizes = await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByIdsSpec(clockingIdsForPenalizes), "ClockingPenalizes");
            foreach (var clocking in clockingForPenalizes)
            {
                clocking.ClockingPenalizes?.ForEach(item =>
                {
                    item.UpdateClockingPaymentStatus((byte)ClockingPaymentStatuses.Paid);
                });
            }

            var clockingPenalizeReadyToUpdate = clockingForPenalizes.Where(x => x.ClockingPenalizes != null).SelectMany(x => x.ClockingPenalizes).ToList();

            await _paySheetOutOfDateDomainService.WithClockingDataChangeAsync(clockings, paysheet.Id);
            _clockingWriteOnlyRepository.BatchUpdate(clockings);
            _clockingPenalizeWriteOnly.BatchUpdate(clockingPenalizeReadyToUpdate);
        }
    }
}
