using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdatePaysheets
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Update)]
    public class UpdatePaysheetsCommand : BaseCommand<Unit>
    {
        public List<Paysheet> Paysheets { get; set; }
        public UpdatePaysheetsCommand(List<Paysheet> paysheets)
        {
            Paysheets = paysheets;
        }
    }
}
