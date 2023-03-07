using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using MediatR;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class CalculateTimeClockingDomainService : ICalculateTimeClockingDomainService
    {
        private readonly IMediator _mediator;

        public CalculateTimeClockingDomainService(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }

        public async Task<int> GetTimeLate(int tenantId, DateTime clockingStartTime, DateTime? checkedInDate)
        {
            if (checkedInDate == null) return 0;

            var setting = await _mediator.Send(new GetSettingQuery(tenantId));

            // Nếu thiết lập tự động tính đi muộn sẽ tính theo số phút trong settings
            // nếu không thiết lập tự động tính đi muộn sẽ tính đi muộn theo giờ bắt đầu vào ca.
            if (setting.IsAutoCalcLateTime)
            {
                var timeTarget = clockingStartTime.AddMinutes(setting.LateTime);
                if  (timeTarget < checkedInDate.Value) return (int)Math.Round((checkedInDate.Value - timeTarget).TotalMinutes);
            }

            return 0;
        }

        public async Task<int> GetTimeEarly(int tenantId, DateTime clockingEndTime, DateTime? checkedOutDate)
        {
            if (checkedOutDate == null) return 0;

            var setting = await _mediator.Send(new GetSettingQuery(tenantId));

            // Nếu thiết lập tự động tính về sớm sẽ tính theo số phút trong settings
            // nếu không thiết lập tự động về sớm sẽ tính về sớm theo giờ kết thúc ca.
            if (setting.IsAutoCalcEarlyTime)
            {
                var timeTarget = clockingEndTime.AddMinutes(-setting.EarlyTime);
                if (timeTarget > checkedOutDate.Value) return (int)Math.Round((timeTarget - checkedOutDate.Value).TotalMinutes);
            }

            return 0;
        }

        public async Task<int> GetOverTimeBeforeShiftWork(int tenantId, DateTime clockingStartTime, DateTime? checkedInDate)
        {
            if (checkedInDate == null) return 0;

            var setting = await _mediator.Send(new GetSettingQuery(tenantId));

            if (setting.IsAutoCalcEarlyTimeOT)
            {
                var timeTarget = clockingStartTime.AddMinutes(-setting.EarlyTimeOT);
                if (timeTarget > checkedInDate.Value) return (int)Math.Round((timeTarget - checkedInDate.Value).TotalMinutes);
            }

            return 0;
        }

        public async Task<int> GetOverTimeAfterShiftWork(int tenantId, DateTime clockingEndTime, DateTime? checkedOutDate)
        {
            if (checkedOutDate == null) return 0;

            var setting = await _mediator.Send(new GetSettingQuery(tenantId));

            if (setting.IsAutoCalcLateTimeOT)
            {
                var timeTarget = clockingEndTime.AddMinutes(setting.LateTimeOT);
                if (timeTarget < checkedOutDate.Value) return (int)Math.Round((checkedOutDate.Value - timeTarget).TotalMinutes);
            }

            return 0;
        }
    }
}
