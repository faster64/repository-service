using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateFingerPrintLog
{
    public class CreateFingerPrintLogCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateFingerPrintLogCommand, List<AutoTimeKeepingResult>>
    {
        private readonly IAutoTimeKeepingDomainService _autoTimeKeepingDomainService;

        public CreateFingerPrintLogCommandHandler(
            IEventDispatcher eventDispatcher,
            IAutoTimeKeepingDomainService autoTimeKeepingDomainService
        )
            : base(eventDispatcher)
        {
            _autoTimeKeepingDomainService = autoTimeKeepingDomainService;
        }

        public async Task<List<AutoTimeKeepingResult>> Handle(CreateFingerPrintLogCommand request, CancellationToken cancellationToken)
        {
            var autoTimeKeepingResults = await _autoTimeKeepingDomainService.AutoTimeKeepingAsync(request.FingerPrintLogs);
            return autoTimeKeepingResults;
        }
    }
}
