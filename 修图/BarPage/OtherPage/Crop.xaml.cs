using System;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Model;
using Microsoft.Graphics.Canvas.Geometry;

namespace 修图.BarPage.OtherPage
{ 
    public sealed partial class Crop : Page
    {
        public Crop()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件     
            App.Setting.CropAngle = 0f;
            SetNumber();
            Render();
        }

        //绑定Refresh，画布刷新
        private void Refresh_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetNumber();
        }

        #endregion


        #region Crop：裁切


        //旋转滑条
        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.CropAngle = (float)(e.NewValue * Math.PI / 180);

            Render();
        }


        //数字选择器
        private void XNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CropX = Value;
            App.Model.Refresh++;//画布刷新 
        }
        private void YNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CropY = Value;
            App.Model.Refresh++;//画布刷新 
        }
        private void WNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CropW = Value;
            App.Model.Refresh++;//画布刷新 
        }
        private void HNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CropH = Value;
            App.Model.Refresh++;//画布刷新 
        }
        private void RNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CropAngle = (float)(Value * Math.PI / 180);
            Render();
        }


        //上下左右
        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.CropY--;
            App.Model.Refresh++;//画布刷新 
        }
        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.CropY++;
            App.Model.Refresh++;//画布刷新 
        }
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.CropX--;
            App.Model.Refresh++;//画布刷新 
        }
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.CropX++;
            App.Model.Refresh++;//画布刷新 
        }


        //算法
        private void SetNumber()
        {
            XNumberPicker.ValueChange -= XNumberPicker_ValueChange;
            YNumberPicker.ValueChange -= YNumberPicker_ValueChange;
            WNumberPicker.ValueChange -= WNumberPicker_ValueChange;
            HNumberPicker.ValueChange -= HNumberPicker_ValueChange;
            RNumberPicker.ValueChange -= RNumberPicker_ValueChange;

            XNumberPicker.Value = (int)App.Setting.CropX;
            YNumberPicker.Value = (int)App.Setting.CropY;
            WNumberPicker.Value = (int)App.Setting.CropW;
            HNumberPicker.Value = (int)App.Setting.CropH;
            RNumberPicker.Value = (int)(App.Setting.CropAngle * 180 / Math.PI);

            XNumberPicker.ValueChange += XNumberPicker_ValueChange;
            YNumberPicker.ValueChange += YNumberPicker_ValueChange;
            WNumberPicker.ValueChange += WNumberPicker_ValueChange;
            HNumberPicker.ValueChange += HNumberPicker_ValueChange;
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



        public static void Render(bool isNumber = false)
        {
            App.Model.SecondCanvasImage = new Transform2DEffect
            {
                Source = App.Model.SecondTopRenderTarget,
                TransformMatrix = Matrix3x2.CreateTranslation(-App.Model.Width / 2, -App.Model.Height / 2) * Matrix3x2.CreateRotation(App.Setting.CropAngle) * Matrix3x2.CreateTranslation(App.Model.Width / 2, App.Model.Height / 2)
            };
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 
        }

        public static void Apply()
        {
            //XYWH
            int X = (int)App.Setting.CropX;
            int Y = (int)App.Setting.CropY;
            int W = (int)App.Setting.CropW;
            int H = (int)App.Setting.CropH;
            Matrix3x2 Matrix = Matrix3x2.CreateTranslation(-App.Model.Width / 2, -App.Model.Height / 2) * Matrix3x2.CreateRotation(App.Setting.CropAngle) * Matrix3x2.CreateTranslation(App.Model.Width / 2, App.Model.Height / 2) * Matrix3x2.CreateTranslation(-X, -Y);

            //跟随宽高 
            App.Model.X += (App.Model.Width - W) / 2 * App.Model.XS;
            App.Model.Y += (App.Model.Height - H) / 2 * App.Model.YS;
            App.Model.CanvasWidth = W * App.Model.XS;
            App.Model.CanvasHeight = H * App.Model.YS;
            App.Model.Width = W;
            App.Model.Height = H;

            foreach (Layer L in App.Model.Layers)
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, W, H);
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(new Transform2DEffect
                    {
                        Source = L.CanvasRenderTarget,
                        TransformMatrix = Matrix
                    });
                }
                L.CanvasRenderTarget = crt;
                L.SetWriteableBitmap(App.Model.VirtualControl);

                if (App.Model.isLowView) L.LowView();
                else L.SquareView();
            }

            //初始化
            App.Initialize(App.Model.VirtualControl, W, H);
            App.Model.MaskAnimatedTarget = new CanvasRenderTarget(App.Model.AnimatedControl,W, H);

            //Undo：撤销
            App.Model.Undos.Clear();//清空
            App.Model.UndoIndex = 0;
             App.Model.isUndo = false;
            App.Model.isRedo = false;
         }

    }
}
