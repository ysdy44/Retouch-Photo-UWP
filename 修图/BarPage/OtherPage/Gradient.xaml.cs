using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.UI.Xaml;
using 修图.Model;
using System;
using System.Collections.Generic;

namespace 修图.BarPage.OtherPage
{ 
    public sealed partial class Gradient : Page
    {
 
        public Gradient()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
             
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.Judge();//判断选区，改变是否动画与选区矩形


            //控件事件   
            // 0
            if (App.Setting.GradientEdgeBehavior == CanvasEdgeBehavior.Clamp) EdgeBehavior0.IsChecked = false;
            else EdgeBehavior0.IsChecked = true;
            // 1
            if (App.Setting.GradientEdgeBehavior == CanvasEdgeBehavior.Wrap) EdgeBehavior1.IsChecked = false;
            else EdgeBehavior1.IsChecked = true;
            // 2      
            if (App.Setting.GradientEdgeBehavior == CanvasEdgeBehavior.Mirror) EdgeBehavior2.IsChecked = false;
            else EdgeBehavior2.IsChecked = true;

            Render();
        }


        #endregion

        #region Gradient：左
        

        private void EdgeBehavior_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;
            
            // 0
            if (tb == EdgeBehavior0)
            {
                EdgeBehavior0.IsChecked = false;
                App.Setting.GradientEdgeBehavior = CanvasEdgeBehavior.Clamp;
            }
            else EdgeBehavior0.IsChecked = true;

            // 1
            if (tb == EdgeBehavior1)
            {
                EdgeBehavior1.IsChecked = false;
                App.Setting.GradientEdgeBehavior = CanvasEdgeBehavior.Wrap;
            }
            else EdgeBehavior1.IsChecked = true;

            // 2
            if (tb == EdgeBehavior2)
            {
                EdgeBehavior2.IsChecked = false;
                App.Setting.GradientEdgeBehavior = CanvasEdgeBehavior.Mirror;
            }
            else EdgeBehavior2.IsChecked = true;

            Render();
        }

        #endregion

        #region Gradient：右


        private void LinearButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
             App.Setting.isGradientRadial = false;
            rightFlyout.Hide();

            Render();
        }

        private void RadialButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Setting.isGradientRadial = true;
            rightFlyout.Hide();

            Render();
        }

        private void ReverseButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        { 
            for (int i = 0; i < App.Setting.GradientStops.Count; i++)
            {
                App.Setting.GradientStops[i] = new CanvasGradientStop
                {
                    Position =1- App.Setting.GradientStops[i].Position,
                    Color = App.Setting.GradientStops[i].Color
                };
            }

             rightFlyout.Hide();
            canvasControl.Invalidate();

            Render();
        }


        #endregion

        #region Gradient：渐变


        float CanvasWidth;//宽度
        float CanvasHeight;//高度
        float CanvasHalfHeight;//一半高度
        float Space = 8;//左右间距

        private void canvasControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasWidth = (float)e.NewSize.Width - Space - Space;
            CanvasHeight = (float)e.NewSize.Height;
            CanvasHalfHeight = CanvasHeight / 2;
        }

        private void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
             //图片
            args.DrawingSession.DrawImage(App.GrayWhiteGrid);

            //渐变
            CanvasLinearGradientBrush Linear = new CanvasLinearGradientBrush(App.Model.VirtualControl, App.Setting.GradientStops.ToArray());
            Linear.StartPoint = new Vector2(0, CanvasHalfHeight);
            Linear.EndPoint = new Vector2(CanvasWidth, CanvasHalfHeight);
            args.DrawingSession.FillRectangle(0, 0, CanvasWidth +Space + Space, CanvasHeight, Linear);

            //点
            for (int i = 0; i < App.Setting.GradientStops.Count; i++)
            {
                 Vector2 v = new Vector2(App.Setting.GradientStops[i].Position * CanvasWidth + Space, CanvasHalfHeight);
             
                //主点
                args.DrawingSession.FillEllipse(v, 10, 10, Colors.Black);//环线
                if (i == App.Setting.GradientCurrent) args.DrawingSession.FillEllipse(v, 9, 9, Colors.White);
                else
                {
                    args.DrawingSession.FillEllipse(v, 9, 9, Colors.DarkGray);
                    args.DrawingSession.FillEllipse(v, 6, 6, Colors.White);
                }
                args.DrawingSession.FillEllipse(v, 6, 6, App.Setting.GradientStops[i].Color);//主题色
            }
        }


        #endregion


        #region Canvas：演示画布


 
        //初始位置
        double HorizontalOffset;
        double VerticalOffset;
        //变化位置
        double HorizontalChange;
        double VerticalChange;

        private void thumeGrid_DragStarted(object sender, DragStartedEventArgs e)
        {
            HorizontalOffset = HorizontalChange = e.HorizontalOffset;//初始位置
            VerticalOffset = VerticalChange = e.VerticalOffset;//初始位置

            App.Setting.GradientCurrent = -1;
            for (int i = 0; i < App.Setting.GradientStops.Count; i++)
            {
                if (Math.Abs((e.HorizontalOffset - Space) - App.Setting.GradientStops[i].Position * CanvasWidth) < Space)
                {
                    App.Setting.GradientCurrent = i;
                    break;
                }
            }

            //如果选中，就添加个
            if (App.Setting.GradientCurrent == -1)
            {
                CanvasGradientStop stop = new CanvasGradientStop
                {
                    Position = (float)(e.HorizontalOffset / CanvasWidth),
                    Color = Color.FromArgb(128, 0, 0, 0)
                };
                App.Setting.GradientStops.Add(stop);

                App.Setting.GradientCurrent = App.Setting.GradientStops.IndexOf(stop);
            }
            canvasControl.Invalidate();
            Render();
        }

        private void thumeGrid_DragDelta(object sender, DragDeltaEventArgs e)
        {
            HorizontalChange += e.HorizontalChange;
            VerticalChange += e.VerticalChange;

            if (App.Setting.GradientCurrent >= 0 && App.Setting.GradientCurrent < App.Setting.GradientStops.Count)
            {
                //移动点
                if ((HorizontalChange - Space) > 0 && (HorizontalChange - Space) < CanvasWidth)
                     App.Setting.GradientSetCurrent((float)((HorizontalChange - Space) / CanvasWidth));//设置位置
 
                //超过高度删除
                if (App.Setting.GradientStops.Count > 2)
                {
                    if (VerticalChange < -90)
                    {
                        App.Setting.GradientStops.RemoveAt(App.Setting.GradientCurrent);
                        App.Setting.GradientCurrent = -1;
                    }
                }

                canvasControl.Invalidate();
                Render();
            }
        }
        private void thumeGrid_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (App.Setting.GradientCurrent >= 0 && App.Setting.GradientCurrent < App.Setting.GradientStops.Count)
            {
                if (Math.Abs(HorizontalChange - HorizontalOffset) < Space)
                {
                    if (Math.Abs(VerticalChange - VerticalOffset) < Space)
                    {
                        colorPicker.ColorChanged -= colorPicker_ColorChanged;
                        colorPicker.Color = App.Setting.GradientGetCurrent;
                        colorPicker.ColorChanged += colorPicker_ColorChanged;

                        Flyout.ShowAt(thumeGrid);
                    }
                }
            }
        }



        #endregion

        #region Picker：颜色选择器


        private void colorPicker_ColorChanged(Color Color, SolidColorBrush Brush)
        {
            if (App.Setting.GradientCurrent >= 0 && App.Setting.GradientCurrent < App.Setting.GradientStops.Count)
                App.Setting.GradientSetCurrent(Color);//设置颜色

            canvasControl.Invalidate();
            Render();
         }

        private void colorPicker_StrawChanged()
        {
            Flyout.Hide();
            App.Model.StrawVisibility = Visibility.Collapsed;
        }


        #endregion


        public static void Render()
        {
            using (CanvasDrawingSession ds = App.Model.SecondSourceRenderTarget.CreateDrawingSession())//新图片上画几何
            {
                ds.Clear(Colors.Transparent);
                ds.FillRectangle(0, 0, App.Model.Width, App.Model.Height, App.Setting.GradientBrush);

                if (App.Model.isAnimated == true)
                    ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationIn);
            }

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        public static void Apply()
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);

            Layer l = App.Model.Layers[App.Model.Index];
            using (CanvasDrawingSession ds = l.CanvasRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(App.Model.SecondSourceRenderTarget);
            }
            l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图}
        }
    }
}
