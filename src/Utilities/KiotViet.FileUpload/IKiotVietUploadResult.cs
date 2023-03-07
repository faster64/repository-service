namespace KiotViet.FileUpload
{
    public class KiotVietUploadResult
    {
        public string Url { get; set; }
        public string Error { get; set; }
        public UploadResultStatuses Status { get; set; }
    }
}
