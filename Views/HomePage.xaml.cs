using Luck.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Luck
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private List<string> nameList;
        private bool isScrolling = false;
        private Task scrollingTask;
        public HomePage()
        {
            this.InitializeComponent();
            nameList = new List<string> { "张三", "李四", "王五", "赵六", "孙七", "周八", "吴九" };
        }

        private async Task ScrollNamesAsync()
        {
            Random random = new Random();
            while (isScrolling)
            {
                var shuffledNames = nameList.OrderBy(n => random.Next()).ToList();
                NameListBox.ItemsSource = shuffledNames;
                //int i = 0;
                //while (i < shuffledNames.Count)
                //{
                //    NameListBox.SelectedIndex = i;
                //    await Task.Delay(10);
                //    i++;
                //    if (i == shuffledNames.Count)
                //    {
                //        i = 0;
                //    }
                //}
                NameListBox.SelectedIndex = random.Next(shuffledNames.Count);
                await Task.Delay(10);
            }
        }

        private async void StartorStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isScrolling)//未在滚动，本次点击按钮开始
            {
                isScrolling = true;
                StartorStopButton.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                StartorStopButtonSymbolIcon.Symbol = Symbol.Stop;
                StartorStopButtonTextBlock.Text = "停止";
                //StartorStopButton.RippleHelper.RippleColor = Colors.Red;
                WinnerBorder.Visibility = Visibility.Collapsed;
                scrollingTask = ScrollNamesAsync();
                await scrollingTask;
            }
            else//已在滚动，本次点击按钮停止
            {
                isScrolling = false;
                StartorStopButton.Background = new SolidColorBrush(Color.FromArgb(255, 192, 242, 188));
                StartorStopButtonSymbolIcon.Symbol = Symbol.Play;
                StartorStopButtonTextBlock.Text = "停止";
                var winner = NameListBox.Items[NameListBox.SelectedIndex] as string;
                WinnerTextBlock.Text = winner;
                WinnerBorder.Visibility = Visibility.Visible;
            }
        }
    }
}
