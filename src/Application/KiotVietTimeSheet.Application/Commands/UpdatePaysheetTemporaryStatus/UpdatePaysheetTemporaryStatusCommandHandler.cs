using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Application.Commands.UpdatePaysheetTemporaryStatus
{
    public class UpdatePaysheetTemporaryStatusCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdatePaysheetTemporaryStatusCommand, PaysheetDto>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdatePaysheetTemporaryStatusCommandHandler> _logger;
        private readonly IPaysheetWriteOnlyRepository _paySheetWriteOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paySheetReadOnlyRepository;

        public UpdatePaysheetTemporaryStatusCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IPaysheetWriteOnlyRepository paySheetWriteOnlyRepository,
            IPaysheetReadOnlyRepository paySheetReadOnlyRepository,
            ILogger<UpdatePaysheetTemporaryStatusCommandHandler> logger
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _logger = logger;
            _paySheetWriteOnlyRepository = paySheetWriteOnlyRepository;
            _paySheetReadOnlyRepository = paySheetReadOnlyRepository;
        }

        public async Task<PaysheetDto> Handle(UpdatePaysheetTemporaryStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var paySheet = await _paySheetReadOnlyRepository.FindByIdAsync(request.PaysheetId);
                paySheet.PaysheetStatus = (byte)PaysheetStatuses.TemporarySalary;
                if (request.PaysheetOldDto != null)
                {
                    paySheet.Name = request.PaysheetOldDto.Name;
                    paySheet.TimeOfStandardWorkingDay = request.PaysheetOldDto.TimeOfStandardWorkingDay;
                    paySheet.WorkingDayNumber = request.PaysheetOldDto.WorkingDayNumber;
                    paySheet.UpdatePeriodName(request.PaysheetOldDto.PaysheetPeriodName);
                    paySheet.UpdateStartAndEndTime(request.PaysheetOldDto.StartTime, request.PaysheetOldDto.EndTime);
                }

                paySheet.ErrorStatus = request.PaySheetError;
                paySheet.PaysheetCreatedDate = request.PaySheetCreateTimeOld;
                _paySheetWriteOnlyRepository.Update(paySheet);
                await _paySheetWriteOnlyRepository.UnitOfWork.CommitAsync();
                return _mapper.Map<PaysheetDto>(paySheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
