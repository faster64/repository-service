using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;

namespace KiotVietTimeSheet.SharedKernel.Models
{
    public class ImportExportFile : IRetailerId, ICreatedBy, ICreatedDate
    {
        public long Id { get; set; }
        public int RetailerId { get; set; }
        public string KvSessionId { get; set; }
        public bool? IsImport { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string EventType { get; set; }
        public byte Status { get; set; }
        public string Message { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte[] Revision { get; set; }
    }
}
