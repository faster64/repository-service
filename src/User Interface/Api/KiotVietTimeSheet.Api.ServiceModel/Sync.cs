using System;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/employee-sync/ping")]
    public class Ping
    {
        
    }
    [Route("/employee-sync/list", "GET")]
    public class SyncEmployeeList : QueryDb<SyncEmployeeDto> ,IReturn<SyncEmployeeDto>
    {
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public DateTime? LastModifiedFrom { get; set; }
    }
    [Route("/employee-sync/create", "POST")]
    public class CreateSyncEmployee : IReturn<object>
    {
        public SyncEmployeeDto Employee { get; set; }
    }
    [Route("/employee-sync/delete/{Id}", "DELETE")]
    public class DeleteSyncEmployee : IReturn<object>
    {
        public long Id { get; set; }
    }

   }