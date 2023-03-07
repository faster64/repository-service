using KiotVietTimeSheet.Application.Abstractions;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.SendEmailActiveTrial
{
    public class SendEmailActiveTrialCommand : BaseCommand<Unit>
    {
        public string SendTo { get; set; }
        public string BccEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public SendEmailActiveTrialCommand(string sendTo, string bccEmail, string subject, string body)
        {
            SendTo = sendTo;
            BccEmail = bccEmail;
            Subject = subject;
            Body = body;
        }
    }
}
