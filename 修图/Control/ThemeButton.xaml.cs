using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;

namespace 修图.Control
{
    public sealed partial class ThemeButton : UserControl
    {
        ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
 
        //Delegate
        public delegate void ThemeChangeHandler(object sender, ElementTheme Theme);
        public event ThemeChangeHandler ThemeChange = null;

        public ThemeButton()
        {
            this.InitializeComponent();
        }


        #region Global：全局


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.localSettings.Values.ContainsKey("isLight") == false)  //判断本地设置有没有这个字典
            {
                App.localSettings.Values["isLight"] = true;//资源容器
            }

            if ((bool)App.localSettings.Values["isLight"] == false)  //判断是不是明主题
            {
                App.Model.Theme = ElementTheme.Dark;
                ToNight.Begin();
                Night();
            }
            else
            {
                App.Model.Theme = ElementTheme.Light;
                ToLight.Begin();
                Light();
            }
   
            ThemeChange?.Invoke(this, App.Model.Theme);
        }
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Model.Theme == ElementTheme.Light)
            {
                App.localSettings.Values["isLight"] = false;//资源容器

                ToNight.Begin();
                Night();
                App.Model.Theme =  this.RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                App.localSettings.Values["isLight"] = true;//资源容器

                ToLight.Begin();
                Light();
                App.Model.Theme =   this.RequestedTheme = ElementTheme.Light;
            }


            ThemeChange?.Invoke(this, App.Model.Theme);
            App.Model.Refresh++;//画布刷新
        }



        #endregion


        #region Theme：主题


        private void Night()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = ElementTheme.Dark;
            }
            App.Model.Theme = ElementTheme.Dark;

            App.Model.AcrylicBrush = (AcrylicBrush)this.Resources["NightAcrylicBrush"];
            App.Model.OpacityBrush = (SolidColorBrush)this.Resources["NightOpacityBrush"];

            App.Model.TitleColor = (SolidColorBrush)this.Resources["NightTitleColor"];
            App.Model.ButtonColor = (SolidColorBrush)this.Resources["NightButtonColor"];

            App.Model.Background = (SolidColorBrush)this.Resources["NightBackground"];// (SolidColorBrush)this.Resources["NightBackground"];
            App.Model.PanelColor = (SolidColorBrush)this.Resources["NightPanelColor"];

            App.Model.LineColor = (SolidColorBrush)this.Resources["NightLineColor"];
            App.Model.TextBackColor = (SolidColorBrush)this.Resources["NightTextBackColor"];

            App.Model.Foreground = (SolidColorBrush)this.Resources["NightForeground"];
            App.Model.SignForeground = (SolidColorBrush)this.Resources["NightSignForeground"];

            //状态栏颜色：更暗的颜色
            //Color StatusBarColor = DarkerColor(App.Model.TitleColor.Color, -13);
            Color StatusBarColor = App.Model.TitleColor.Color;
            TitleBar.ButtonInactiveBackgroundColor = TitleBar.ButtonBackgroundColor = StatusBarColor;
      TitleBar.InactiveBackgroundColor = TitleBar.BackgroundColor = StatusBarColor;
        }

        private void Light()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = ElementTheme.Light;
            }

            App.Model.Theme = ElementTheme.Light;

            App.Model.AcrylicBrush = (AcrylicBrush)this.Resources["LightAcrylicBrush"];
            App.Model.OpacityBrush = (SolidColorBrush)this.Resources["LightOpacityBrush"];

            App.Model.TitleColor = (SolidColorBrush)this.Resources["LightTitleColor"];
            App.Model.ButtonColor = (SolidColorBrush)this.Resources["LightButtonColor"];

            App.Model.Background = (SolidColorBrush)this.Resources["LightBackground"];
            App.Model.PanelColor = (SolidColorBrush)this.Resources["LightPanelColor"];

            App.Model.LineColor = (SolidColorBrush)this.Resources["LightLineColor"];
            App.Model.TextBackColor = (SolidColorBrush)this.Resources["LightTextBackColor"];

            App.Model.Foreground = (SolidColorBrush)this.Resources["LightForeground"];
            App.Model.SignForeground = (SolidColorBrush)this.Resources["LightSignForeground"];

            //状态栏颜色：更暗的颜色
            //Color StatusBarColor = DarkerColor(App.Model.TitleColor.Color, -13);
            Color StatusBarColor = App.Model.TitleColor.Color;
            TitleBar.ButtonInactiveBackgroundColor = TitleBar.ButtonBackgroundColor = StatusBarColor;
          TitleBar.InactiveBackgroundColor = TitleBar.BackgroundColor = StatusBarColor;
        }


        //返回更暗的颜色
        private Color DarkerColor(Color color,int amount)
        {
            int R = color.R + amount;
            int G = color.G + amount;
            int B = color.B + amount;

            return Color.FromArgb(color.A, (byte)R, (byte)G, (byte)B);
         }


        #endregion


    }
}
