namespace KiotViet.FileUpload.Providers.AmazoneS3
{
    public class AmazoneS3FileUploadConfiguration : KiotVietFileUploadConfiguration
    {
        public string AwsAccessKeyId { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string AwsCacheControlHeader { get; set; }
        public string AwsBucketName { get; set; }
        public string AwsCloudFrontUrl { get; set; }
        public new int MaxAvatarSize { get; set; }
        public new int MaxAvatarWidth { get; set; }
        public new int MaxAvatarHeight { get; set; }
    }
}
