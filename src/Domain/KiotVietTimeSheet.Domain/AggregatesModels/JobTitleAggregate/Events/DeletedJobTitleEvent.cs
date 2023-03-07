using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Events
{
    public class DeletedJobTitleEvent : DomainEvent
    {
        public JobTitle JobTitle { get; set; }

        public DeletedJobTitleEvent(JobTitle jobTitle)
        {
            JobTitle = jobTitle;
        }
    }
}
