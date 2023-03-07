using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;

namespace KiotVietTimeSheet.Application.Parameters.Interfaces
{
    public interface IUpdateConfirmClockingParamBuilder
    {
        IConfirmClockingHistoryWriteOnlyRepository GetConfirmClockingHistoryWriteOnlyRepository();
        IConfirmClockingWriteOnlyRepository GetConfirmClockingWriteOnlyRepository();
        ICalculateTimeClockingDomainService GetCalculateTimeClockingDomainService();
        IAutoTimeKeepingDomainService GetAutoTimeKeepingDomainService();
        IConfirmClockingDomainService GetConfirmClockingDomainService();
        IEmployeeWriteOnlyRepository GetEmployeeWriteOnlyRepository();
        IClockingHistoryWriteOnlyRepository GetClockingHistoryWriteOnlyRepository();
    }
}
