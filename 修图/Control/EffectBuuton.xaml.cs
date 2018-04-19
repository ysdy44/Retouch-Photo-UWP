
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;
using 修图.Model;


namespace 修图.Control
{
    public sealed partial class EffectBuuton : UserControl
    {
        //Delegate
        public delegate void EffectHandler(int Index);
        public event EffectHandler Effect;
         
        public EffectBuuton()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        #region Global：全局


        //浮出：开启后渲染【特效源图渲染目标】和【特效上渲染目标】【特效下渲染目标】
        private void flyout_Opened(object sender, object e)
        {
            App.Judge();//判断选区，改变是否动画与选区矩形


            //初始化上中下渲染目标
            App.InitializeEffect();
        }

        private void ToEffect(int Index)
        {
            flyout.Hide();
             
            //如果图层不可视或透明
            if (App.Model.CurrentVisual==false) App.Tip(App.resourceLoader.GetString("/Layer/Hided_"));
            else Effect(Index);
        }







        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 0
            if (Pivot.SelectedIndex == 0) EffectButton0.IsChecked = true;
            else EffectButton0.IsChecked = false;
            // 1
            if (Pivot.SelectedIndex == 1) EffectButton1.IsChecked = true;
            else EffectButton1.IsChecked = false;
            // 2
            if (Pivot.SelectedIndex == 2) EffectButton2.IsChecked = true;
            else EffectButton2.IsChecked = false;
            // 3
            if (Pivot.SelectedIndex == 3) EffectButton3.IsChecked = true;
            else EffectButton3.IsChecked = false;
        }
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             // 0
            if (Pivot.SelectedIndex == 0) EffectButton0.IsChecked = true;
            else EffectButton0.IsChecked = false;
            // 1
            if (Pivot.SelectedIndex == 1) EffectButton1.IsChecked = true;
            else EffectButton1.IsChecked = false;
            // 2
            if (Pivot.SelectedIndex == 2) EffectButton2.IsChecked = true;
            else EffectButton2.IsChecked = false;
            // 3
            if (Pivot.SelectedIndex == 3) EffectButton3.IsChecked = true;
            else EffectButton3.IsChecked = false;
        }
        private void EffectButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;
            
            // 0
            if (tb == EffectButton0)
            {
                    EffectButton0.IsChecked = true;
                Pivot.SelectedIndex = 0;
            }
            else  EffectButton0.IsChecked = false;
 

            // 1
            if (tb == EffectButton1) 
            {
                 EffectButton1.IsChecked = true;
                Pivot.SelectedIndex = 1;
            }
            else  
            {
                  EffectButton1.IsChecked = false;
            }


            // 2
            if (tb == EffectButton2)
            {
                 EffectButton2.IsChecked = true;
                Pivot.SelectedIndex = 2;
            }
            else
            {
                 EffectButton2.IsChecked = false;
            }

            // 3
            if (tb == EffectButton3)
            {
                 EffectButton3.IsChecked = true;
                Pivot.SelectedIndex = 3;
            }
            else
            {
                 EffectButton3.IsChecked = false;
            }
         }


        #endregion


        #region Effect：特效


        //Gray：黑白
        private  void GrayButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            flyout.Hide();

             Layer l = App.Model.Layers[App.Model.Index];

            //如果图层不可视或透明
            if (l.Visual == false || l.Opacity <= 0) App.Tip(App.resourceLoader.GetString("/Layer/Hided_"));
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                using (CanvasDrawingSession ds = l.CanvasRenderTarget.CreateDrawingSession())
                {
                    if (App.Model.isAnimated == false) //清空：如果无选区
                        ds.Clear(Colors.Transparent);
                    else if (App.Model.isAnimated == true)  //选区内清空：如果有选区
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);

                    ds.DrawImage(Adjust.GetGrayscale(App.Model.SecondSourceRenderTarget)); //绘画：特效渲染目标
                }

                l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }

        //Invert：反色
        private  void InvertButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            flyout.Hide();
            Layer l = App.Model.Layers[App.Model.Index];

            //如果图层不可视或透明
            if (l.Visual == false || l.Opacity <= 0) App.Tip(App.resourceLoader.GetString("/Layer/Hided_"));
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                using (CanvasDrawingSession ds = l.CanvasRenderTarget.CreateDrawingSession())
                {
                    if (App.Model.isAnimated == false) //清空：如果无选区
                        ds.Clear(Colors.Transparent);
                    else if (App.Model.isAnimated == true)  //选区内清空：如果有选区
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);

                    ds.DrawImage(Adjust.GetInvert(App.Model.SecondSourceRenderTarget)); //绘画：特效渲染目标
                }

                l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }

        //Exposure：曝光
        private void ExposureButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(200); }
        
        //Brightness：亮度
        private void BrightnessButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(201); }

       //Saturation：饱和度
        private void SaturationButton_Tapped(object sender, TappedRoutedEventArgs e)  { ToEffect(202);  }

        //HueRotation：色相
        private void HueRotationButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(203);  }

        //Contrast：对比度
        private void ContrastButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(204);  }

        //Temperature：对冷暖比度
        private void TemperatureButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(205);}

        //HighlightsAndShadows：高光/阴影
        private void HighlightsAndShadowsButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(206);}


        #endregion


        #region Effect1：特效1


        //GaussianBlur：高斯模糊
        private void GaussianBlurButton_Tapped(object sender, TappedRoutedEventArgs e)  {  ToEffect(210);   }

        //DirectionalBlur：定向模糊
        private void DirectionalBlurButton_Tapped(object sender, TappedRoutedEventArgs e)  {  ToEffect(211);  }

        //Sharpen：锐化
        private void SharpenButton_Tapped(object sender, TappedRoutedEventArgs e) {  ToEffect(212); }

        //Shadow：阴影
        private void ShadowButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(213); }

        //ChromaKey：色度键
        private void ChromaKeyButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(214); }

        //EdgeDetection：边缘检测
        private void EdgeDetectionButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(215); }

        //Border：边界
        private void BorderButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(216); }

        //Emboss：浮雕
        private void EmbossButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(217); }

        //Lighting：暗室灯光
        private void LightingButton_Tapped(object sender, TappedRoutedEventArgs e) {ToEffect(218); } 


        #endregion


        #region Effect2：特效2


        //LuminanceToAlpha：亮度转透明度
        private void LuminanceToAlphaButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            flyout.Hide();

            Layer l = App.Model.Layers[App.Model.Index];

            //如果图层不可视或透明
            if (l.Visual == false || l.Opacity <= 0) App.Tip(App.resourceLoader.GetString("/Layer/Hided_"));
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                using (CanvasDrawingSession ds = l.CanvasRenderTarget.CreateDrawingSession())
                {
                   ds.DrawImage(Adjust.GetLuminanceToAlpha(App.Model.SecondSourceRenderTarget), 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);//绘画：特效渲染目标
                }

                l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }

        //Fog：迷雾
        private void FogButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            flyout.Hide();

            Layer l = App.Model.Layers[App.Model.Index];

            //如果图层不可视或透明
            if (l.Visual == false || l.Opacity <= 0) App.Tip(App.resourceLoader.GetString("/Layer/Hided_"));
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                ICanvasImage ic = new TurbulenceEffect
                {
                    Octaves = 4,
                    Size = new Vector2(App.Model.Width, App.Model.Height)
                };
                using (CanvasDrawingSession ds = l.CanvasRenderTarget.CreateDrawingSession())
                {
                    ds.DrawImage(ic, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);//绘画：特效渲染目标
                }

                l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }

        //Sepia：深褐色
        private void SepiaButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            flyout.Hide();

            Layer l = App.Model.Layers[App.Model.Index];

            //如果图层不可视或透明
            if (l.Visual == false || l.Opacity <= 0) App.Tip(App.resourceLoader.GetString("/Layer/Hided_"));
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                using (CanvasDrawingSession ds = l.CanvasRenderTarget.CreateDrawingSession())
                {
                    if (App.Model.isAnimated == false) //清空：如果无选区
                        ds.Clear(Colors.Transparent);
                    else if (App.Model.isAnimated == true)  //选区内清空：如果有选区
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);

                     ds.DrawImage(Adjust.GetSepia(App.Model.SecondSourceRenderTarget));
                }

                l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }

        //Posterize：多色调分印
        private void PosterizeButton_Tapped(object sender, TappedRoutedEventArgs e) 
        {
            flyout.Hide();

            Layer l = App.Model.Layers[App.Model.Index];

            //如果图层不可视或透明
            if (l.Visual == false || l.Opacity <= 0) App.Tip(App.resourceLoader.GetString("/Layer/Hided_"));
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                using (CanvasDrawingSession ds = l.CanvasRenderTarget.CreateDrawingSession())
                {
                    if (App.Model.isAnimated == false) //清空：如果无选区
                        ds.Clear(Colors.Transparent);
                    else if (App.Model.isAnimated == true)  //选区内清空：如果有选区
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);

                    ds.DrawImage(Adjust.GetPosterize(App.Model.SecondSourceRenderTarget));
                }

                l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }



        //Colouring：上色
        private void ColouringButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(220); }

        private void TintButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(221); }

        //DiscreteTransfer：离散转让
        private void DiscreteTransferButton_Tapped(object sender, TappedRoutedEventArgs e){ ToEffect(222); }

        //Vignette：装饰图案
        private void VignetteButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(223); }

        //GammaTransfer：伽马转染
        private void GammaTransferButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(224); }


        #endregion


        #region Effect3：特效3


        //Glass：玻璃
        private void GlassButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(230); }


        //PinchPunch：膨胀收缩
        private void PinchPunchButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(231); }

        //Morphology：形态学
        private void MorphologyButton_Tapped(object sender, TappedRoutedEventArgs e) { ToEffect(232); } 






        #endregion

    }
}
