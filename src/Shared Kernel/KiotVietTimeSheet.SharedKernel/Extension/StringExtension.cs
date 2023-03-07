namespace KiotVietTimeSheet.SharedKernel.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// Chuẩn hóa chuỗi
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPerfectString(this string str)
        {
            return (str ?? string.Empty).Normalize().Trim();
        }
    }
}
