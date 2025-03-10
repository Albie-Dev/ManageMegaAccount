using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MMA.Service
{
    partial class LinkHasher
    {
        #region Mã hóa và giải mã AES (mới thêm vào)
        private static readonly string encryptionKey = "HqPHeH7U8Xjl6gK0b0kTgJw6vde5VqI60QdRtde1L0M="; // 256-bit key (Base64)
        private static readonly string encryptionIV = "OGV7au1A06klHJ7g3FYR4Q=="; // 128-bit IV (Base64)

        // Mã hóa chuỗi bằng AES
        public static string EncryptToken(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Sử dụng Convert.FromBase64String() để giải mã từ Base64
                aesAlg.Key = Convert.FromBase64String(encryptionKey);  // 32 bytes cho AES-256
                aesAlg.IV = Convert.FromBase64String(encryptionIV);    // 16 bytes cho AES-128

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());  // Trả về Base64 mã hóa
                }
            }
        }

        // Giải mã chuỗi bằng AES
        public static string DecryptToken(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Sử dụng Convert.FromBase64String() để giải mã từ Base64
                aesAlg.Key = Convert.FromBase64String(encryptionKey);  // 32 bytes cho AES-256
                aesAlg.IV = Convert.FromBase64String(encryptionIV);    // 16 bytes cho AES-128

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();  // Trả về chuỗi giải mã
                        }
                    }
                }
            }
        }
        #endregion
    }
}
