using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
 
 namespace 修图.BarPage.ToolPage
{ 
    public sealed partial class Hand : Page
    {

        bool isBinding = false;//是否绑定的滑条改变了滑条鱼数字选择器内容
        public Hand()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局


        private void _ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            isBinding = true;
            Slider.Value = NumberPicker.Value = (int)App.Model.CanvasWidth;// Math.Min(App.Model.CanvasWidth, App.Model.CanvasHeight);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
              int wh = App.Model.Width > App.Model.Height ? (int)App.Model.CanvasHeight : (int)App.Model.CanvasWidth;
            Slider.Value = NumberPicker.Value = wh;
 
            Slider.ValueChanged += Slider_ValueChanged;
            NumberPicker.ValueChange += NumberPicker_ValueChange;
        }


        #endregion



        #region Hand：手掌


        private void NumberPicker_ValueChange(object sender, int Value)
        {
            if (isBinding == false)
            {
                Slider.Value = Value;

                if (App.Model.Width < App.Model.Height) WidthChanged(Value);
                else HeightChanged(Value);

                App.Model.isReStroke = true;//重新套索
                App.Model.Refresh++;//更新画布    
            }
            isBinding = false;
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isBinding == false)
            {
                if (NumberPicker != null) NumberPicker.Value = (int)e.NewValue;

                if (App.Model.Width < App.Model.Height) WidthChanged(e.NewValue);
                else HeightChanged(e.NewValue);

                App.Model.isReStroke = true;//重新套索
                App.Model.Refresh++;//更新画布
            }
            isBinding = false;
        }


        #endregion



        private void WidthChanged(double Value)
        {
            App.Model.CanvasWidth = Value;
            App.Model.CanvasHeight = App.Model.CanvasWidth / App.Model.Width * App.Model.Height;
            App.Model.X = (App.Model.GridWidth - App.Model.CanvasWidth) / 2;
            App.Model.Y = (App.Model.GridHeight - App.Model.CanvasHeight) / 2;
        }
        private void HeightChanged(double Value)
        {
            App.Model.CanvasHeight = Value;
            App.Model.CanvasWidth = App.Model.CanvasHeight / App.Model.Height * App.Model.Width;
            App.Model.X = (App.Model.GridWidth - App.Model.CanvasWidth) / 2;
            App.Model.Y = (App.Model.GridHeight - App.Model.CanvasHeight) / 2;
        } 

    
    }
}
