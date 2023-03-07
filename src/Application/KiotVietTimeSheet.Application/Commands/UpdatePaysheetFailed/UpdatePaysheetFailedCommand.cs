using KiotVietTimeSheet.Application.Abstractions;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdatePaysheetFailed
{
    public class UpdatePaysheetFailedCommand : BaseCommand<Unit>
    {
        public long PaysheetId { get; set; }

        public UpdatePaysheetFailedCommand(long paysheetId)
        {
            PaysheetId = paysheetId;
        }
    }
}
