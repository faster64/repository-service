namespace KiotVietTimeSheet.Application.Dto
{
    public class InternalResponseDto
    {
        public InternalResponseStatusDto ResponseStatus { get; set; }
    }

    public class InternalResponseStatusDto
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}