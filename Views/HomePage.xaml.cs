using Luck.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Luck
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public  class Student
    {
        public  int ID { get; set; }
        public  string Name { get; set; }
    }

    public sealed partial class HomePage : Page
    {
        int i = 0;
        private bool isScrolling = false;
        private Task scrollingTask;
        private List<Storyboard> storyboards = new List<Storyboard>();
        public HomePage()
        {
            this.InitializeComponent();
            Random random = new Random();
        }

        private async Task ScrollNamesAsync()
        {
            NameContainer.Children.Clear();
           
            while (isScrolling)
            {
                TextBlock textBlock = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = TextAlignment.Center,
                    FontSize=30,
                    FontFamily =(FontFamily)Application.Current.Resources["HarmonyOSSans"],
                    Name = i.ToString(),
                    Text = App.NamesList[i].Name
                };


                Storyboard storyboard = new Storyboard();
                storyboards.Insert(0, storyboard);

                // 创建DoubleAnimation用于水平移动
                DoubleAnimation translateXAnimation = new DoubleAnimation();
                translateXAnimation.From = textBlock.RenderTransformOrigin.Y;
                translateXAnimation.To = textBlock.RenderTransformOrigin.Y + 10000; // 向右移动100像素
                translateXAnimation.Duration = TimeSpan.FromSeconds(5); // 动画持续时间为2秒

                // 创建TranslateTransform并应用到矩形
                TranslateTransform translateTransform = new TranslateTransform();
                textBlock.RenderTransform = translateTransform;

                // 设置动画目标和目标属性
                Storyboard.SetTarget(translateXAnimation, textBlock);
                Storyboard.SetTargetProperty(translateXAnimation, "(UIElement.RenderTransform).(TranslateTransform.Y)");

                // 将动画添加到Storyboard
                storyboard.Children.Add(translateXAnimation);


                NameContainer.Children.Insert(0, textBlock);
               

                // 启动Storyboard动画
                storyboard.Begin();

                i++;
                if (i >= App.NamesList.Count - 1) i = 0;
                if (i % 10 == 0) GC.Collect();

                await Task.Delay(10);
            }
        }

        private async void DestroyAsync(int id, int delay = 5000)
        {
            await Task.Delay(delay);
            try { 
                NameContainer.Children.RemoveAt(id);
                storyboards.RemoveAt(id);
            }
            catch (Exception)
            { 

            }

        }
        
        
        private async void StartorStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isScrolling)//未在滚动，本次点击按钮开始
            {
                isScrolling = true;
                StartorStopButton.Background = new SolidColorBrush(Color.FromArgb(255, 192, 242, 188));
                StartorStopButtonSymbolIcon.Symbol = Symbol.Stop;
                StartorStopButtonTextBlock.Text = "停止";
                RippleHelper.SetRippleColor(StartorStopButton,Colors.Yellow);
                WinnerBorder.Visibility = Visibility.Collapsed;
                scrollingTask = ScrollNamesAsync();
                await scrollingTask;
                
            }
            else//已在滚动，本次点击按钮停止
            {
                isScrolling = false;
                StartorStopButton.Background = new SolidColorBrush(Color.FromArgb(255, 192, 242, 188));
                StartorStopButtonSymbolIcon.Symbol = Symbol.Play;
                StartorStopButtonTextBlock.Text = "开始";
                RippleHelper.SetRippleColor(StartorStopButton, Colors.Green);

                int middleIndex = NameContainer.Children.Count / 2;
                TextBlock middleTextBlock = (TextBlock)NameContainer.Children[middleIndex];
                WinnerTextBlock.Text = (NameContainer.Children[3] as TextBlock).Text;
                WinnerBorder.Visibility = Visibility.Visible;
                
               

                foreach (var storyboard in storyboards)
                {
                    storyboard.Pause();
                }
            }
        }

    }
}
