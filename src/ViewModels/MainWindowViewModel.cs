using CryptoUtilities.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;

namespace CryptoUtilities.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        //--------------------------------------------------
        // バインディングデータ
        //--------------------------------------------------
        /// <summary>
        /// タイトル
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        /// <summary>
        /// AES:CBCモードかどうか
        /// </summary>
        private bool _isAesModeCBC;
        public bool IsAesModeCBC
        {
            get { return _isAesModeCBC; }
            set { SetProperty(ref _isAesModeCBC, value); }
        }

        /// <summary>
        /// AES:ECBモードかどうか
        /// </summary>
        private bool _isAesModeECB;
        public bool IsAesModeECB
        {
            get { return _isAesModeECB; }
            set { SetProperty(ref _isAesModeECB, value); }
        }

        /// <summary>
        /// AES:キー
        /// </summary>
        private string _aesKey;
        public string AesKey
        {
            get { return _aesKey; }
            set { SetProperty(ref _aesKey, value); }
        }

        /// <summary>
        /// AES:初期化ベクター
        /// </summary>
        private string _aesInitializationVector;
        public string AesInitializationVector
        {
            get { return _aesInitializationVector; }
            set { SetProperty(ref _aesInitializationVector, value); }
        }

        /// <summary>
        /// AES:プレーンテキスト
        /// </summary>
        private string _aesPlainText = @"00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        public string AesPlainText
        {
            get { return _aesPlainText; }
            set { SetProperty(ref _aesPlainText, value); }
        }

        /// <summary>
        /// AES:暗号文
        /// </summary>
        private string _aesCipherText = @"00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        public string AesCipherText
        {
            get { return _aesCipherText; }
            set { SetProperty(ref _aesCipherText, value); }
        }

        //--------------------------------------------------
        // バインディングコマンド
        //--------------------------------------------------
        /// <summary>
        /// Closedイベント時コマンド
        /// </summary>
        private DelegateCommand _commandClosed;
        public DelegateCommand CommandClosed =>
            _commandClosed ?? (_commandClosed = new DelegateCommand(ExecuteCommandClosed));

        /// <summary>
        /// AES暗号化コマンド
        /// </summary>
        private DelegateCommand _commandAesEncrypt;
        public DelegateCommand CommandAesEncrypt =>
            _commandAesEncrypt ?? (_commandAesEncrypt = new DelegateCommand(ExecuteCommandAesEncrypt));

        /// <summary>
        /// AES復号化コマンド
        /// </summary>
        private DelegateCommand _commandAesDecrypt;
        public DelegateCommand CommandAesDecrypt =>
            _commandAesDecrypt ?? (_commandAesDecrypt = new DelegateCommand(ExecuteCommandAesDecrypt));

        //--------------------------------------------------
        // 内部変数
        //--------------------------------------------------
        private Configuration _configuration = new();

        //--------------------------------------------------
        // 内部定義
        //--------------------------------------------------
        private enum EncryptDecrypt
        {
            Encrypt,
            Decrypt
        }

        //--------------------------------------------------
        // メソッド
        //--------------------------------------------------
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            // バージョン情報を取得してタイトルに反映する
            Assembly assm = Assembly.GetExecutingAssembly();
            string version = assm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Title = $"{Resources.Strings.ApplicationName} Ver.{version}";

            // 設定の読み出し
            _configuration.Load();

            // 設定の反映
            IsAesModeCBC = _configuration.IsAesModeCBC;
            IsAesModeECB = _configuration.IsAesModeECB;
            AesKey = _configuration.AesKey;
            AesInitializationVector = _configuration.AesInitializationVector;
        }

        /// <summary>
        /// Closedイベントハンドラ
        /// </summary>
        private void ExecuteCommandClosed()
        {
            // 設定の保存
            _configuration.IsAesModeCBC = IsAesModeCBC;
            _configuration.IsAesModeECB = IsAesModeECB;
            _configuration.AesKey = AesKey;
            _configuration.AesInitializationVector = AesInitializationVector;
            _configuration.Save();
        }

        /// <summary>
        /// AES暗号化コマンド実行
        /// </summary>
        private void ExecuteCommandAesEncrypt()
        {
            // AES入力無効の場合は終了
            if (!IsEnableInputAes(EncryptDecrypt.Encrypt))
            {
                return;
            }

            // モード決定
            CipherMode mode = CipherMode.CBC;
            if (IsAesModeCBC)
            {
                mode = CipherMode.CBC;
            }
            else if (IsAesModeECB)
            {
                mode = CipherMode.ECB;
            }
            else
            {
                return;
            }

            // Byte配列に変換
            byte[] plainTextBytes = HexStringToBytes(AesPlainText);
            byte[] keyBytes = HexStringToBytes(AesKey);
            byte[] initializationVectorBytes = HexStringToBytes(AesInitializationVector);

            // 暗号化
            byte[] encryptedBytes = CryptAes.Encrypt(plainTextBytes, keyBytes, initializationVectorBytes, mode);
            AesCipherText = BytesToHexString(encryptedBytes);
        }

        /// <summary>
        /// AES復号化コマンド実行
        /// </summary>
        private void ExecuteCommandAesDecrypt()
        {
            // AES入力無効の場合は終了
            if (!IsEnableInputAes(EncryptDecrypt.Decrypt))
            {
                return;
            }

            // モード決定
            CipherMode mode = CipherMode.CBC;
            if (IsAesModeCBC)
            {
                mode = CipherMode.CBC;
            }
            else if (IsAesModeECB)
            {
                mode = CipherMode.ECB;
            }
            else
            {
                return;
            }

            // Byte配列に変換
            byte[] cipherTextBytes = HexStringToBytes(AesCipherText);
            byte[] keyBytes = HexStringToBytes(AesKey);
            byte[] initializationVectorBytes = HexStringToBytes(AesInitializationVector);

            // 復号化
            byte[] decryptedBytes = CryptAes.Decrypt(cipherTextBytes, keyBytes, initializationVectorBytes, mode);
            AesPlainText = BytesToHexString(decryptedBytes);
        }

        /// <summary>
        /// 16進数文字列->Byte配列変換処理
        /// </summary>
        /// <param name="hexString">16進数文字列</param>
        /// <returns>Byte配列</returns>
        private static byte[] HexStringToBytes(string hexString)
        {
            var splited = hexString.Split(' ').Select(hex => Convert.ToByte(hex, 16));
            return splited.ToArray();
        }

        /// <summary>
        /// Byte配列->16進数文字列変換処理
        /// </summary>
        /// <param name="bytes">Byte配列</param>
        /// <returns>16進数文字列</returns>
        private static string BytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        /// <summary>
        /// AES入力有効確認
        /// </summary>
        /// <param name="type">暗号化or復号化</param>
        /// <returns>入力有効or入力向こう</returns>
        private bool IsEnableInputAes(EncryptDecrypt type)
        {
            // プレーンテキストチェック
            if ((type == EncryptDecrypt.Encrypt) && (AesPlainText == string.Empty))
            {
                _ = MessageBox.Show(Resources.Strings.MessagePlainTextEmptyError, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // 暗号文チェック
            if ((type == EncryptDecrypt.Decrypt) && (AesCipherText == string.Empty))
            {
                _ = MessageBox.Show(Resources.Strings.MessageCipherTextEmptyError, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // キーの有無
            if(AesKey == string.Empty)
            {
                _ = MessageBox.Show(Resources.Strings.MessageKeyEmptyError, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // キーの長さ
            // LegalKeySizes:MinSize=128 MaxSize=256 SkipSize=64
            byte[] keyBytes = HexStringToBytes(AesKey);
            int keyBitSize = keyBytes.Length * 8;
            if (keyBitSize is not 128 and not 192 and not 256)
            {
                _ = MessageBox.Show(Resources.Strings.MessageKeyLengthError, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // 初期化ベクターの有無
            if(AesInitializationVector == string.Empty)
            {
                _ = MessageBox.Show(Resources.Strings.MessageInitializationVectorEmptyError, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // 初期化ベクターの長さ(CBCモード時に使用)
            // IV:The size of the IV property must be the same as the BlockSize property divided by 8.
            // BlockSize:For AES, the only valid block size is 128 bits.
            byte[] initializationVectorBytes = HexStringToBytes(AesInitializationVector);
            if ((IsAesModeCBC) && (initializationVectorBytes.Length != 16))
            {
                _ = MessageBox.Show(Resources.Strings.MessageInitializationVectoLengthError, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
