using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateUpdateFingerPrint
{
    public class CreateUpdateFingerPrintCommand : BaseCommand<FingerPrintDto>
    {
        public FingerPrintDto FingerPrint { get; set; }

        public CreateUpdateFingerPrintCommand(FingerPrintDto fingerPrint)
        {
            FingerPrint = fingerPrint;
        }
    }
}
