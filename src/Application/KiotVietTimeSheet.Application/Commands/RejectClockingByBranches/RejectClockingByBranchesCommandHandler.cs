using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;

namespace KiotVietTimeSheet.Application.Commands.RejectClockingByBranches
{
    public class RejectClockingByBranchesCommandHandler : BaseCommandHandler,
        IRequestHandler<RejectClockingByBranchesCommand, List<ClockingDto>>
    {
        private readonly IMapper _mapper;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IRejectClockingsDomainService _rejectClockingsService;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public RejectClockingByBranchesCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IRejectClockingsDomainService rejectClockingsService,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        ): base(eventDispatcher)
        {
            _mapper = mapper;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _rejectClockingsService = rejectClockingsService;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<List<ClockingDto>> Handle(RejectClockingByBranchesCommand request, CancellationToken cancellationToken)
        {
            var iforAllClockings = request.IforAllClockings;
            var startTime = request.StartTime;
            var employeeId = request.EmployeeId;
            var statuses = request.Statuses;
            var branchIds = request.BranchIds;
            // Get data from database
            startTime = startTime.Date;
            var clockings = new List<Clocking>();
            foreach (var branchId in branchIds)
            {
                var specification = new FindClockingByBranchIdSpec(branchId)
                    .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void));
                if (!iforAllClockings)
                {
                    specification = specification.And(new FindClockingByStartTimeGreaterThanOrEqualSpec(startTime));

                }
                specification = specification.And(new FindClockingByEmployeeIdsSpec(new List<long>() { employeeId }));

                specification = specification.And(new FindClockingByStatusesSpec(new List<byte>() { statuses }));
                clockings.AddRange(await _clockingWriteOnlyRepository.GetBySpecificationAsync(specification));
            }

            // End
            if (!clockings.Any())
            {
                return new List<ClockingDto>();
            }

            var returnObj = await _rejectClockingsService.RejectClockingsAsync(clockings);
            if (!returnObj)
            {
                return null;
            }

            await _paySheetOutOfDateDomainService.WithClockingDataChangeAsync(clockings
                .Where(x => (x.CheckOutDate != null || x.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence) && x.ClockingPaymentStatus != (byte)ClockingPaymentStatuses.Paid).ToList());

            await _timeSheetIntegrationEventService.AddEventAsync(new RejectMultipleClockingIntegrationEvent(clockings));

            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<List<ClockingDto>>(clockings);
            return result;
        }
    }
}

