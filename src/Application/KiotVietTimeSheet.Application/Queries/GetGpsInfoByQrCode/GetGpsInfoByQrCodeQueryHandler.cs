using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetListGpsInfo
{
    public class GetGpsInfoByQrCodeQueryHandler : QueryHandlerBase,
        IRequestHandler<GetGpsInfoByQrCodeQuery, GpsInfoDto>
    {
        private readonly IGpsInfoReadOnlyRepository _gpsInfoReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly ISettingsReadOnlyRepository _settingsRepository;
        private readonly IEventDispatcher _eventDispatcher;

        public GetGpsInfoByQrCodeQueryHandler(
            IAuthService authService,
            IGpsInfoReadOnlyRepository gpsInfoReadOnlyRepository,
            IMapper mapper,
            ISettingsReadOnlyRepository settingsRepository,
            IEventDispatcher eventDispatcher) : base(authService)
        {
            _gpsInfoReadOnlyRepository = gpsInfoReadOnlyRepository;
            _mapper = mapper;
            _settingsRepository = settingsRepository;
            _eventDispatcher = eventDispatcher;
    }

        public async Task<GpsInfoDto> Handle(GetGpsInfoByQrCodeQuery request, CancellationToken cancellationToken)
        {
            var isUseClockingGps = await _settingsRepository.IsUseClockingGps(request.Tenant.Id);
            if (!isUseClockingGps)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(nameof(GpsInfo), "Hiện tại cửa hàng đã ngừng chấm công trên thiết bị di động. Vui lòng thực hiện chấm công theo hình thức khác"));
                return null;
            }

            var gpsInfo = await _gpsInfoReadOnlyRepository.GetForClockingGps(request.Tenant, request.QrKey);
            if (gpsInfo == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(nameof(GpsInfo), "Không thể chấm công bằng mã này. Vui lòng quét mã QR mới để chấm công."));
                return null;
            }

            if (gpsInfo.IsDeleted)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(nameof(GpsInfo), "Chi nhánh đã ngừng chấm công trên thiết bị di động. Vui lòng thực hiện chấm công theo hình thức khác"));
                return null;
            }

            return _mapper.Map<GpsInfoDto>(gpsInfo);
        }
    }
}
