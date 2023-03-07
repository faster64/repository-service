using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.Commands.RejectClockingByFilter
{
    public class RejectClockingByFilterCommandHandler : BaseCommandHandler,
        IRequestHandler<RejectClockingByFilterCommand, List<ClockingDto>>
    {
        private readonly IMapper _mapper;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IRejectClockingsDomainService _rejectClockingsService;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public RejectClockingByFilterCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IRejectClockingsDomainService rejectClockingsService,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _rejectClockingsService = rejectClockingsService;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<List<ClockingDto>> Handle(RejectClockingByFilterCommand request, CancellationToken cancellationToken)
        {
            var statusesExtension = request.StatusesExtension;
            var startTime = request.StartTime;
            var endTime = request.EndTime;
            var employeeIds = request.EmployeeIds;
            var shiftId = request.ShiftId;
            var branchId = request.BranchId;
            // Get data from database
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;

            var specification = new FindClockingByBranchIdSpec(branchId)
                                .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(startTime))
                                .And(new FindClockingByStartTimeLessThanSpec(endTime))
                                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void));

            if (employeeIds != null && employeeIds.Count > 0)
            {
                specification = specification.And(new FindClockingByEmployeeIdsSpec(employeeIds));
            }

            if (shiftId > 0)
            {
                specification = specification.And(new FindClockingByShiftIdSpec(shiftId));
            }

            if (statusesExtension != null && statusesExtension.Count > 0)
            {
                specification = specification.And(new FindClockingByStatuesExtensionSpec(statusesExtension));
            }

            var clockings = await _clockingWriteOnlyRepository.GetBySpecificationAsync(specification);
            // End

            var errors = new List<string>();
            if (clockings == null || !clockings.Any())
            {
                errors.Add(Message.clocking_cannotFindClockings);

                NotifyValidationErrors(typeof(Clocking), errors);

                return null;
            }

            var returnObj = await _rejectClockingsService.RejectClockingsAsync(clockings);
            if (!returnObj)
            {
                return null;
            }

            await _paySheetOutOfDateDomainService.WithClockingDataChangeAsync(clockings
                .Where(x => (x.CheckOutDate != null || x.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence) && x.ClockingPaymentStatus != (byte)ClockingPaymentStatuses.Paid).ToList());

            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync();

            var timeSheet = new TimeSheet();
            timeSheet.SetRepeatWithDate(true, request.StartTime, request.EndTime);
            await _timeSheetIntegrationEventService.AddEventAsync(new RejectMultipleClockingIntegrationEvent(timeSheet, clockings));

            var result = _mapper.Map<List<ClockingDto>>(clockings);
            return result;
        }
    }
}
