using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CancelPayslip
{
    public class CancelPayslipCommandHandler : BaseCommandHandler,
        IRequestHandler<CancelPayslipCommand, PayslipDto>
    {
        private readonly IPayslipWriteOnlyRepository _payslipWriteOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ICancelPayslipDomainService _cancelPayslipDomainService;

        public CancelPayslipCommandHandler(
            IEventDispatcher eventDispatcher,
            IPayslipWriteOnlyRepository payslipWriteOnlyRepository,
            IMapper mapper,
            IKiotVietServiceClient kiotVietServiceClient,
            ICancelPayslipDomainService cancelPayslipDomainService
        )
            : base(eventDispatcher)
        {
            _payslipWriteOnlyRepository = payslipWriteOnlyRepository;
            _mapper = mapper;
            _kiotVietServiceClient = kiotVietServiceClient;
            _cancelPayslipDomainService = cancelPayslipDomainService;
        }

        public async Task<PayslipDto> Handle(CancelPayslipCommand request, CancellationToken cancellationToken)
        {
            var payslip = await _payslipWriteOnlyRepository.FindBySpecificationAsync(new FindPayslipByIdSpec(request.Id));
            if (payslip == null) return new PayslipDto();
            var payslipIds = new List<long>() { payslip.Id };
            var payslips = new List<PayslipDto> { _mapper.Map<PayslipDto>(payslip) };

            if (request.IsCheckPayslipPayment)
            {
                var payslipPayments =
                    await _kiotVietServiceClient.GetPayslipPaymentsAsync(
                        new GetPayslipPaymentsReq
                        {
                            PayslipIds = payslipIds,
                            Status = (byte)PayslipPaymentStatuses.Paid,
                            WithAllocation = false
                        });
                if (payslipPayments != null && payslipPayments.Data.Any()) return null;
            }

            await _cancelPayslipDomainService.CancelAsync(payslipIds);

            #region Hủy phiếu thanh toán

            await _kiotVietServiceClient.VoidPayslipPaymentsAsync(
                new VoidPayslipPaymentReq
                {
                    Payslips = payslips,
                    IsVoidPayslipPayment = request.IsCancelPayment,
                    IsUpdatePaysheetTracking = true
                });

            #endregion

            await _payslipWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<PayslipDto>(payslip);
        }

    }
}
