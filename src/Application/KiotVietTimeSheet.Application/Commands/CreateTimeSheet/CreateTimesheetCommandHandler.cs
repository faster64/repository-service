using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateTimesheet
{
    public class CreateTimesheetCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateTimesheetCommand, TimeSheetDto>
    {
        private readonly ICreateTimeSheetClockingDomainService _createTimeSheetClockingDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;

        public CreateTimesheetCommandHandler(
            IEventDispatcher eventDispatcher,
            ICreateTimeSheetClockingDomainService createTimeSheetClockingDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository
        )
            : base(eventDispatcher)
        {
            _createTimeSheetClockingDomainService = createTimeSheetClockingDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
        }

        public async Task<TimeSheetDto> Handle(CreateTimesheetCommand request, CancellationToken cancellationToken)
        {
            var timeSheetDto = request.TimeSheet;
            var result = await _createTimeSheetClockingDomainService.CreateAsync(timeSheetDto);
            if (!result.IsValid)
            {
                NotifyValidationErrors(typeof(TimeSheet), result.ValidationErrors);
                return null;
            }

            var clockings = result.TimeSheets.SelectMany(x => x.Clockings).ToList();
            await _timeSheetIntegrationEventService.AddEventAsync(new CreateMultipleClockingIntegrationEvent(timeSheetDto, clockings));
            await _timeSheetWriteOnlyRepository.UnitOfWork.CommitAsync();
            return result.TimeSheets.FirstOrDefault();
        }
    }
}
