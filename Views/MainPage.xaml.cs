using Luck.Controls;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Linq;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Luck
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer = new DispatcherTimer();
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public MainPage()
        {
            this.InitializeComponent();
            // 隐藏系统标题栏并设置新的标题栏
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(AppTitleBar);
            //将标题栏右上角的3个按钮改为透明
            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            FlipSide.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnFlipSidePointerReleased), true);
            LoadingPageFrame.Navigate(typeof(LoadingPage));
            HomePageFrame.Navigate(typeof(HomePage));
            timer.Interval = TimeSpan.FromSeconds(5);
//#if DEBUG
//            timer.Interval = TimeSpan.FromSeconds(0);
//#endif
            timer.Tick += timer_Tick;
            timer.Start();
            Random random = new Random();
            List<string> students = new List<string>();
            UpdateLineNumbers();
            if (localSettings != null)
                Editor.Text = (string)localSettings.Values["Names"];
            string[] _lines = Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in _lines)
            {
                students.Add(line);
            }
            students = students.OrderBy(n => random.Next()).ToList();
            foreach (var item in students)
            {
                Student student = new Student()
                {
                    Name = item
                };
                App.NamesList.Add(student);
            }
        }

        private void timer_Tick(object sender, object e)
        {
            FlipSide.IsFlipped = !FlipSide.IsFlipped;
            timer.Stop();
        }

        private void OnFlipSidePointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var position = e.GetCurrentPoint(FlipSide).Position;
            var v2 = (position.ToVector2() - FlipSide.RenderSize.ToVector2() / 2);
            FlipSide.Axis = new Vector2(-v2.Y, v2.X);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }
        public void UpdateLineNumbers()
        {
            string text = Editor.Text;
            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            lineNumberTextBlock.Text = string.Empty;
            for (int i = 0; i < lines.Length; i++)
            {
                lineNumberTextBlock.Text += $"{i + 1}\n";
            }
        }

        private void ClassNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["ClassName"] = ClassNameTextBox.Text;
        }


        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".txt");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    // 打开文件并读取内容
                    string text = await FileIO.ReadTextAsync(file);
                    Editor.Text = text;
                    ClassNameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                    localSettings.Values["Names"] = Editor.Text;

                    PopupNotice popupNotice = new PopupNotice("成功打开文件: " + file.Name);
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
                }
                catch (Exception)
                {
                    PopupNotice popupNotice = new PopupNotice("打开文件失败");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("打开操作已取消");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
        }
        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建文件选取器
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("文本文档", new List<string>() { ".txt" });
            string filename = ClassNameTextBox.Text;
            savePicker.SuggestedFileName = filename;

            // 显示文件选取器并等待用户选择文件
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // 将文本写入文件
                string text = Editor.Text;
                await FileIO.WriteTextAsync(file, text);
                Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    PopupNotice popupNotice = new PopupNotice(file.Name + " 保存成功");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
                }
                else
                {
                    PopupNotice popupNotice = new PopupNotice(file.Name + " 保存失败");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("保存操作已取消");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
        }
        public void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                localSettings.Values["Names"] = Editor.Text;
            }
            catch (Exception)
            {
                PopupNotice popupNotice = new PopupNotice("保存失败，请删除部分名字后重试");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            UpdateLineNumbers();

        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
