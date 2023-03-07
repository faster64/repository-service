using KiotVietTimeSheet.Application.Abstractions;
using MediatR;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.SharedKernel.Domain;
using System.Threading.Tasks;
using System.Threading;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;

namespace KiotVietTimeSheet.Application.Commands.UpdatePayslip
{
    public class UpdatePayslipCommandHandler : BaseCommandHandler, IRequestHandler<UpdatePayslipCommand, Unit>
    {
        private readonly IPayslipWriteOnlyRepository _payslipWriteOnlyRepository;
        private readonly IPayslipReadOnlyRepository _payslipReadOnlyRepository;

        public UpdatePayslipCommandHandler(
            IEventDispatcher eventDispatcher,
            IPayslipWriteOnlyRepository payslipWriteOnlyRepository,
            IPayslipReadOnlyRepository payslipReadOnlyRepository
        )
            : base(eventDispatcher)
        {
            _payslipWriteOnlyRepository = payslipWriteOnlyRepository;
            _payslipReadOnlyRepository = payslipReadOnlyRepository;
        }

        public async Task<Unit> Handle(UpdatePayslipCommand request, CancellationToken cancellationToken)
        {
            var payslip = await _payslipReadOnlyRepository.FindByIdAsync(request.PayslipId);
            if (payslip == null || !request.TotalPayment.HasValue) return Unit.Value;
            payslip.UpdateTotalPayment(request.TotalPayment.Value);
            await _payslipWriteOnlyRepository.UpdateTotalPaymentAsync(payslip);
            await _payslipWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
