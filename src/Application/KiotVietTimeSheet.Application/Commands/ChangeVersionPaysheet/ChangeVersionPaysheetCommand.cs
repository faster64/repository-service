using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.ChangeVersionPaysheet
{
    public class ChangeVersionPaysheetCommand : BaseCommand<Unit>
    {
        public List<long> Ids { get; set; }

        public ChangeVersionPaysheetCommand(List<long> ids)
        {
            Ids = ids;
        }
    }
}
