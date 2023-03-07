using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace KiotViet.FileUpload.Helpers
{
    public static class ImageHelper
    {
        public static bool IsImage(Stream stream)
        {
            try
            {
                using (var image = Image.FromStream(stream))
                {
                    stream.Position = 0;

                    if (!image.RawFormat.Equals(ImageFormat.Jpeg) &&
                        !image.RawFormat.Equals(ImageFormat.Gif) &&
                        !image.RawFormat.Equals(ImageFormat.Png) &&
                        !image.RawFormat.Equals(ImageFormat.Bmp))
                        return false;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
