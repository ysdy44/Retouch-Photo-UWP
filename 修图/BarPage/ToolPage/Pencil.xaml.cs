using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace 修图.BarPage.ToolPage
{ 
    public sealed partial class Pencil : Page
    {
        public Pencil()
        {
            this.InitializeComponent(); 
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;

            HeightSlider.ValueChanged += HeightSlider_ValueChanged;
            HeightNumberPicker.ValueChange += HeightNumberPicker_ValueChange;
        }

        private async void Flyout_Opened(object sender, object e)
        {
            ShiwEllipse.Width = WidthSlider.Value;
            ShiwEllipse.Height = HeightSlider.Value;

            if (ShiwEllipse != null) await ShiwEllipse.Rotate(value: App.Model.InkAngle / (float)Math.PI * 180, centerX: (float)ShiwEllipse.Width / 2, centerY: (float)ShiwEllipse.Height / 2, duration: 0, delay: 0).StartAsync();
        }
        private void Flyout_Closed(object sender, object e)
        {
            App.Model.InkSet();//设置墨水
        }


        
         private void InkColorButton_ColorChanged(Color Color)
        {
            App.Model.InkSet();
        }

        //尺子
        private void RulerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Model.尺子.IsVisible == false && App.Model.量角器.IsVisible == false)
            {
                Ruler.Visibility = Visibility.Collapsed;
                尺子.Visibility = Visibility.Visible;
                量角器.Visibility = Visibility.Collapsed;

                App.Model.尺子.IsVisible = true;
                App.Model.量角器.IsVisible = false;
            }
            else if (App.Model.尺子.IsVisible == true && App.Model.量角器.IsVisible == false)
            {
                Ruler.Visibility = Visibility.Collapsed;
                尺子.Visibility = Visibility.Collapsed;
                量角器.Visibility = Visibility.Visible;

                App.Model.尺子.IsVisible = false;
                App.Model.量角器.IsVisible = true;
            }
            else if (App.Model.尺子.IsVisible == false && App.Model.量角器.IsVisible == true)
            {
                Ruler.Visibility = Visibility.Visible;
                尺子.Visibility = Visibility.Collapsed;
                量角器.Visibility = Visibility.Collapsed;

                App.Model.尺子.IsVisible = false;
                App.Model.量角器.IsVisible = false;
            }
        }
        


        #endregion


        #region InkCanvas：墨水画布


        private async void WidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.InkWidth = (float)e.NewValue;
            WidthNumberPicker.Value = (int)App.Model.InkWidth;

            ShiwEllipse.Width = App.Model.InkWidth;
            await ShiwEllipse.Rotate(value: App.Model.InkAngle / (float)Math.PI * 180, centerX: (float)ShiwEllipse.Width / 2, centerY: (float)ShiwEllipse.Height / 2, duration: 0, delay: 0).StartAsync();
        }
        private async void WidthNumberPicker_ValueChange(object sender, int Value)
        {
            App.Model.InkWidth = Value;
            WidthSlider.Value = Value;

            ShiwEllipse.Width = App.Model.InkWidth;
            await ShiwEllipse.Rotate(value: App.Model.InkAngle / (float)Math.PI * 180, centerX: (float)ShiwEllipse.Width / 2, centerY: (float)ShiwEllipse.Height / 2, duration: 0, delay: 0).StartAsync();
        }



        private async void HeightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.InkHeight = (float)e.NewValue;
            HeightNumberPicker.Value = (int)App.Model.InkHeight;

            ShiwEllipse.Height = App.Model.InkHeight;
            await ShiwEllipse.Rotate(value: App.Model.InkAngle / (float)Math.PI * 180, centerX: (float)ShiwEllipse.Width / 2, centerY: (float)ShiwEllipse.Height / 2, duration: 0, delay: 0).StartAsync();
        }
        private async void HeightNumberPicker_ValueChange(object sender, int Value)
        {
            App.Model.InkHeight = Value;
            HeightSlider.Value = Value;

            ShiwEllipse.Height = App.Model.InkHeight;
            await ShiwEllipse.Rotate(value: App.Model.InkAngle / (float)Math.PI * 180, centerX: (float)ShiwEllipse.Width / 2, centerY: (float)ShiwEllipse.Height / 2, duration: 0, delay: 0).StartAsync();
        }



        private async void RotationPicker_AngleChange(object sender, double Angle, double Rotation)
        {
            App.Model.InkAngle = (float)Angle;

            await ShiwEllipse.Rotate(value: -(float)Rotation+7, centerX: (float)ShiwEllipse.Width / 2, centerY: (float)ShiwEllipse.Height / 2, duration: 0, delay: 0).StartAsync();
        }



        private void ColorButton_ColorChanged(Color Color)
        {
            App.Model.InkSet();//设置墨水
        }



        #endregion


    }
}
