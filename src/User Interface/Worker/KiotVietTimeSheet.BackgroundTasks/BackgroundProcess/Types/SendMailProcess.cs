using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.BackgroundTasks.Configuration;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using KiotVietTimeSheet.Utilities;
using ServiceStack;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class SendMailProcess : BaseBackgroundProcess
    {

        private readonly WorkerConfiguration _workerConfiguration;

        public SendMailProcess(
            IKiotVietInternalService kiotVietInternalService,
            IAuthService authService,
            WorkerConfiguration workerConfiguration): base(kiotVietInternalService,
            authService)
        {
            _workerConfiguration = workerConfiguration;
        }

        public async Task SendMailIntegration(SentMailIntegrationEvent @event)
        {
            var emailObject = @event.EmailEvent;
            emailObject.SmtpHost = _workerConfiguration.ConfigMail.KiotMailServerList;
            emailObject.SendTo = _workerConfiguration.ConfigMail.SendToWhenActiveTimeSheetEmails != null ? _workerConfiguration.ConfigMail.SendToWhenActiveTimeSheetEmails.Join(",") : "";
            emailObject.SenderEmail = _workerConfiguration.ConfigMail.SendFromWhenActiveTimeSheetEmail;
            emailObject.Port = _workerConfiguration.ConfigMail.KiotMailPort;
            emailObject.UseSsl = _workerConfiguration.ConfigMail.KiotMailUseSsl;
            emailObject.PasswordCertify = _workerConfiguration.ConfigMail.KiotMailPasswordCertify;
            emailObject.UserCertify = _workerConfiguration.ConfigMail.KiotMailUsernameCertify;
            await EmailHelper.SendMailToServerAsync(emailObject.SmtpHost, emailObject.Port, emailObject.UseSsl,
                emailObject.UserCertify, emailObject.PasswordCertify, emailObject.SenderEmail,
                emailObject.SendTo, "", "", "", "", emailObject.Subject, emailObject.Body);
        }

    }
}
