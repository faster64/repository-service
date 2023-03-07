using KiotVietTimeSheet.SharedKernel.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models
{
    public class CronSchedule :
        IEntityIdlong,
        ICreatedDate,
        IModifiedDate,
        ITenantId
    {

        // For EF
        public CronSchedule(){ }

        [Key]
        public long Id { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public int TenantId { get; set; }
        public string TenantCode { get; set; }
        public int? BranchId { get; set; }
        public string Params { get; set; }
        public DateTime LastSync { get; set; }
        public DateTime NextRun { get; set; }
        public DateTime? LimitRun { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int Duration { get; set; }
        public bool IsRunning { get; set; }
        public int Processed { get; set; }
    }
}