using System.IO;
using System.Text.Json;

namespace CryptoUtilities.Models
{
    /// <summary>
    /// 設定クラス
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// AES:CBCモードかどうか
        /// </summary>
        public bool IsAesModeCBC {  get; set; }

        /// <summary>
        /// AES:ECBモードかどうか
        /// </summary>
        public bool IsAesModeECB { get; set; }

        /// <summary>
        /// AES:キー
        /// </summary>
        public string AesKey {  get; set; }

        /// <summary>
        /// AES:初期化ベクター
        /// </summary>
        public string AesInitializationVector { get; set; }

        /// <summary>
        /// 設定ファイル名
        /// </summary>
        private string _configFileName = @"Configuration.json";

        /// <summary>
        /// 設定保存処理
        /// </summary>
        public void Save()
        {
            using FileStream fs = File.Create(_configFileName);
            using StreamWriter sw = new(fs, System.Text.Encoding.UTF8);
            JsonSerializerOptions options = new() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);
            sw.WriteLine(jsonString);
        }

        /// <summary>
        /// 設定読み出し処理
        /// </summary>
        public void Load()
        {
            if (File.Exists(_configFileName))
            {
                using FileStream fs = File.OpenRead(_configFileName);
                using StreamReader sr = new(fs, System.Text.Encoding.UTF8);
                Configuration readConfig = JsonSerializer.Deserialize<Configuration>(sr.ReadToEnd());
                IsAesModeCBC = readConfig.IsAesModeCBC;
                IsAesModeECB = readConfig.IsAesModeECB;
                AesKey = readConfig.AesKey;
                AesInitializationVector = readConfig.AesInitializationVector;
            }
            else
            {
                // ファイルがないときはデフォルト値で動作する
                IsAesModeCBC = false;
                IsAesModeECB = true;
                AesKey = @"00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
                AesInitializationVector = @"00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
            }
        }
    }
}
