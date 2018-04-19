using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using 修图.Model;

namespace 修图.Control
{
    public sealed partial class OtherButton : UserControl
    {
        //Delegate
        public delegate void OtherHandler(int Index);
        public event OtherHandler Other;
         
        public OtherButton()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局
         
 
        private void flyout_Opened(object sender, object e)
        {
            App.Judge();//判断选区，改变是否动画与选区矩形
        }

        private void ToOther(int Index,bool isTip=false)
        {
            flyout.Hide();

            if (App.Model.Index < 0) App.Model.Index = 0;//图层索引
            Layer l = App.Model.Layers[App.Model.Index];

            if (isTip==false)//是否提示图层可视
            {
                 //如果图层不可视或透明
                if (l.Visual == false || l.Opacity <= 0) App.Tip();
                else Other(Index);
            }
            else
            {
                Other(Index);
            }
        }




        #endregion


        #region Stretch：拉伸


        private int Width;
        private int Height;
        private bool isGeometric;//是否等比

        private async void OtherStretchButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            WidthNumberPicker.Value = Width = App.Model.Width;
            HeightNumberPicker.Value= Height = App.Model.Height;

            if (isGeometric==true) GeometricIcon.Opacity = 1;
            else GeometricIcon.Opacity = 0.5;

            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;
            HeightNumberPicker.ValueChange += HeightNumberPicker_ValueChange;

            flyout.Hide(); 
            await StretchContentDialog.ShowAsync();
        }
        private void GeometricButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (isGeometric == false)
             {
                isGeometric = true;
                GeometricIcon.Opacity = 1;
            }
            else
            {
                isGeometric = false;
                GeometricIcon.Opacity = 0.5;
            }
        }


        private void WidthNumberPicker_ValueChange(object sender, int Value)
        {
            if (isGeometric == true)
            {
                Height = Value*App.Model.Height / App.Model.Width;
                Width = Value;
                
                HeightNumberPicker.ValueChange -= HeightNumberPicker_ValueChange;
                HeightNumberPicker.Value = Height;
                HeightNumberPicker.ValueChange += HeightNumberPicker_ValueChange;
            }
            else Width = Value;
        }

        private void HeightNumberPicker_ValueChange(object sender, int Value)
        {
            if (isGeometric == true) 
            {
                Width  = Value*App.Model.Width / App.Model.Height;
                Height = Value;

                WidthNumberPicker.ValueChange -= WidthNumberPicker_ValueChange;
                WidthNumberPicker.Value = Width;
                WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;
            }
            else   Height = Value;
        }


        private void StretchContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            WidthNumberPicker.ValueChange -= WidthNumberPicker_ValueChange;
            HeightNumberPicker.ValueChange -= HeightNumberPicker_ValueChange;

            float xs =1f / App.Model.Width* Width;
            float ys =1f /  App.Model.Height*  Height;
            Matrix3x2 Matrix = Matrix3x2.CreateScale( xs,ys);

            //跟随宽高 
            App.Model.Width = Width;
            App.Model.Height = Height;
            App.Model.CanvasWidth = Width;
            App.Model.CanvasHeight = Height;

            foreach (Layer L in App.Model.Layers)
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(new Transform2DEffect { Source = L.CanvasRenderTarget, TransformMatrix = Matrix });
                }
                L.CanvasRenderTarget = crt;
                if (App.Model.isLowView) L.LowView();
                else L.SquareView();
                L.SetWriteableBitmap(App.Model.VirtualControl);
            }
 
            //初始化
            App.Initialize(App.Model.VirtualControl,  Width,  Height);
            App.Model.MaskAnimatedTarget = new CanvasRenderTarget(App.Model.AnimatedControl, Width, Height);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 

            //Undo：撤销
            App.Model.Undos.Clear();//清空
            App.Model.UndoIndex = 0;
            App.Model.isUndo = false;
            App.Model.isRedo = false;
        }


        #endregion


        #region Rotate：旋转 


        //Rotate：旋转 
        private void OtherRotateButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Collapsed;
            RotateGrid.Visibility = Visibility.Visible;
        }
        private void RotateButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Visible;
            RotateGrid.Visibility = Visibility.Collapsed;
        }

        //FlipHorizontal：水平镜像
        private void FlipHorizontalButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Vector2 Center = new Vector2(App.Model.Width / 2, App.Model.Height / 2);
            Matrix3x2 Matrix = Matrix3x2.CreateScale(-1f,1f,Center);

            foreach (Layer L in App.Model.Layers)
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(new Transform2DEffect { Source = L.CanvasRenderTarget, TransformMatrix = Matrix });
                }
                L.CanvasRenderTarget = crt;
                L.SetWriteableBitmap(App.Model.VirtualControl);
            }
            Grid.Visibility = Visibility.Visible;
            RotateGrid.Visibility = Visibility.Collapsed;
            flyout.Hide();

            //改变选区与套索
            App.MaskClear();

            //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = false;
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 

            //Undo：撤销
            App.Model.Undos.Clear();//清空
            App.Model.UndoIndex = 0;
            App.Model.isUndo = false;
            App.Model.isRedo = false;
        }

        //FlipVertical：垂直镜像
        private void FlipVerticalButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Vector2 Center = new Vector2(App.Model.Width / 2, App.Model.Height / 2);
            Matrix3x2 Matrix = Matrix3x2.CreateScale(1f, -1f, Center);

            foreach (Layer L in App.Model.Layers)
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(new Transform2DEffect { Source = L.CanvasRenderTarget, TransformMatrix = Matrix });
                }
                L.CanvasRenderTarget = crt;
                L.SetWriteableBitmap(App.Model.VirtualControl);
            }
            Grid.Visibility = Visibility.Visible;
            RotateGrid.Visibility = Visibility.Collapsed;
            flyout.Hide();

            //改变选区与套索
            App.MaskClear();

            //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = false;
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 

            //Undo：撤销
            App.Model.Undos.Clear();//清空
            App.Model.UndoIndex = 0;
            App.Model.isUndo = false;
            App.Model.isRedo = false;
        }

        //LeftTurn：左旋转
        private void LeftTurnButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
             float Rotate = (float)-Math.PI / 2;
            Matrix3x2 Matrix = Matrix3x2.CreateRotation(Rotate) * Matrix3x2.CreateTranslation(0, App.Model.Width);

            foreach (Layer L in App.Model.Layers)
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Height, App.Model.Width);
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(new Transform2DEffect { Source = L.CanvasRenderTarget, TransformMatrix = Matrix });
                }
                L.CanvasRenderTarget = crt;
                L.SetWriteableBitmap(App.Model.VirtualControl);
            }

            //交换国土
            if (App.Model.Width != App.Model.Height)
            {
                var W = App.Model.Width;
                var H = App.Model.Height;
                App.Model.X += (App.Model.Width - W) / 2 * App.Model.XS;
                App.Model.Y += (App.Model.Height - H) / 2 * App.Model.YS;
                App.Model.CanvasWidth = H * App.Model.XS;
                App.Model.CanvasHeight = W * App.Model.YS;
                App.Model.Width = H;
                App.Model.Height = W;

                //初始化
                App.Initialize(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
                App.Model.MaskAnimatedTarget = new CanvasRenderTarget(App.Model.AnimatedControl, App.Model.Width, App.Model.Height);
            }
            Grid.Visibility = Visibility.Visible;
            RotateGrid.Visibility = Visibility.Collapsed;
            flyout.Hide();

            //改变选区与套索
            App.MaskClear();

            //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = false;
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 

            //Undo：撤销
            App.Model.Undos.Clear();//清空
            App.Model.UndoIndex = 0;
            App.Model.isUndo = false;
            App.Model.isRedo = false;
        }

        //RightTurn：右旋转
        private void RightTurnButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            float Rotate = (float)Math.PI / 2;
            Matrix3x2 Matrix = Matrix3x2.CreateRotation(Rotate) * Matrix3x2.CreateTranslation(App.Model.Height,0);

            foreach (Layer L in App.Model.Layers)
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Height, App.Model.Width);
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(new Transform2DEffect { Source = L.CanvasRenderTarget, TransformMatrix = Matrix });
                }
                L.CanvasRenderTarget = crt;
                L.SetWriteableBitmap(App.Model.VirtualControl);
            }

            //交换国土
            if (App.Model.Width != App.Model.Height)
            {
                var W = App.Model.Width;
                var H = App.Model.Height;
                App.Model.X += (App.Model.Width - W) / 2 * App.Model.XS;
                App.Model.Y += (App.Model.Height - H) / 2 * App.Model.YS;
                App.Model.CanvasWidth = H * App.Model.XS;
                App.Model.CanvasHeight = W * App.Model.YS;
                App.Model.Width = H;
                App.Model.Height = W;

                //初始化
                App.Initialize(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
                App.Model.MaskAnimatedTarget = new CanvasRenderTarget(App.Model.AnimatedControl, App.Model.Width, App.Model.Height);
            }
            Grid.Visibility = Visibility.Visible;
            RotateGrid.Visibility = Visibility.Collapsed;
            flyout.Hide();

            //改变选区与套索
            App.MaskClear();

            //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = false;
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 

            //Undo：撤销
            App.Model.Undos.Clear();//清空
            App.Model.UndoIndex = 0;
            App.Model.isUndo = false;
            App.Model.isRedo = false;
        }

        //OverTurn：翻转
        private void OverTurnButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Vector2 Center = new Vector2(App.Model.Width / 2, App.Model.Height / 2);
            Matrix3x2 Matrix = Matrix3x2.CreateScale(-1f, -1f,Center);

            foreach (Layer L in App.Model.Layers)
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(new Transform2DEffect{Source = L.CanvasRenderTarget,TransformMatrix = Matrix});
                }
                L.CanvasRenderTarget = crt;
                L.SetWriteableBitmap(App.Model.VirtualControl);
            }
            Grid.Visibility = Visibility.Visible;
            RotateGrid.Visibility = Visibility.Collapsed;
            flyout.Hide();

            //改变选区与套索
            App.MaskClear();

            //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = false;
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新 

            //Undo：撤销
            App.Model.Undos.Clear();//清空
            App.Model.UndoIndex = 0;
            App.Model.isUndo = false;
            App.Model.isRedo = false;
        }


        #endregion


        #region Other：杂项 


        //Crop：裁切
        private void OtherCropButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeCrop();
            ToOther(300,true);
        }

        //Gradient：渐变
        private void OtherGradientButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeOther();
            ToOther(303);
        }

        //Fade：渐隐
        private void OtherFadeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeEffect();
            ToOther(304);
        }

        //Text：文字
        private void OtherTextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeOther();
            ToOther(305,true);
        }

        //Grids：网格线
        private void GridsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeOther();
            ToOther(306, true);
        }

        //Fill：填充
        private void OtherFillButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeOther();
            ToOther(307);
        }

         //Transform：变换
        private void OtherTransformButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
             App.InitializeEffect();
            ToOther(308);
        }

        //Transform3D：变换3D
         private void OtherTransform3DButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeEffect();
            ToOther(309);
        }








        #endregion
         
    }
}
