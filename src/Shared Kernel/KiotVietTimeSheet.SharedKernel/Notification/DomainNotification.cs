using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.SharedKernel.Notification
{
    public class DomainNotification : Message
    {
        public ErrorResult ErrorResult { get; private set; }
        public DomainNotification(string key, string value)
        {
            ErrorResult = new ErrorResult()
            {
                Message = value
            };

        }
        public DomainNotification(string key, ErrorResult value)
        {
            ErrorResult = value;


        }
    }
}
