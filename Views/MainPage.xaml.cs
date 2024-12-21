using Luck.Controls;
using System;
using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Luck
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer = new DispatcherTimer();
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
            timer.Tick += timer_Tick;
            timer.Start();
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
    }
}
