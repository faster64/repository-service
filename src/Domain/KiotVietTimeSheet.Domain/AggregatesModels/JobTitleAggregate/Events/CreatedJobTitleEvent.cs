using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Events
{
    public class CreatedJobTitleEvent : DomainEvent
    {
        public JobTitle JobTitle { get; set; }

        public CreatedJobTitleEvent(JobTitle jobTitle)
        {
            JobTitle = jobTitle;
        }
    }
}
