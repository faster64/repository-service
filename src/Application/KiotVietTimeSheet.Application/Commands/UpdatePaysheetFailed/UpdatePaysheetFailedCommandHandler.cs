using System;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Application.Commands.UpdatePaysheetFailed
{
    public class UpdatePaysheetFailedCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdatePaysheetFailedCommand, Unit>
    {
        private readonly ILogger<UpdatePaysheetFailedCommandHandler> _logger;
        private readonly IPaysheetWriteOnlyRepository _paySheetWriteOnlyRepository;
        private readonly IPaysheetReadOnlyRepository _paySheetReadOnlyRepository;

        public UpdatePaysheetFailedCommandHandler(
            IEventDispatcher eventDispatcher,
            IPaysheetWriteOnlyRepository paySheetWriteOnlyRepository,
            IPaysheetReadOnlyRepository paySheetReadOnlyRepository,
            ILogger<UpdatePaysheetFailedCommandHandler> logger
        ) : base(eventDispatcher)
        {
            _logger = logger;
            _paySheetWriteOnlyRepository = paySheetWriteOnlyRepository;
            _paySheetReadOnlyRepository = paySheetReadOnlyRepository;
        }

        public async Task<Unit> Handle(UpdatePaysheetFailedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var paySheet = await _paySheetReadOnlyRepository.FindByIdAsync(request.PaysheetId);
                if(paySheet == null)
                    _logger.LogInformation("UpdatePaysheetFailedCommand : PaySheet is null");
                else
                {
                    paySheet.PaysheetStatus = (byte) PaysheetStatuses.Void;
                    _paySheetWriteOnlyRepository.Update(paySheet);
                    await _paySheetWriteOnlyRepository.UnitOfWork.CommitAsync();
                }
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
