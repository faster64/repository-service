namespace KiotVietTimeSheet.Application.Dto
{
    public class AutoTimeKeepingResult
    {
        public FingerPrintLogDto FingerPrintLog { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public bool IsExist { get; set; } = false;
    }
}
