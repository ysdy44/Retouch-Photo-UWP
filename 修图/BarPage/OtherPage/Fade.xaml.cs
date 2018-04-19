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
using 修图.Model;
using 修图.Library;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;

namespace 修图.BarPage.OtherPage
{ 
    public sealed partial class Fade : Page
    {
        public Fade()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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

        #region Fade：左


        private void EdgeBehavior_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == EdgeBehavior0)
            {
                EdgeBehavior0.IsChecked = false;
                App.Setting.FadeEdgeBehavior = CanvasEdgeBehavior.Clamp;
            }
            else EdgeBehavior0.IsChecked = true;

            // 1
            if (tb == EdgeBehavior1)
            {
                EdgeBehavior1.IsChecked = false;
                App.Setting.FadeEdgeBehavior = CanvasEdgeBehavior.Wrap;
            }
            else EdgeBehavior1.IsChecked = true;

            // 2
            if (tb == EdgeBehavior2)
            {
                EdgeBehavior2.IsChecked = false;
                App.Setting.FadeEdgeBehavior = CanvasEdgeBehavior.Mirror;
            }
            else EdgeBehavior2.IsChecked = true;

            Render();
        }

        #endregion

        #region Fade：右


        private void LinearButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Setting.isFadeRadial = false;
            rightFlyout.Hide();

            Render();
        }

        private void RadialButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.Setting.isFadeRadial = true;
            rightFlyout.Hide();

            Render();
        }

        private void ReverseButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
             for (int i = 0; i < App.Setting.FadeStops.Count; i++)
            {
                App.Setting.FadeStops[i] = new CanvasGradientStop
                {
                    Position =1 - App.Setting.FadeStops[i].Position,
                    Color = App.Setting.FadeStops[i].Color
                };
            }

            rightFlyout.Hide();
            canvasControl.Invalidate();

            Render();
        }


        #endregion

        #region Fade：渐变


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
            CanvasLinearGradientBrush Linear = new CanvasLinearGradientBrush(App.Model.VirtualControl, App.Setting.FadeStops.ToArray());
            Linear.StartPoint = new Vector2(0, CanvasHalfHeight);
            Linear.EndPoint = new Vector2(CanvasWidth, CanvasHalfHeight);
            args.DrawingSession.FillRectangle(0, 0, CanvasWidth + Space + Space, CanvasHeight, Linear);

            //点
            for (int i = 0; i < App.Setting.FadeStops.Count; i++)
            {
                Vector2 v = new Vector2(App.Setting.FadeStops[i].Position * CanvasWidth + Space, CanvasHalfHeight);

                //主点
                args.DrawingSession.FillEllipse(v, 10, 10, Colors.Black);//环线
                if (i == App.Setting.FadeCurrent) args.DrawingSession.FillEllipse(v, 9, 9, Colors.White);
                else
                {
                    args.DrawingSession.FillEllipse(v, 9, 9, Colors.DarkGray);
                    args.DrawingSession.FillEllipse(v, 6, 6, Colors.White);
                }
                args.DrawingSession.FillEllipse(v, 6, 6, App.Setting.FadeStops[i].Color);//主题色
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

            App.Setting.FadeCurrent = -1;
            for (int i = 0; i < App.Setting.FadeStops.Count; i++)
            {
                if (Math.Abs((e.HorizontalOffset - Space) - App.Setting.FadeStops[i].Position * CanvasWidth) < Space)
                {
                    App.Setting.FadeCurrent = i;
                    break;
                }
            }

            //如果选中，就添加个
            if (App.Setting.FadeCurrent == -1)
            {
                CanvasGradientStop stop = new CanvasGradientStop
                {
                    Position = (float)(e.HorizontalOffset / CanvasWidth),
                    Color = Color.FromArgb(128, 0, 141, 255)
                };
                App.Setting.FadeStops.Add(stop);

                App.Setting.FadeCurrent = App.Setting.FadeStops.IndexOf(stop);
            }
            canvasControl.Invalidate();
            Render();
        }

        private void thumeGrid_DragDelta(object sender, DragDeltaEventArgs e)
        {
            HorizontalChange += e.HorizontalChange;
            VerticalChange += e.VerticalChange;

            if (App.Setting.FadeCurrent >= 0 && App.Setting.FadeCurrent < App.Setting.FadeStops.Count)
            {
                //移动点
                if ((HorizontalChange - Space) > 0 && (HorizontalChange - Space) < CanvasWidth)
                    App.Setting.FadeSetCurrent((float)((HorizontalChange - Space) / CanvasWidth));//设置位置

                //超过高度删除
                if (App.Setting.FadeStops.Count > 2)
                {
                    if (VerticalChange < -90)
                    {
                        App.Setting.FadeStops.RemoveAt(App.Setting.FadeCurrent);
                        App.Setting.FadeCurrent = -1;
                    }
                }

                canvasControl.Invalidate();
                Render();
            }
        }
        private void thumeGrid_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (App.Setting.FadeCurrent >= 0 && App.Setting.FadeCurrent < App.Setting.FadeStops.Count)
            {
                if (Math.Abs(HorizontalChange - HorizontalOffset) < Space)
                {
                    if (Math.Abs(VerticalChange - VerticalOffset) < Space)
                    {
                        OpacityNumberPicker.ValueChange -= OpacityNumberPicker_ValueChange;
                        OpacitySlider.ValueChanged -= OpacitySlider_ValueChanged;
                        OpacitySlider.Value = OpacityNumberPicker.Value = App.Setting.FadeGetCurrent;
                        OpacityNumberPicker.ValueChange+= OpacityNumberPicker_ValueChange;
                        OpacitySlider.ValueChanged += OpacitySlider_ValueChanged;

                        Flyout.ShowAt(thumeGrid);
                    }
                }
            }
        }



        #endregion

        #region Picker：数字选择器


        private void OpacityNumberPicker_ValueChange(object sender, int Value)
        {
            OpacitySlider.Value = Value;
            App.Setting.FadeSetCurrent(Value);

            canvasControl.Invalidate();
              Render();
        }

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            OpacityNumberPicker.Value = (int)e.NewValue;
            App.Setting.FadeSetCurrent((int)e.NewValue);

            canvasControl.Invalidate();
            Render();
        }


        #endregion


        public static void Render()
        {
             CanvasCommandList alphaMask = new CanvasCommandList(App.Model.VirtualControl);
            using (CanvasDrawingSession ds = alphaMask.CreateDrawingSession())
            {
                ds.FillRectangle(0, 0, App.Model.Width, App.Model.Height, App.Setting.FadeBrush);
              }

            App.Model.SecondCanvasImage = new AlphaMaskEffect
            {
                //两个IGraphicsEffectSource，前者为底图，后者为蒙版
                Source = App.Model.SecondSourceRenderTarget,
                AlphaMask = alphaMask,
            };


            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }


        public static void Apply()
        {
            if (App.Model.SecondCanvasImage != null)
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                {
                    //如果有选区：扣掉选区
                    if (App.Model.isAnimated == true)
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MainRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);
                    //如果没有选区：全部擦除掉
                    else ds.Clear(Colors.Transparent);

                    ds.DrawImage(App.Model.SecondCanvasImage);
                }
                App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
            }
        }

     
    }
}
