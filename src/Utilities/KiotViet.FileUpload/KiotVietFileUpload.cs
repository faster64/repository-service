using System;
using System.IO;
using System.Threading.Tasks;

namespace KiotViet.FileUpload
{
    public abstract class KiotVietFileUpload : IKiotVietFileUpload, IDisposable
    {
        public KiotVietFileUpload() { }

        public abstract void Dispose();
        public abstract Task<KiotVietUploadResult> UploadAvatarAsync(string fileName, MemoryStream stream, string contentType);
    }
}
