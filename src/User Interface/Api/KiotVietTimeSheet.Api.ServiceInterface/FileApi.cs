using KiotViet.FileUpload;
using KiotVietTimeSheet.Api.ServiceModel;
using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class FileApi : BaseApi
    {
        private readonly IKiotVietFileUpload _kiotVietFileUpload;
        public FileApi(ILogger<FileApi> logger, IKiotVietFileUpload kiotVietFileUpload)
            : base(logger)
        {
            _kiotVietFileUpload = kiotVietFileUpload;
        }

        public async Task<object> Post(FileUploadReq req)
        {
            var results = new List<KiotVietUploadResult>();

            foreach (var uploadedFile in Request.Files.Where(uploadedFile => uploadedFile.ContentLength > 0))
            {
                using (var ms = new MemoryStream())
                {
                    uploadedFile.WriteTo(ms);
                    string key = string.Format("{0}/{1}", CurrentRetailerId, Guid.NewGuid().ToString("N").ToLower());
                    results.Add(await _kiotVietFileUpload.UploadAvatarAsync(key, ms, uploadedFile.ContentType));
                }
            }

            return Ok(results);
        }
    }
}
