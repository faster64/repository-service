using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Events
{
    public class UpdatedJobTitleEvent : DomainEvent
    {
        public JobTitle JobTitle { get; set; }

        public UpdatedJobTitleEvent(JobTitle jobTitle)
        {
            JobTitle = jobTitle;
        }
    }
}
