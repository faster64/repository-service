using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PaysheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.AutoLoadingAndUpdatePaySheet
{
    public class AutoLoadingAndUpdatePaysheetCommandHandler : BaseCommandHandler,
        IRequestHandler<AutoLoadingAndUpdatePaysheetCommand, PaysheetDto>
    {
        private readonly IMapper _mapper;
        private readonly IWorkingDayForPaysheetDomainService _workingDayForPaySheetDomainService;
        private readonly IPaysheetWriteOnlyRepository _paySheetWriteOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paySheetReadOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;

        public AutoLoadingAndUpdatePaysheetCommandHandler(
            IEventDispatcher eventDispatcher, 
            IMapper mapper,
            IWorkingDayForPaysheetDomainService workingDayForPaySheetDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IPaysheetWriteOnlyRepository paySheetWriteOnlyRepository,
            IPaysheetReadOnlyRepository paySheetReadOnlyRepository,
            IAuthService authService,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IMediator mediator) : base(
            eventDispatcher)
        {
            _mapper = mapper;
            _workingDayForPaySheetDomainService = workingDayForPaySheetDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _paySheetWriteOnlyRepository = paySheetWriteOnlyRepository;
            _paySheetReadOnlyRepository = paySheetReadOnlyRepository;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _authService = authService;
            _mediator = mediator;
        }

        public async Task<PaysheetDto> Handle(AutoLoadingAndUpdatePaysheetCommand request,
            CancellationToken cancellationToken)
        {
            var checkExistsPaySheetHasPending =
                await _paysheetReadOnlyRepository.GetBySpecificationAsync(
                    new FindPaysheetByStatusSpec((byte)PaysheetStatuses.Pending).And(new FindPaysheetByBranchId(_authService.Context.BranchId)));

            if (checkExistsPaySheetHasPending != null && checkExistsPaySheetHasPending.Count >= 2)
            {
                NotifyValidationErrors(typeof(CreateOrUpdatePaysheetValidator), new List<string>() { Message.paysheet_created_max_two_times });
                return null;
            }

            var paySheet = await _paySheetReadOnlyRepository.FindByIdAsync(request.PaySheetId);

            if (paySheet == null || paySheet.IsDeleted)
            {
                var notify = string.Format(Message.is_deletedCheckAgain, Label.paysheet, "");
                NotifyValidationErrors(typeof(CreateOrUpdatePaysheetValidator), new List<string>() { notify });
                return null;
            }

            if (paySheet.PaysheetStatus == (byte)PaysheetStatuses.Pending)
            {
                NotifyValidationErrors(typeof(CreateOrUpdatePaysheetValidator), new List<string>() { Message.paysheet_has_pending });
                return null;
            }

            paySheet.PaysheetStatus = (byte)PaysheetStatuses.Pending;
            var paySheetCreateTimeOld = paySheet.PaysheetCreatedDate;
            _paySheetWriteOnlyRepository.Update(paySheet);

            // Lấy số ngày công chuẩn
            var standardWorkingDayNumber = await _workingDayForPaySheetDomainService.GetWorkingDayPaysheetAsync(_authService.Context.BranchId, paySheet.StartTime, paySheet.EndTime);
            // Lấy thời gian ngày công chuẩn trong thiết lập cửa hàng
            var settingObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);
            var timeOfStandardWorkingDay = settingObjectDto.StandardWorkingDay;


            await _timeSheetIntegrationEventService.AddEventAsync(
                new AutoLoadingAndUpdatePaysheetIntegrationEvent(paySheet.Id, standardWorkingDayNumber, timeOfStandardWorkingDay, request.BranchesDto, paySheetCreateTimeOld));

            await _paySheetWriteOnlyRepository.UnitOfWork.CommitAsync();

            return _mapper.Map<PaysheetDto>(paySheet);
        }
    }
}
