using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models
{
    public class FingerMachine : BaseEntity,
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
        public string MachineName { get; set; }
        public string MachineId { get; set; }
        public string Vendor { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public string Note { get; set; }
        public string IpAddress { get; set; }
        public int? Commpass { get; set; }
        public int? Port { get; set; }
        public int? ConnectionType { get; set; }

        public FingerMachine()
        {

        }

        public FingerMachine(int branchId, string machineName, string vendor, string machineId, string note, int status)
        {
            BranchId = branchId;
            MachineName = machineName;
            Vendor = vendor;
            MachineId = machineId;
            Note = note;
            Status = status;
        }

        public FingerMachine(
            int branchId,
            string machineName,
            string vendor,
            string machineId,
            string note,
            int status,
            string ipAddress,
            int? commpass,
            int? port,
            int? connectionType)
        {
            BranchId = branchId;
            MachineName = machineName;
            Vendor = vendor;
            MachineId = machineId;
            Note = note;
            Status = status;
            IpAddress = ipAddress;
            Commpass = commpass;
            Port = port;
            ConnectionType = connectionType;
        }

        public void Update(string machineName, string machineId, string ipAddress, int? port, int? commpass, string note, int? connectionType)
        {
            MachineName = machineName;
            MachineId = machineId;
            Note = note;
            IpAddress = ipAddress;
            Port = port;
            Commpass = commpass;
            ConnectionType = connectionType;
        }
    }
}
