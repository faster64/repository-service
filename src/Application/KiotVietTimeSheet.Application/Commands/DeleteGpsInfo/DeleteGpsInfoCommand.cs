using KiotVietTimeSheet.Application.Abstractions;

namespace KiotVietTimeSheet.Application.Commands.DeleteGpsInfo
{
    public class DeleteGpsInfoCommand : BaseCommand
    {
        public long Id { get; set; }
        public DeleteGpsInfoCommand(long id)
        {
            Id = id;
        }
    }

}
