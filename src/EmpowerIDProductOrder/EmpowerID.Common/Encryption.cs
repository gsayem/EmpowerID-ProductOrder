using System.Security.Cryptography;
using System.Text;

namespace EmpowerID.Common
{
    public static class Encryption
    {
        private static byte[] TransformPBKDF2(byte[] data, bool isEncrypting, string salt)
        {
            var rfc = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(salt), new byte[8], 1000, HashAlgorithmName.SHA256);
            using var aes = Aes.Create();
            aes.Key = rfc.GetBytes(32);
            aes.IV = rfc.GetBytes(16);
            var cryptoTransform = isEncrypting ? aes.CreateEncryptor() : aes.CreateDecryptor();
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
            cryptoStream.Write(data);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }
        public static string ToEncrypt(this string plain, string salt)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plain);
            var transformedBytes = TransformPBKDF2(plainBytes, true, salt);
            return BitConverter.ToString(transformedBytes).Replace("-", "").ToUpper();
        }

        public static string ToDecrypt(this string cipher, string salt)
        {
            var cipherBytes = Convert.FromHexString(cipher);
            var transformedBytes = TransformPBKDF2(cipherBytes, false, salt);
            return Encoding.UTF8.GetString(transformedBytes);
        }
    }
}
