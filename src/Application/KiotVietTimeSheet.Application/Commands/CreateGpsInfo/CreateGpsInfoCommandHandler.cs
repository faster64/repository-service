using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.Validators.GpsInfoValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.Utilities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateGpsInfo
{
    public class CreateGpsInfoCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateGpsInfoCommand, GpsInfoDto>
    {
        private readonly IMapper _mapper;
        private readonly IGpsInfoWriteOnlyRepository _gpsInfoWriteOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly GpsInfoCreateOrUpdateValidator _gpsInfoCreateOrUpdateValidator;
        public CreateGpsInfoCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IGpsInfoWriteOnlyRepository gpsInfoWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            GpsInfoCreateOrUpdateValidator gpsInfoCreateOrUpdateValidator,
            IKiotVietServiceClient kiotVietServiceClient
            )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _gpsInfoWriteOnlyRepository = gpsInfoWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _gpsInfoCreateOrUpdateValidator = gpsInfoCreateOrUpdateValidator;
            _kiotVietServiceClient = kiotVietServiceClient; 

        }
        public async Task<GpsInfoDto> Handle(CreateGpsInfoCommand request, CancellationToken cancellationToken)
        {
            var gpsInfor = _mapper.Map<GpsInfo>(request.GpsInfoDto);
            var validator = await _gpsInfoCreateOrUpdateValidator.ValidateAsync(gpsInfor);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(GpsInfo), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return request.GpsInfoDto;
            }
            var checkExit = await _gpsInfoWriteOnlyRepository.AnyBySpecificationAsync(new FindGpsInfoByBranchIdSpec(gpsInfor.BranchId));
            if (checkExit) {
                var branch = await _kiotVietServiceClient.GetBranchById(gpsInfor.BranchId);
                NotifyValidationErrors(typeof(GpsInfo),new List<string> { string.Format(Message.timeSheet_gpsInfoHasExits, branch.Name) });
            }
            gpsInfor.Status = (byte)GpsInfoStatus.Active;
            gpsInfor.QrKey = await _gpsInfoWriteOnlyRepository.GetNewQrKey();
            _gpsInfoWriteOnlyRepository.Add(gpsInfor);
            var result = _mapper.Map<GpsInfoDto>(gpsInfor);
            await _timeSheetIntegrationEventService.AddEventAsync(new CreatedGpsInfoIntegrationEvent(gpsInfor));
            await _gpsInfoWriteOnlyRepository.UnitOfWork.CommitAsync();
            return result;
        }
        
        
    }
}
