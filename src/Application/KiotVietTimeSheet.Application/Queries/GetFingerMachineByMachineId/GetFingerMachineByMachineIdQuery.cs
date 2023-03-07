using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetFingerMachineByMachineId
{
    public class GetFingerMachineByMachineIdQuery : QueryBase<FingerMachineDto>
    {
        public string MachineId { get; set; }

        public GetFingerMachineByMachineIdQuery(string machineId)
        {
            MachineId = machineId;
        }
    }
}
