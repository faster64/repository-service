using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.CancelTimeSheet
{
    public class CancelTimeSheetCommandHandler : BaseCommandHandler,
        IRequestHandler<CancelTimeSheetCommand, Unit>
    {
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IEventDispatcher _eventDispatcher;

        public CancelTimeSheetCommandHandler(
            IEventDispatcher eventDispatcher,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<Unit> Handle(CancelTimeSheetCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _timeSheetWriteOnlyRepository.FindBySpecificationAsync(new FindTimeSheetByIdSpec(request.Id));

            if (timeSheet == null)
            {
                NotifyTimeSheetInDbIsNotExists();
                return new Unit();
            }

            var errors = new List<string>();
            if (timeSheet.TimeSheetStatus == (byte)TimeSheetStatuses.Void)
            {
                errors.Add(Message.timeSheet_haveBeenCancelled);
                NotifyValidationErrors(typeof(Clocking), errors);
                return new Unit();
            }

            timeSheet.Cancel();
            var timeSheetClockings =
                await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByTimeSheetIdSpec(request.Id));
            if (timeSheetClockings != null)
            {
                var clockings = timeSheetClockings.Where(c => c.ClockingStatus == (byte)ClockingStatuses.Created).ToList();
                clockings.Each(clocking =>
                {
                    clocking.Delete();
                });

                // Audit log
                await _timeSheetIntegrationEventService.AddEventAsync(new RejectMultipleClockingIntegrationEvent(timeSheet, clockings));

                _clockingWriteOnlyRepository.BatchDelete(clockings);
            }

            _timeSheetWriteOnlyRepository.Update(timeSheet);

            await _timeSheetWriteOnlyRepository.UnitOfWork.CommitAsync();
            return new Unit();
        }

        private void NotifyTimeSheetInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(TimeSheet).Name, Message.timeSheet_notExist));
        }

    }
}
