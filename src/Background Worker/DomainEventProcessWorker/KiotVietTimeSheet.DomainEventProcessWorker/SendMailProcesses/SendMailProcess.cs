using System;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.SendMailEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.Configuration;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.Utilities;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.DomainEventProcessWorker.SendMailProcesses
{
    public class SendMailProcess
    {
        private readonly IWorkerConfiguration _workerConfiguration;

        private ILogger<SendMailProcess> _logger;
        private ILogger<SendMailProcess> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<SendMailProcess>>());

        public SendMailProcess(EfDbContext db, IKiotVietApiClient kiotVietApiClient, IWorkerConfiguration workerConfiguration)
        {
            _workerConfiguration = workerConfiguration;

        }

        public async Task SendMailAsync(SentMailIntegrationEvent @event)
        {
            try
            {
                if (@event?.EmailEvent != null)
                {

                    var emailObject = @event.EmailEvent;
                    emailObject.SmtpHost = _workerConfiguration.KiotMailServerList.FirstOrDefault();
                    emailObject.SendTo = _workerConfiguration.SendToWhenActiveTimeSheetEmails != null ? _workerConfiguration.SendToWhenActiveTimeSheetEmails.Join(",") : "";
                    emailObject.SenderEmail = _workerConfiguration.SendFromWhenActiveTimeSheetEmail;
                    emailObject.Port = _workerConfiguration.KiotMailPort;
                    emailObject.UseSsl = _workerConfiguration.KiotMailUseSsl;
                    emailObject.PasswordCertify = _workerConfiguration.KiotMailPasswordCertify;
                    emailObject.UserCertify = _workerConfiguration.KiotMailUsernameCertify;
                    await EmailHelper.SendMailToServerAsync(emailObject.SmtpHost, emailObject.Port, emailObject.UseSsl,
                        emailObject.UserCertify, emailObject.PasswordCertify, emailObject.SenderEmail,
                        emailObject.SendTo, "", "", "", "", emailObject.Subject, emailObject.Body);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }


        #region Private Methods


        #endregion
    }
}
