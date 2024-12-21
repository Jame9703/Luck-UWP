using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Luck
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            StartAnimation();
        }
        private void StartAnimation()
        {
            DoubleAnimation verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = 0;
            verticalAnimation.To = 100;
            verticalAnimation.Duration = TimeSpan.FromSeconds(10);  // 动画持续时间，可调整
            verticalAnimation.RepeatBehavior = RepeatBehavior.Forever;
            verticalAnimation.AutoReverse = false;

            Storyboard.SetTarget(verticalAnimation, scrollViewerForAnimation);
            Storyboard.SetTargetProperty(verticalAnimation, "(ScrollViewer.VerticalOffset)");

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(verticalAnimation);
            storyboard.Begin(); 
        }
    }
}
