namespace KiotVietTimeSheet.Api.ServiceModel.Types
{

    public class Response<T> where T : class
    {
        public T Result { get; set; }
        public string Message { get; set; }
        public object Errors { get; set; }
    }
}
