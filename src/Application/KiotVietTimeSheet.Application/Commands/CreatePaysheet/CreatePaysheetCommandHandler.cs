using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PaysheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreatePaysheet
{
    public class CreatePaysheetCommandHandler : BaseCommandHandler,
        IRequestHandler<CreatePaysheetCommand, PaysheetDto>
    {
        private readonly IMapper _mapper;
        private readonly IWorkingDayForPaysheetDomainService _workingDayForPaySheetDomainService;
        private readonly IAuthService _authService;
        private readonly IPaysheetWriteOnlyRepository _paySheetWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IMediator _mediator;

        public CreatePaysheetCommandHandler(
            IAuthService authService,
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IWorkingDayForPaysheetDomainService workingDayForPaySheetDomainService,
            IPaysheetWriteOnlyRepository paySheetWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IMediator mediator
        )
            : base(eventDispatcher)
        {
            _authService = authService;
            _mapper = mapper;
            _workingDayForPaySheetDomainService = workingDayForPaySheetDomainService;
            _mediator = mediator;
            _paySheetWriteOnlyRepository = paySheetWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
        }

        public async Task<PaysheetDto> Handle(CreatePaysheetCommand request, CancellationToken cancellationToken)
        {
            var checkExistsPaySheetHasPending =
                await _paysheetReadOnlyRepository.GetBySpecificationAsync(
                    new FindPaysheetByStatusSpec((byte) PaysheetStatuses.Pending).And(new FindPaysheetByBranchId(_authService.Context.BranchId)));

            if (checkExistsPaySheetHasPending != null && checkExistsPaySheetHasPending.Count >= 2)
            {
                NotifyValidationErrors(typeof(CreateOrUpdatePaysheetValidator), new List<string>() { Message.paysheet_created_max_two_times });
                return null;
            }

            // Lấy số ngày công chuẩn
            var standardWorkingDayNumber = await _workingDayForPaySheetDomainService.GetWorkingDayPaysheetAsync(_authService.Context.BranchId, request.StartTime, request.EndTime);
            // Lấy thời gian ngày công chuẩn trong thiết lập cửa hàng

            var settingsObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);

            var timeOfStandardWorkingDay = settingsObjectDto.StandardWorkingDay;

            var paySheet = new Paysheet(
                string.Empty,
                string.Empty,
                standardWorkingDayNumber,
                request.SalaryPeriod,
                request.StartTime,
                request.EndTime,
                (byte)(PaysheetStatuses.Pending),
                _authService.Context.User.Id,
                false,
                timeOfStandardWorkingDay
            );

            _paySheetWriteOnlyRepository.Add(paySheet);

            await _paySheetWriteOnlyRepository.UnitOfWork.CommitAsync();
            await _timeSheetIntegrationEventService.AddEventAsync(
                new CreatePaysheetEmptyIntegrationEvent(paySheet.Id, request.BranchesDto));
            return _mapper.Map<PaysheetDto>(paySheet);
        }

    }
}
