using System.Security.Cryptography;
using System.Text;

namespace KiotVietTimeSheet.Utilities
{
    public static class EncryptHelpers
    {
        private const string PassPhrase = "nhQDhFofU534UHFf";
        private const string SaltValue = "c5elplhg";        // can be any string
        private const string HashAlgorithm = "SHA1";    // can be "MD5" if need
        private const int PasswordIterations = 2;       // can be any number
        private const string InitVector = "N75UJeuvnDfAarGt"; // must be 16 bytes
        private const int KeySize = 256;                // can be 192 or 128

        public static string RijndaelEncrypt(string plainText)
        {
            return RijndaelEncryption.Encrypt(plainText, PassPhrase, SaltValue, HashAlgorithm, PasswordIterations, InitVector, KeySize);
        }

        public static string RijndaelDecrypt(string cipherText)
        {
            return RijndaelEncryption.Decrypt(cipherText, PassPhrase, SaltValue, HashAlgorithm, PasswordIterations, InitVector, KeySize);
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            var sb = new StringBuilder();
            foreach (var b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
