using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
using 修图.Library;

 
namespace 修图.BarPage.EffectPage
{
    public sealed partial class Brightness : Page
    {
        public Brightness()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {           
            //控件事件
            BrightnessSlider.Value = BrightnessNumberPicker.Value= (int)(App.Setting.Brightness * 100f);
            BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;
            BrightnessNumberPicker.ValueChange += BrightnessNumberPicker_ValueChange;

            Render();
        }

        private void flyout_Opened(object sender, object e)
        {
            //控件事件
            SetWhite(new Point(FloatToCanvas(App.Setting.BrightnessWhiteX) + 10d, FloatToCanvas(1.0f - App.Setting.BrightnessWhiteY) + 10d));
            SetBlack(new Point(FloatToCanvas(App.Setting.BrightnessBlackX) + 10d, FloatToCanvas(1.0f - App.Setting.BrightnessBlackY) + 10d));
        }

        #endregion

        #region Brightness：亮度


        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BrightnessNumberPicker.Value = (int)e.NewValue;
            App.Setting.Brightness = (float)e.NewValue / 100;

            BrightnesstoPoint(App.Setting.Brightness);
            Render();
        }

        private void BrightnessNumberPicker_ValueChange(object sender, int Value)
        {
            BrightnessSlider.Value = Value;
            App.Setting.Brightness = (float)Value / 100;

            BrightnesstoPoint(App.Setting.Brightness);
            Render();
        }


        private void BrightnesstoPoint(float Brightness)
        {
            App.Setting.BrightnessWhiteX = Math.Min(2 - Brightness, 1);
            App.Setting.BrightnessWhiteY = 1f;
            App.Setting.BrightnessBlackX = Math.Max(1 - Brightness, 0);
            App.Setting.BrightnessBlackY = 0f;
        }


        #endregion

        #region Brightness：亮度曲线


        private void Remove_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //重置
            App.Setting.BrightnessBlackX = App.Setting.BrightnessBlackY = 0f;
            App.Setting.BrightnessWhiteX = App.Setting.BrightnessWhiteY = 1f;

            SetWhite(new Point(FloatToCanvas(App.Setting.BrightnessWhiteX) + 10d, FloatToCanvas(1.0f - App.Setting.BrightnessWhiteY) + 10d));
            SetBlack(new Point(FloatToCanvas(App.Setting.BrightnessBlackX) + 10d, FloatToCanvas(1.0f - App.Setting.BrightnessBlackY) + 10d));

            Render();
        }




        enum point
        {
            None,
            White,
            Black
        }
        point Points = point.None;

        private void BrightnessCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Point p = Judge.Position(e, BrightnessCanvas);

            if (p.X >= 10 && p.X < 110 && p.Y >= 110 && p.Y < 210) Points = point.Black;
            else if (p.X > 110 && p.X <= 210 && p.Y > 10 && p.Y <= 110) Points = point.White;
            else Points = point.None;
        }
        private void BrightnessCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point p = Judge.Position(e, BrightnessCanvas);

            if (Points == point.Black)
            {
                if (p.X < 10) p.X = 10;
                else if (p.X > 109) p.X = 109;
                if (p.Y < 110) p.Y = 110;
                else if (p.Y > 210) p.Y = 210;

                SetBlack(p);
                App.Setting.BrightnessBlackX = CanvasToFloat(p.X);
                App.Setting.BrightnessBlackY = 1 - CanvasToFloat(p.Y);

                Render();
            }
            else if (Points == point.White)
            {
                if (p.X < 111) p.X = 111;
                else if (p.X > 210) p.X = 210;
                if (p.Y < 10) p.Y = 10;
                else if (p.Y > 109) p.Y = 109;

                SetWhite(p);
                App.Setting.BrightnessWhiteX = CanvasToFloat(p.X);
                App.Setting.BrightnessWhiteY = 1 - CanvasToFloat(p.Y);

                Render();
            }
        }


        private void BrightnessCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Points = point.None;

            Render();
        }
        private void BrightnessCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Points = point.None;

            Render();
        }




        //转换
        private double FloatToCanvas(float Float)
        {
            return Float * 200;
        }
        private float CanvasToFloat(double Double)
        {
            return (float)(Double / 200);
        }

        public double Distance(Point A, Point B)
        {
            return Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));//椭圆半焦距
        }




        //设置
        private void SetWhite(Point p)
        {
            Bezier.Point2 = p;
            WhiteLine.X2 = p.X;
            WhiteLine.Y2 = p.Y;

            Canvas.SetLeft(WhitePoint, p.X - 10);
            Canvas.SetTop(WhitePoint, p.Y - 10);
        }
        private void SetBlack(Point p)
        {
            Bezier.Point1 = p;
            BlackLine.X2 = p.X;
            BlackLine.Y2 = p.Y;

            Canvas.SetLeft(BlackPoint, p.X - 10);
            Canvas.SetTop(BlackPoint, p.Y - 10);
        }


        #endregion



        private void Render()
        {
            //Brightness：亮度
            App.Model.SecondCanvasImage = Adjust.GetBrightness(App.Model.SecondSourceRenderTarget, App.Setting.BrightnessWhiteX, App.Setting.BrightnessWhiteY, App.Setting.BrightnessBlackX, App.Setting.BrightnessBlackY);//得到曝光

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }


    }
}
