using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CryptoUtilities.Models
{
    /// <summary>
    /// 例外ハンドラークラス
    /// </summary>
    public class ExceptionHandler
    {
        //--------------------------------------------------
        // メソッド
        //--------------------------------------------------
        /// <summary>
        /// DispatcherUnhandledExceptionイベント発生時の処理
        /// </summary>
        /// <param name="sender">イベントソース</param>
        /// <param name="e">イベントデータ</param>
        public static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) => HandleException(e.Exception);

        /// <summary>
        /// UnobservedTaskExceptionイベント発生時の処理
        /// </summary>
        /// <param name="sender">イベントソース</param>
        /// <param name="e">イベントデータ</param>
        public static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) => HandleException(e.Exception.InnerException);

        /// <summary>
        /// UnhandledExceptionイベント発生時の処理
        /// </summary>
        /// <param name="sender">イベントソース</param>
        /// <param name="e">イベントデータ</param>
        public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) => HandleException((Exception)e.ExceptionObject);

        /// <summary>
        /// 例外発生時の処理
        /// </summary>
        /// <param name="e">例外情報</param>
        private static void HandleException(Exception exception)
        {
            _ = MessageBox.Show(exception.ToString(), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);

            // 終了する｡
            Environment.Exit(1);
        }
    }
}
