using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace InkApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            inkCanvas.InkPresenter.InputDeviceTypes =
              Windows.UI.Core.CoreInputDeviceTypes.Mouse |
              Windows.UI.Core.CoreInputDeviceTypes.Pen |
              Windows.UI.Core.CoreInputDeviceTypes.Touch;
        }


        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {

            // ユーザーがファイルピッカーを使用してINKファイルを選択する
            // ピッカーの初期化
            Windows.Storage.Pickers.FileOpenPicker openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".gif");
            // ピッカーを表示
            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                // ストリームの読み込み
                IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                // ファイルの読み込み
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(stream);
                }
                stream.Dispose();
            }
            // ユーザーが キャンセル を選択すると、ピッカーは null が返る
            else
            {
                // キャンセル操作
            }

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // InkCanvas 上のすべてのストロークを取得
            IReadOnlyList<InkStroke> currentStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            if (currentStrokes.Count > 0)
            {
                // ユーザーがファイルピッカーを使用してINKファイルを選択する
                // ピッカーの初期化
                Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("GIF with embedded ISF", new List<string>() { ".gif" });
                savePicker.DefaultFileExtension = ".gif";
                savePicker.SuggestedFileName = "InkSample";

                // ファイルピッカーを表示
                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {

                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    // 書き込みのためにファイルストリームを開く
                    IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                    // 出力ストリームにインクストロークを出力
                    using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
                    {
                        await inkCanvas.InkPresenter.StrokeContainer.SaveAsync(outputStream);
                        await outputStream.FlushAsync();
                    }
                    stream.Dispose();

                    // ファイルを更新できるように出力を確定
                    Windows.Storage.Provider.FileUpdateStatus status =
                        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);

                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        // 保存
                    }
                    else
                    {
                        // 保存できなかった
                    }
                }
                // ユーザーが キャンセル を選択すると、ピッカーは null が返る
                else
                {
                    // キャンセル操作
                }
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
        }

    }
}
