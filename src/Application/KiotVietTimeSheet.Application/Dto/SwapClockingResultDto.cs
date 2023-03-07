namespace KiotVietTimeSheet.Application.Dto
{
    public class SwapClockingResultDto
    {
        public SwapClockingResultDto(ClockingDto source, ClockingDto target)
        {
            SourceClocking = source;
            TargetClocking = target;
        }
        public ClockingDto SourceClocking { get; set; }
        public ClockingDto TargetClocking { get; set; }
    }
}
