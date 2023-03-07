using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.GpsInfoValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.UpdateGpsInfo
{
    public class UpdateGpsInfoCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateGpsInfoCommand, GpsInfoDto>
    {        
        private readonly IGpsInfoWriteOnlyRepository _gpsInfoWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IMapper _mapper;
        private readonly GpsInfoCreateOrUpdateValidator _gpsInfoCreateOrUpdateValidator;
        public UpdateGpsInfoCommandHandler(
            IEventDispatcher eventDispatcher,
            IGpsInfoWriteOnlyRepository gpsInfoWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IMapper mapper,
            GpsInfoCreateOrUpdateValidator gpsInfoCreateOrUpdateValidator)
            : base(eventDispatcher)
        {
            _gpsInfoWriteOnlyRepository = gpsInfoWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _mapper = mapper;
            _gpsInfoCreateOrUpdateValidator = gpsInfoCreateOrUpdateValidator;
        }
        public async Task<GpsInfoDto> Handle(UpdateGpsInfoCommand request, CancellationToken cancellationToken)
        {
            var gpsInfoUpdate = request.GpsInfo;
            var viewModel = _mapper.Map<GpsInfo>(gpsInfoUpdate);
            var validator = await _gpsInfoCreateOrUpdateValidator.ValidateAsync(viewModel);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(GpsInfo), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return request.GpsInfo;
            }
            var gpsInforCur = await _gpsInfoWriteOnlyRepository.FindByIdAsync(gpsInfoUpdate.Id);
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedGpsInfoIntegrationEvent(viewModel, gpsInforCur));
            gpsInforCur.Update(gpsInfoUpdate.Coordinate, gpsInfoUpdate.Address, gpsInfoUpdate.LocationName, gpsInfoUpdate.WardName, gpsInfoUpdate.Province, gpsInfoUpdate.District,gpsInfoUpdate.RadiusLimit);
            _gpsInfoWriteOnlyRepository.Update(gpsInforCur);

            await _gpsInfoWriteOnlyRepository.UnitOfWork.CommitAsync();
            var result = _mapper.Map<GpsInfoDto>(gpsInforCur);
            return result;

        }
    }
}
