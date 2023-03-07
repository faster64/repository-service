using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;

namespace KiotVietTimeSheet.Application.DomainService
{
    public class RejectClockingsDomainService : IRejectClockingsDomainService
    {
        #region PROPERTIES
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;


        #endregion

        #region CONSTRUCTORS

        public RejectClockingsDomainService(
            IEventDispatcher eventDispatcher,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository
        )
        {
            _eventDispatcher = eventDispatcher;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
        }

        #endregion

        /// <inheritdoc />
        public async Task<bool> RejectClockingsAsync(List<Clocking> clockings)
        {
            if (clockings.Count > Constant.MaximumClockingCanUpdate)
            {
                NotifyExceedMaximumClocking();
                return false;
            }
            var timeSheetIds = clockings.Select(c => c.TimeSheetId).Distinct().ToList();
            var timeSheets = await _timeSheetWriteOnlyRepository.GetBySpecificationAsync(new FindTimeSheetByTimeSheetIdsSpec(timeSheetIds));
            var timeSheetClockings = await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByTimeSheetIdsSpec(timeSheetIds));
            var lsRejectClockings = new List<Clocking>();

            #region process domain

            timeSheets.ForEach(timeSheet =>
            {
                var listClockingOfTimeSheet = clockings.Where(c => c.TimeSheetId == timeSheet.Id).ToList();
                listClockingOfTimeSheet.ForEach(c =>
                {
                    var existClocking = timeSheetClockings.FirstOrDefault(clocking => clocking.Id == c.Id);
                    if (existClocking == null) return;
                    existClocking.Reject();
                    lsRejectClockings.Add(existClocking);
                });

                // hủy lịch làm việc nếu không còn clockings nào
                var listRejectedClockingIds = lsRejectClockings.Select(x => x.Id).ToList();
                if (timeSheetClockings.Any(c =>
                    c.ClockingStatus != (byte)ClockingStatuses.Void && !c.IsDeleted &&
                    c.TimeSheetId == timeSheet.Id && !listRejectedClockingIds.Contains(c.Id))) return;
                timeSheet.UpdateTimeSheetStatus((byte)TimeSheetStatuses.Void);
            });

            #endregion

            #region save change
            _clockingWriteOnlyRepository.BatchUpdate(lsRejectClockings);
            _timeSheetWriteOnlyRepository.BatchUpdate(timeSheets);
            #endregion

            return true;
        }

        private void NotifyExceedMaximumClocking()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Clocking).Name, string.Format(Message.clocking_maximumClockingUpdate, Constant.MaximumClockingCanUpdate)));
        }
    }
}
