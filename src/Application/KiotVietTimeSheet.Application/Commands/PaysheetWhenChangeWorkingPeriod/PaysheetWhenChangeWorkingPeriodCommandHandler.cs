using System;
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
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.PaysheetWhenChangeWorkingPeriod
{
    public class PaysheetWhenChangeWorkingPeriodCommandHandler : BaseCommandHandler,
        IRequestHandler<PaysheetWhenChangeWorkingPeriodCommand, PaysheetDto>
    {
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IWorkingDayForPaysheetDomainService _workingDayForPaysheetDomainService;
        private readonly IPaysheetWriteOnlyRepository _paysheetWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public PaysheetWhenChangeWorkingPeriodCommandHandler(
            IAuthService ausService,
            IEventDispatcher eventDispatcher,
            IPaysheetReadOnlyRepository paysheetReadOnlyRepository,
            IMediator mediator,
            IWorkingDayForPaysheetDomainService workingDayForPaysheetDomainService,
            IPaysheetWriteOnlyRepository paysheetWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IMapper mapper
        )
            : base(eventDispatcher)
        {
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
            _eventDispatcher = eventDispatcher;
            _authService = ausService;
            _workingDayForPaysheetDomainService = workingDayForPaysheetDomainService;
            _paysheetWriteOnlyRepository = paysheetWriteOnlyRepository;
            _mapper = mapper;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _mediator = mediator;
        }

        public async Task<PaysheetDto> Handle(PaysheetWhenChangeWorkingPeriodCommand request, CancellationToken cancellationToken)
        {
            var paysheetDto = request.PaysheetDto;
            var paySheet = await _paysheetReadOnlyRepository.FindBySpecificationAsync(
                new FindPaysheetByIdSpec(paysheetDto.Id).And(new FindPaysheetByBranchId(paysheetDto.BranchId)).And(
                    new FindPaysheetByStatuses(new List<byte> { (byte)PaysheetStatuses.TemporarySalary, (byte)PaysheetStatuses.Draft })), true);

            if (paySheet == null)
            {
                NotifyPaysheetInDbIsNotExists();
                return null;
            }

            var paysheetOldDto = new PaysheetDto
            {
                Id = paySheet.Id,
                Name = paySheet.Name,
                TimeOfStandardWorkingDay = paySheet.TimeOfStandardWorkingDay ?? 0,
                WorkingDayNumber = paySheet.WorkingDayNumber,
                PaysheetPeriodName = paySheet.PaysheetPeriodName,
                StartTime = paySheet.StartTime,
                EndTime = paySheet.EndTime,
                PaysheetCreatedDate = paySheet.PaysheetCreatedDate
            };

            var paySheetCreateTimeOld = paySheet.PaysheetCreatedDate;
            // Lấy thời gian ngày công chuẩn trong thiết lập cửa hàng
            var settingsObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);
            var timeOfStandardWorkingDay = settingsObjectDto.StandardWorkingDay;
            // Lấy số ngày công chuẩn
            var standardWorkingDayNumber =
                await _workingDayForPaysheetDomainService.GetWorkingDayPaysheetAsync(paysheetDto.BranchId,
                    paysheetDto.StartTime, paysheetDto.EndTime);

            paySheet.TimeOfStandardWorkingDay = timeOfStandardWorkingDay;
            paySheet.WorkingDayNumber = standardWorkingDayNumber;
            var paysheetPeriodName = paysheetDto.StartTime.ToString("dd/MM/yyyy") + " - " + paysheetDto.EndTime.ToString("dd/MM/yyyy");
            paySheet.Name = "Bảng lương " + paysheetPeriodName;
            paySheet.IsDraft = false;
            paySheet.PaysheetStatus = (byte) PaysheetStatuses.Pending;
            paySheet.UpdatePeriodName(paysheetPeriodName);
            paySheet.UpdateStartAndEndTime(paysheetDto.StartTime, paysheetDto.EndTime);
            paySheet.PaysheetCreatedDate = DateTime.Now;

            _paysheetWriteOnlyRepository.Update(paySheet);
            
            await _timeSheetIntegrationEventService.AddEventAsync(
                new ChangePeriodPaysheetIntegrationEvent(paySheet.Id, request.BranchesDto, paySheetCreateTimeOld, paysheetOldDto));

            await _paysheetWriteOnlyRepository.UnitOfWork.CommitAsync();
            var resultPaySheetDto = _mapper.Map<PaysheetDto>(paySheet);
            resultPaySheetDto.IsChanged = false;
            return resultPaySheetDto;
        }

        private void NotifyPaysheetInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Paysheet).Name, string.Format(Message.not_exists, Label.paysheet)));
        }
    }
}
