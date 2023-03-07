using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications;
using KiotVietTimeSheet.Domain.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.DomainService
{
    public class WorkingDayForPaysheetDomainService : IWorkingDayForPaysheetDomainService
    {
        private readonly IBranchSettingReadOnlyRepository _branchSettingReadOnlyRepository;

        public WorkingDayForPaysheetDomainService(
            IBranchSettingReadOnlyRepository branchSettingReadOnlyRepository
            )
        {
            _branchSettingReadOnlyRepository = branchSettingReadOnlyRepository;
        }
        public async Task<int> GetWorkingDayPaysheetAsync(int branchId, DateTime from, DateTime to)
        {
            from = from.Date;
            var branchSetting = await _branchSettingReadOnlyRepository.FindBySpecificationAsync(new FindBranchSettingByBranchIdSpec(branchId));
            var dateTimeRange = new DateTimeEnumerator(from, to);

            var workingDays = 0;
            foreach (DateTime day in dateTimeRange)
            {
                var isWorkingDay = branchSetting == null || branchSetting.WorkingDaysInArray.Any(x => x == (byte)day.DayOfWeek);
                if (isWorkingDay) workingDays++;
            }
            return workingDays;
        }
    }
}
