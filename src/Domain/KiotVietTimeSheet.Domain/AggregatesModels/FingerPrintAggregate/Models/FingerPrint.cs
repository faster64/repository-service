using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models
{
    public class FingerPrint : BaseEntity,
        ITenantId,
        IEntityIdlong,
        IBranchId,
        ICreatedDate,
        ICreatedBy,
        IModifiedDate,
        IModifiedBy
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public string FingerCode { get; set; }
        public long EmployeeId { get; set; }
        public string FingerName { get; set; }
        public int FingerNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }

        public FingerPrint()
        {

        }

        public FingerPrint(string fingerCode, long employeeId, string fingerName, int fingerNo)
        {
            FingerCode = fingerCode;
            EmployeeId = employeeId;
            FingerName = fingerName;
            FingerNo = fingerNo;
        }

        public void Update(string fingerCode, long employeeId, string fingerName, int fingerNo)
        {
            FingerCode = fingerCode;
            EmployeeId = employeeId;
            FingerName = fingerName;
            FingerNo = fingerNo;
        }

        public void UpdatedEmployee(long employeeId)
        {
            EmployeeId = employeeId;
        }

        public void UpdatedFingerCode(string fingerCode)
        {
            FingerCode = fingerCode;
        }
    }
}
