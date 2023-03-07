using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/finger-print-logs",
        "POST",
        Summary = "Thêm dữ liệu chấm công",
        Notes = "")
    ]
    public class CreateFingerPrintLogReq
    {
        public List<FingerPrintLogDto> FingerPrintLogs { get; set; }
    }
}
