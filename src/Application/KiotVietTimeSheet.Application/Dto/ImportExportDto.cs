using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KiotVietTimeSheet.SharedKernel.Auth;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ImportExportFileDto
    {
        [DataMember(Name = "Id")]
        public long Id { get; set; }

        [DataMember(Name = "RetailerId")]
        public int RetailerId { get; set; }

        [DataMember(Name = "KvSessionId")]
        public string KvSessionId { get; set; }

        [DataMember(Name = "IsImport")]
        public bool? IsImport { get; set; }

        [DataMember(Name = "FileName")]
        public string FileName { get; set; }

        [DataMember(Name = "FilePath")]
        public string FilePath { get; set; }

        [DataMember(Name = "EventType")]
        public string EventType { get; set; }

        [DataMember(Name = "Status")]
        public byte Status { get; set; }

        [DataMember(Name = "Message")]
        public string Message { get; set; }

        [DataMember(Name = "CreatedBy")]
        public long CreatedBy { get; set; }

        [DataMember(Name = "ModifiedBy")]
        public long? ModifiedBy { get; set; }

        [DataMember(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = "ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [DataMember(Name = "Revision")]
        public byte[] Revision { get; set; }
    }

    public class ImportExportExecutionContext
    {
        public string Id { get; set; }

        public SessionUser User { get; set; }

        public string IpSource { get; set; }

        public string ClientInfo { get; set; }

        public string Host { get; set; }

        public int BranchId { get; set; }

        public IDictionary<string, ISet<int>> Permissions { get; set; }

        public int[] AuthorizedBranchIds { get; set; }

        public int RetailerId { get; set; }

        public string RetailerCode { get; set; }

        public int IndustryId { get; set; }

        public int GroupId { get; set; }

        public ImportExportKvGroup Group { get; set; }

        public string BearerToken { get; set; }
        public bool IsFnB()
        {
            return IndustryId == 15;
        }
    }

    public class ImportExportSession : AuthUserSession
    {
        public SessionUser CurrentUser { get; set; }

        public string KvSessionId { get; set; }

        public int CurrentRetailerId { get; set; }

        public int CurrentIndustryId { get; set; }

        public int GroupId { get; set; }

        public DateTime ExpireDate { get; set; }

        public string CurrentRetailerCode { get; set; }

        public string CurrentLang { get; set; }

        public int CurrentBranchId { get; set; }

        public IDictionary<string, IList<int>> PermissionMap { get; set; }

        public int[] PermittedBranchIds { get; set; }
    }

    public class ImportExportKvGroup
    {
        public int Id { get; set; }

        public string ConnectionString { get; set; }
    }
}
