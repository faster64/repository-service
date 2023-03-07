using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateFingerPrintLog
{
    public class CreateFingerPrintLogCommand : BaseCommand<List<AutoTimeKeepingResult>>
    {
        public List<FingerPrintLogDto> FingerPrintLogs { get; set; }

        public CreateFingerPrintLogCommand(List<FingerPrintLogDto> fingerPrintLogs)
        {
            FingerPrintLogs = fingerPrintLogs;
        }
    }
}
