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
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CancelPaysheet
{
    public class CancelPaysheetCommandHandler : BaseCommandHandler,
        IRequestHandler<CancelPaysheetCommand, PaysheetDto>
    {
        private readonly IPaysheetWriteOnlyRepository _paysheetWriteOnlyRepository;
        private readonly IPayslipWriteOnlyRepository _payslipWriteOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ICancelPayslipDomainService _cancelPayslipDomainService;
        private readonly IMapper _mapper;

        public CancelPaysheetCommandHandler(
            IEventDispatcher eventDispatcher,
            IPaysheetWriteOnlyRepository paysheetWriteOnlyRepository,
            IPayslipWriteOnlyRepository payslipWriteOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ICancelPayslipDomainService cancelPayslipDomainService,
            IMapper mapper
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _paysheetWriteOnlyRepository = paysheetWriteOnlyRepository;
            _payslipWriteOnlyRepository = payslipWriteOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _cancelPayslipDomainService = cancelPayslipDomainService;
            _mapper = mapper;
        }

        public async Task<PaysheetDto> Handle(CancelPaysheetCommand request, CancellationToken cancellationToken)
        {
            var paySheet = await _paysheetWriteOnlyRepository.FindBySpecificationAsync(new FindPaysheetByIdSpec(request.Id));

            if (paySheet == null)
            {
                NotifyPaySheetInDbIsNotExists();
                return null;
            }

            var errors = new List<string>();
            if (paySheet.PaysheetStatus == (byte)PaysheetStatuses.Void)
            {
                errors.Add(Message.paysheet_haveBeenCancelled);
                NotifyValidationErrors(typeof(Paysheet), errors);
                return null;
            }

            var payslips = await _payslipWriteOnlyRepository.GetBySpecificationAsync(new FindPayslipByPaysheetIdSpec(paySheet.Id));
            if (request.IsCheckPayslipPayment && payslips.Any())
            {
                var payslipPayments =
                    await _kiotVietServiceClient.GetPayslipPaymentsAsync(
                        new GetPayslipPaymentsReq
                        {
                            PaysheetId = paySheet.Id,
                            Status = (byte)PayslipPaymentStatuses.Paid,
                            WithAllocation = false
                        });
                if (payslipPayments != null && payslipPayments.Data.Any()) return null;
            }

            paySheet.Cancel();
            await _cancelPayslipDomainService.CancelAsync(payslips.Select(s => s.Id).ToList(), true);

            // Hủy phiếu thanh toán, phiếu hoàn trả tạm ứng
            await CancelPayslipPayment(payslips, request.IsCancelPayment, true);

            _paysheetWriteOnlyRepository.Update(paySheet);
            await _paysheetWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<PaysheetDto>(paySheet);
        }

        private async Task CancelPayslipPayment(List<Payslip> listPayslipsNeedCancel, bool isCancelPayment, bool cancelPaySheet = false)
        {
            if (listPayslipsNeedCancel != null && listPayslipsNeedCancel.Any())
            {
                await _kiotVietServiceClient.VoidPayslipPaymentsAsync(
                    new VoidPayslipPaymentReq
                    {
                        Payslips = _mapper.Map<List<PayslipDto>>(listPayslipsNeedCancel),
                        IsVoidPayslipPayment = isCancelPayment,
                        IsCancelPaysheet = cancelPaySheet
                    });
            }
        }

        private void NotifyPaySheetInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Paysheet).Name, string.Format(Message.not_existsInSystem, Label.paysheet)));
        }

    }
}
