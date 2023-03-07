using System.Runtime.Serialization;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/importexport/exportfile",
        "POST",
        Summary = "Import export",
        Notes = "")
    ]
    public class ImportExportReq : IReturn<object>
    {
        public string Type { get; set; }
        public string FileName { get; set; }
        public string Columns { get; set; }
        public string Filters { get; set; }
        public string Revision { get; set; }
    }

    [Route("/importcommission", "POST")]
    public class ImportCommissionReq : IReturn<object>
    {
    }

    [DataContract]
    public class ImportExportRes
    {
        [DataMember(Name = "Data")]
        public ImportExportFileDto Data { get; set; }
    }

    [DataContract]
    public class ImportCommissionRes
    {
        [DataMember(Name = "Status")]
        public string Status { get; set; }

        [DataMember(Name = "Data")]
        public ImportExportFileDto Data { get; set; }
    }
}
