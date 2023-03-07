using KiotVietTimeSheet.SharedKernel.Models;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PenalizeDto : BaseEntityByBaseInfo
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

        public decimal Value { get; set; }

        public List<ClockingPenalizeDto> ClockingPenalizesDto { get; set; }
    }
}
