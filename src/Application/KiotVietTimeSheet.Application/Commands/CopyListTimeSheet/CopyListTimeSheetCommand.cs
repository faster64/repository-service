using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CopyListTimeSheet
{
    [RequiredPermission(TimeSheetPermission.Clocking_Copy)]
    public class CopyListTimeSheetCommand : BaseCommand<List<TimeSheetDto>>
    {
        public int BranchId { get; set; }

        public DateTime CopyFrom { get; set; }

        public DateTime CopyTo { get; set; }

        public DateTime PasteFrom { get; set; }

        public DateTime PasteTo { get; set; }

        public CopyListTimeSheetCommand(int branchId, DateTime copyFrom, DateTime copyTo, DateTime pasteFrom, DateTime pasteTo)
        {
            BranchId = branchId;
            CopyFrom = copyFrom;
            CopyTo = copyTo;
            PasteFrom = pasteFrom;
            PasteTo = pasteTo;
        }
    }
}
