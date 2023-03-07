using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Application.Commands.ImportExport;
using KiotVietTimeSheet.Infrastructure.Securities.KvAuth;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Host;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class ImportExportApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        #endregion

        #region Constructors
        public ImportExportApi(
            ILogger<EmployeeApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        public async Task<object> Post(ImportExportReq req)
        {
            var session = SessionAs<KVSession>();
            var execCommand = req.ConvertTo<ExportCommand>();
            execCommand.KvSessionId = session.KvSessionId;
            execCommand.KVSession = session;

            var result = await _mediator.Send(execCommand);

            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault()?.Message, Errors);
            }

            return new ImportExportRes { Data = result };
        }

        public async Task<object> Post(ImportCommissionReq req)
        {
            var session = SessionAs<KVSession>();
            var execCommand = req.ConvertTo<ImportCommissionCommand>();
            execCommand.Files = Request.Files;
            execCommand.BearerToken = Request.GetBearerToken();
            execCommand.KvSessionId = session.KvSessionId;
            execCommand.KVSession = session;

            var result = await _mediator.Send(execCommand);

            if (Errors.Any())
            {
                return Errors.FirstOrDefault()?.Message;
            }

            return new ImportCommissionRes { Status = ImportStatus.Success.ToString(), Data = result };
        }
    }
}
