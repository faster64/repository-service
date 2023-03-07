using System.IO;
using System.Threading.Tasks;

namespace KiotViet.FileUpload
{
    public interface IKiotVietFileUpload
    {
        Task<KiotVietUploadResult> UploadAvatarAsync(string fileName, MemoryStream stream, string contentType);
    }
}
