using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptoUtilities.Models
{
    /// <summary>
    /// AES暗号クラス
    /// </summary>
    internal class CryptAes
    {
        /// <summary>
        /// 暗号化処理
        /// </summary>
        /// <param name="plainText">プレーンテキスト</param>
        /// <param name="key">キー</param>
        /// <param name="initializationVector">初期化ベクター</param>
        /// <param name="mode">モード</param>
        /// <returns>暗号化結果</returns>
        public static byte[] Encrypt(byte[] plainText, byte[] key, byte[] initializationVector, CipherMode mode)
        {
            byte[] encrypted = null;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = key.Length * 8;
                aes.Key = key;
                if (mode == CipherMode.CBC)
                {
                    aes.IV = initializationVector;
                }
                aes.Mode = mode;

                using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (BinaryWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                encrypted = msEncrypt.ToArray();
            }

            return encrypted;
        }

        /// <summary>
        /// 復号化処理
        /// </summary>
        /// <param name="cipherText">暗号文</param>
        /// <param name="key">キー</param>
        /// <param name="initializationVector">初期化ベクター</param>
        /// <param name="mode">モード</param>
        /// <returns>復号化結果</returns>
        public static byte[] Decrypt(byte[] cipherText, byte[] key, byte[] initializationVector, CipherMode mode)
        {
            string decrypted = null;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = key.Length * 8;
                aes.Key = key;
                if (mode == CipherMode.CBC)
                {
                    aes.IV = initializationVector;
                }
                aes.Mode = mode;

                using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using MemoryStream msDecrypt = new(cipherText);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);

                decrypted = srDecrypt.ReadToEnd();
            }

            return Encoding.ASCII.GetBytes(decrypted);
        }
    }
}
