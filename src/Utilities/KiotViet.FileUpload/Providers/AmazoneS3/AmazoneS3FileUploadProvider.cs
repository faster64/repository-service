using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KiotViet.FileUpload.Providers.AmazoneS3
{
    public class AmazoneS3FileUploadProvider : KiotVietFileUpload
    {
        private const int DIV_TO_MB = 1048576;
        private readonly AmazonS3Client _client;
        private readonly AmazoneS3FileUploadConfiguration _configuration;

        public AmazoneS3FileUploadProvider(AmazoneS3FileUploadConfiguration configuration)
        {
            _configuration = configuration;
            _client = new AmazonS3Client(
                _configuration.AwsAccessKeyId, 
                _configuration.AwsSecretAccessKey,
                new AmazonS3Config
                {
                    SignatureVersion = "2",
                    RegionEndpoint = RegionEndpoint.APSoutheast1
                }
            );
        }

        public override async Task<KiotVietUploadResult> UploadAvatarAsync(string fileName, MemoryStream stream, string contentType)
        {
            var result = new KiotVietUploadResult();

            if (stream.Length > _configuration.MaxAvatarSize)
            {
                result.Status = UploadResultStatuses.Error;
                result.Error = $"Dung lượng file không được lớn quá {_configuration.MaxAvatarSize / DIV_TO_MB} MB";

                return result;
            }

            if (await UploadFileAsync(_configuration.AwsBucketName, fileName, stream, contentType))
            {
                result.Status = UploadResultStatuses.Success;
                result.Url = _configuration.AwsCloudFrontUrl + fileName;
            }
            else
            {
                result.Status = UploadResultStatuses.Error;
                result.Error = @"Upload không thành công";
            }

            return result;
        }

        private async Task<bool> UploadFileAsync(string bucketName, string key, Stream stream, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                Headers = { CacheControl = _configuration.AwsCacheControlHeader }
            };

            try
            {
                await _client.PutObjectAsync(request);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
      
        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void DisposeAmazon(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                // TODOs: dispose managed state (managed objects).
                _client?.Dispose();
            }

            // TODOs: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODOs: set large fields to null.

            _disposedValue = true;
        }

        // TODOs: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // dispose OrmLiteRepository
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //  Call Dispose equal false
        // 

        // This code added to correctly implement the disposable pattern.
        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            DisposeAmazon(true);
            // TODOs: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
