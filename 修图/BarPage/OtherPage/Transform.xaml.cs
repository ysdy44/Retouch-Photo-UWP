using System;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Model;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;

namespace 修图.BarPage.OtherPage
{ 
    public sealed partial class Transform : Page
    {
        public Transform()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件     
             SetNumber();

            Render();
        }

        //绑定Refresh，画布刷新
        private void Refresh_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetNumber();
        }

        #endregion



        #region Snapping：吸附


        private void SnappingButton_Loaded(object sender, RoutedEventArgs e)
        {
            SnappingButton.Checked += SnappingButton_Checked;
            SnappingButton.Unchecked += SnappingButton_Unchecked;

            SnappingButton.IsChecked = App.Setting.TransformSnapping;
        }

        private void SnappingButton_Checked(object sender, RoutedEventArgs e)
        {
            App.Setting.TransformSnapping = true;
            Render();
        }
        private void SnappingButton_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Setting.TransformSnapping = false;
            Render();
        }


        #endregion

        #region Snapping：算法


        public static void Horizontal(HorizontalAlignment Alignment)
        {
            var TransformX = -(float)App.Setting.TransformRect.X + (float)(App.Setting.TransformW - App.Setting.TransformRect.Width) / 2;

            switch (Alignment)
            {
                case HorizontalAlignment.Left:
                    App.Setting.TransformX = TransformX;break;
                case HorizontalAlignment.Center:
                     App.Setting.TransformX  = TransformX+(float)App.Model.Width / 2 - App.Setting.TransformW / 2;break;
                case HorizontalAlignment.Right:
                     App.Setting.TransformX = TransformX+ App.Model.Width - App.Setting.TransformW;break;
                default:break;
            }
        }
        public static float GetHorizontal(HorizontalAlignment Alignment)
        {
            var TransformX = -(float)App.Setting.TransformRect.X + (float)(App.Setting.TransformW - App.Setting.TransformRect.Width) / 2;

            switch (Alignment)
            {
                case HorizontalAlignment.Left:
                    return TransformX;
                case HorizontalAlignment.Center:
                    return TransformX + (float)App.Model.Width / 2 - App.Setting.TransformW / 2;
                case HorizontalAlignment.Right:
                  return TransformX+ App.Model.Width - App.Setting.TransformW;
                default:return TransformX;
            }
        }




        public static void Vertical(VerticalAlignment Alignment)
        {
            var TransformY = -(float)App.Setting.TransformRect.Y + (float)(App.Setting.TransformH - App.Setting.TransformRect.Height) / 2;

            switch (Alignment)
            {
                case VerticalAlignment.Top:
                    App.Setting.TransformY = TransformY;break;
                case VerticalAlignment.Center:
                    App.Setting.TransformY = TransformY + (float)App.Model.Height / 2 - App.Setting.TransformH / 2;break;
                case VerticalAlignment.Bottom:
                    App.Setting.TransformY = TransformY + App.Model.Height - App.Setting.TransformH;break;
                default:break;
            }
        }

        public static float GetVertical(VerticalAlignment Alignment)
        {
            var TransformY = -(float)App.Setting.TransformRect.Y + (float)(App.Setting.TransformH - App.Setting.TransformRect.Height) / 2;

            switch (Alignment)
            {
                case VerticalAlignment.Top:
                  return TransformY;
                case VerticalAlignment.Center:
                    return TransformY + (float)App.Model.Height / 2 - App.Setting.TransformH / 2;
                case VerticalAlignment.Bottom:
                    return TransformY + App.Model.Height - App.Setting.TransformH;
                default:
                    return TransformY;
            }
        }

        #endregion

        #region Flip：镜像


        //Flip：镜像
        private void HorizontalButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Setting.TransformW = -App.Setting.TransformW;
            Render();
        }
        private void VerticalButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Setting.TransformH = -App.Setting.TransformH;
            Render();
        }


        //旋转
        private void LeftTurnButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Setting.TransformAngle -=(float) Math.PI/2;
            App.Setting.TransformAngle %= (float)(Math.PI * 2);
            Render();
        }
        private void RightTurnButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Setting.TransformAngle += (float)Math.PI / 2;
            App.Setting.TransformAngle %= (float)(Math.PI * 2);
            Render();
        }
        private void OverTurnButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Setting.TransformAngle -= (float)Math.PI;
            App.Setting.TransformAngle %= (float)(Math.PI * 2);
            Render();
        }


        #endregion

        #region Ratio：等比例


        private void RatioButton_Loaded(object sender, RoutedEventArgs e)
        {
            RatioButton.Checked += RatioButton_Checked;
            RatioButton.Unchecked += RatioButton_Unchecked;
            RatioButton.IsChecked = App.Setting.TransformRatio;
        }

        private void RatioButton_Checked(object sender, RoutedEventArgs e)
        {
            App.Setting.TransformRatio = true;
            Render();
        }
        private void RatioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Setting.TransformRatio = false;
            Render();
        }


        #endregion



        #region Transform：变换


        //数字选择器
        private void XNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.TransformX = Value;
            Render();
        }
        private void YNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.TransformY = Value;
            Render();
        }
        private void WNumberPicker_ValueChange(object sender, int Value)
        {
            //Ratio：等比例
            if (App.Setting.TransformRatio == true) App.Setting.TransformH *= Value / App.Setting.TransformW;
 
            App.Setting.TransformW = Value;
            Render();
        }
        private void HNumberPicker_ValueChange(object sender, int Value)
        {
            //Ratio：等比例
            if (App.Setting.TransformRatio == true) App.Setting.TransformW *= Value / App.Setting.TransformH;

            App.Setting.TransformH = Value;
            Render();
        }
        private void XSkewNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.TransformXSkew = (float)(Value * Math.PI / 180);
            Render();
        }
         private void YSkewNumberPicker_ValueChange(object sender, int Value)
        {
             App.Setting.TransformYSkew = (float)(Value * Math.PI / 180);
            Render();
        }
        private void RNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.TransformAngle = (float)(Value * Math.PI / 180);
            Render();
        }


        //上下左右
        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.TransformY--;
            YNumberPicker.Value =(int) App.Setting.TransformY;
            Render();
        }
        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.TransformY++;
            YNumberPicker.Value = (int)App.Setting.TransformY;
            Render();
        }
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.TransformX--;
            XNumberPicker.Value = (int)App.Setting.TransformX;
            Render();
        }
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.TransformX++;
            XNumberPicker.Value = (int)App.Setting.TransformX;
            Render();
        }


        //算法
        private void SetNumber()
        {
            XNumberPicker.ValueChange -= XNumberPicker_ValueChange;
            YNumberPicker.ValueChange -= YNumberPicker_ValueChange;
            WNumberPicker.ValueChange -= WNumberPicker_ValueChange;
            HNumberPicker.ValueChange -= HNumberPicker_ValueChange;
            XSkewNumberPicker.ValueChange -= XSkewNumberPicker_ValueChange;
            YSkewNumberPicker.ValueChange -= YSkewNumberPicker_ValueChange;
            RNumberPicker.ValueChange -= RNumberPicker_ValueChange;

            XNumberPicker.Value = (int)App.Setting.TransformX;
            YNumberPicker.Value = (int)App.Setting.TransformY;
            WNumberPicker.Value = (int)App.Setting.TransformW;
            HNumberPicker.Value = (int)App.Setting.TransformH;
            XSkewNumberPicker.Value = (int)(App.Setting.TransformXSkew * 180 / Math.PI);
            YSkewNumberPicker.Value = (int)(App.Setting.TransformYSkew * 180 / Math.PI);
            RNumberPicker.Value = (int)(App.Setting.TransformAngle * 180 / Math.PI);

            XNumberPicker.ValueChange += XNumberPicker_ValueChange;
            YNumberPicker.ValueChange += YNumberPicker_ValueChange;
            WNumberPicker.ValueChange += WNumberPicker_ValueChange;
            HNumberPicker.ValueChange += HNumberPicker_ValueChange;
            XSkewNumberPicker.ValueChange += XSkewNumberPicker_ValueChange;
            YSkewNumberPicker.ValueChange += YSkewNumberPicker_ValueChange;
            RNumberPicker.ValueChange += RNumberPicker_ValueChange;
        }
        private void NullNumber()
        {
            XNumberPicker.ValueChange -= XNumberPicker_ValueChange;
            YNumberPicker.ValueChange -= YNumberPicker_ValueChange;
            WNumberPicker.ValueChange -= WNumberPicker_ValueChange;
            HNumberPicker.ValueChange -= HNumberPicker_ValueChange;
            RNumberPicker.ValueChange -= RNumberPicker_ValueChange;

            XNumberPicker.Value = 0;
            YNumberPicker.Value = 0;
            WNumberPicker.Value = App.Model.Width;
            HNumberPicker.Value = App.Model.Height;
            RNumberPicker.Value = 0;

            XNumberPicker.ValueChange += XNumberPicker_ValueChange;
            YNumberPicker.ValueChange += YNumberPicker_ValueChange;
            WNumberPicker.ValueChange += WNumberPicker_ValueChange;
            HNumberPicker.ValueChange += HNumberPicker_ValueChange;
            RNumberPicker.ValueChange += RNumberPicker_ValueChange;
        }



        #endregion


        public static ICanvasImage GetRender(int tool)
        {
             if (tool == 102) //Mask：粘贴
                return App.Model.Clipboard;
            else if ( tool == 111)  //Transform：变换选区
                return App.Model.MaskRenderTarget;
            else if (tool == 308)  //Transform：变换
                return App.Model.SecondSourceRenderTarget;
           else //if (tool == 400)  //Layer：图片
                return App.Model.SecondCanvasBitmap;
        }

        public static void Render()
        {
            App.Model.SecondCanvasImage = new Transform2DEffect
            {
                Source = GetRender(App.Model.Tool),
                TransformMatrix = App.Setting.TransformMatrix,
                 InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
            };

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 
        }

    
    }
}
