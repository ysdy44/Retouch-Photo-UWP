using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.ApplicationModel.DataTransfer;

namespace 修图.Control
{
    public sealed partial class AdjustControl : UserControl
    {
        //渐变笔刷
        CanvasLinearGradientBrush redBrush;
        CanvasLinearGradientBrush greenBrush;
        CanvasLinearGradientBrush blueBrush;

        bool isRender;//是否渲染：用来判断是控件自己的刷新画布还是依赖属性绑定的刷新

        #region DependencyProperty：依赖属性


        //刷新
        public int Refresh
        {
            get { return (int)GetValue(RefreshProperty); }
            set { SetValue(RefreshProperty, value); }
        }

        public static readonly DependencyProperty RefreshProperty =
            DependencyProperty.Register("Refresh", typeof(int), typeof(AdjustControl), new PropertyMetadata(0, new PropertyChangedCallback(RefreshOnChang)));

        private static void RefreshOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AdjustControl Con = (AdjustControl)sender;

            Con.isRender = true;

            Con.CanvasControl.Invalidate();//刷新画布内容 
            Con.Follow();
            Con.isRender = false;
            Con.Texted();
        }



        #endregion

        public AdjustControl()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }

        #region Global：全局


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Follow();//滑条跟随数值

             //Light：光线 
            BrightnessSlider.ValueChanged += Slider_ValueChanged;
            ExposureSlider.ValueChanged += Slider_ValueChanged;
            ContrastSlider.ValueChanged += Slider_ValueChanged;
            HighlightsSlider.ValueChanged += Slider_ValueChanged;
            ShadowsSlider.ValueChanged += Slider_ValueChanged;
            //Color：颜色
            SaturationSlider.ValueChanged += Slider_ValueChanged;
            HueRotationSlider.ValueChanged += Slider_ValueChanged;
            TemperatureSlider.ValueChanged += Slider_ValueChanged;
            VignetteSlider.ValueChanged += Slider_ValueChanged;
            //Gamma：伽马
            AlpheSlider.ValueChanged += Slider_ValueChanged;
            RedSlider.ValueChanged += Slider_ValueChanged;
            GreenSlider.ValueChanged += Slider_ValueChanged;
            BlueSlider.ValueChanged += Slider_ValueChanged;
        }


 

        #endregion



        #region Adjust：调整


        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isRender == false)
            {
                Changed();//改变参数
                Render();//渲染目标
                Texted();
            }
         }



        //Histogram：直方图
        private void HistogramButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (HistogramBorder.Height < 90) HistogramShow.Begin();
            else HistogramFade.Begin();
        }
         //Light：光线 
        private void LightButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (LightPanel.Opacity == 0) LightShow.Begin();
            else LightFade.Begin();
        }
        //Color：颜色
        private void ColorButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ColorPanel.Opacity == 0) ColorShow.Begin();
            else ColorFade.Begin();
        }
        //Gamma：伽马
        private void GammaButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (GammaPanel.Opacity == 0) GammaShow.Begin();
            else GammaFade.Begin();
        }


        #endregion

        #region Adjust：算法

 
        //输入参数，跟随滑条
        private void Follow()
        {            
            //Light：光线 
            BrightnessSlider.Value = App.Setting.AdjustArray[0] * 100f; //亮度
            ExposureSlider.Value = App.Setting.AdjustArray[1] * 100;//曝光
            ContrastSlider.Value = App.Setting.AdjustArray[2] * 200;//对比度
            HighlightsSlider.Value = App.Setting.AdjustArray[3] * 200;//高光
            ShadowsSlider.Value = App.Setting.AdjustArray[4] * 200;//阴影

            //Color：颜色
            SaturationSlider.Value = App.Setting.AdjustArray[5] * 100;//饱和度
            HueRotationSlider.Value = App.Setting.AdjustArray[6] * 100;//色相
            TemperatureSlider.Value = App.Setting.AdjustArray[7] * 200;//冷暖
            VignetteSlider.Value = App.Setting.AdjustArray[8] * 200; //装饰图案

            //Gamma：伽马
            AlpheSlider.Value = App.Setting.AdjustArray[9] * 100;
            RedSlider.Value = App.Setting.AdjustArray[10] * 100;
            GreenSlider.Value = App.Setting.AdjustArray[11] * 100;
            BlueSlider.Value = App.Setting.AdjustArray[12] * 100;

        }

        //根据滑条，改变参数
        private void Changed()
        {
            //Light：光线 
            App.Setting.AdjustArray[0] = (float)BrightnessSlider.Value / 100f; //亮度
            App.Setting.AdjustArray[1] = (float)ExposureSlider.Value / 100;//曝光
            App.Setting.AdjustArray[2] = (float)ContrastSlider.Value / 200;//对比度
            App.Setting.AdjustArray[3] = (float)HighlightsSlider.Value / 200;//高光
            App.Setting.AdjustArray[4] = (float)ShadowsSlider.Value / 200;//阴影

            //Color：颜色
            App.Setting.AdjustArray[5] = (float)SaturationSlider.Value / 100;//饱和度
            App.Setting.AdjustArray[6] = (float)HueRotationSlider.Value / 100;//色相
            App.Setting.AdjustArray[7] = (float)TemperatureSlider.Value / 200;//冷暖
            App.Setting.AdjustArray[8] = (float)VignetteSlider.Value / 200; //装饰图案

            //Gamma：伽马
            App.Setting.AdjustArray[9] = (float)AlpheSlider.Value / 100;
            App.Setting.AdjustArray[10] = (float)RedSlider.Value / 100;
            App.Setting.AdjustArray[11] = (float)GreenSlider.Value / 100;
            App.Setting.AdjustArray[12] = (float)BlueSlider.Value / 100;
        }


    


        private void Texted()
        {
            Text.Text =
                "Light：" +
                App.Setting.AdjustArray[0].ToString() + " " +
                App.Setting.AdjustArray[1].ToString() + " " +
                App.Setting.AdjustArray[2].ToString() + " " +
                App.Setting.AdjustArray[3].ToString() + " " +
                App.Setting.AdjustArray[4].ToString() + " " +

                "  Color：" +
                App.Setting.AdjustArray[5].ToString() + " " +
                App.Setting.AdjustArray[6].ToString() + " " +
                App.Setting.AdjustArray[7].ToString() + " " +
                App.Setting.AdjustArray[8].ToString() + " " +

                "  Gammar：" +
                App.Setting.AdjustArray[9].ToString() + " " +
                App.Setting.AdjustArray[10].ToString() + " " +
                App.Setting.AdjustArray[11].ToString() + " " +
                App.Setting.AdjustArray[12].ToString() ;
            }


        #endregion



        #region Histogram：画布


        private void Canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(Canvas_CreateResourcesAsync(sender).AsAsyncAction());
        }
        private async Task Canvas_CreateResourcesAsync(CanvasControl sender)
        {
           redBrush = CreateGradientBrush(sender, 255, 20, 60);
            greenBrush = CreateGradientBrush(sender, 127, 255, 0);
            blueBrush = CreateGradientBrush(sender, 0, 144, 255);
        }
        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (App.Model.MainRenderTarget != null)
            {
                //绘画直方图
                DrawHistogram(args.DrawingSession, sender.Size, EffectChannelSelect.Red, redBrush);
                DrawHistogram(args.DrawingSession, sender.Size, EffectChannelSelect.Green, greenBrush);
                DrawHistogram(args.DrawingSession, sender.Size, EffectChannelSelect.Blue, blueBrush);
            }
        }
        #endregion

        #region Histogram：直方图


        //创建直方图
        private void DrawHistogram(CanvasDrawingSession drawingSession, Size size, EffectChannelSelect channelSelect, CanvasLinearGradientBrush brush)
        {
            int binCount = (int)CanvasControl.ActualWidth; //256;//矩形数量
            const float graphPower = 0.25f; // 非线性规模使得图更清楚地显示微小的变化。

            try
            {
                float[] histogram = CanvasImage.ComputeHistogram(App.Model.MainRenderTarget, App.Model.MainRenderTarget.Bounds, drawingSession, channelSelect, binCount);

                var w = (float)size.Width / binCount;
                var h = (float)size.Height;

                for (int i = 0; i < binCount; i++)
                {
                    var x = i * w;
                    var y = (1 - (float)Math.Pow(histogram[i], graphPower)) * h;

                    brush.StartPoint = new Vector2(x, y);
                    brush.EndPoint = new Vector2(x, h);

                    drawingSession.FillRectangle(x, y, w, h - y, brush);
                }
            }
            catch (Exception)  {  }
        }
        //创建笔刷
        private CanvasLinearGradientBrush CreateGradientBrush(ICanvasResourceCreator resourceCreator, byte r, byte g, byte b)
        {
            const int stopCount = 8;
            const float falloffPower = 4;

            var stops = new CanvasGradientStop[stopCount];

            for (int i = 0; i < stopCount; i++)
            {
                var t = (float)i / (stopCount - 1);
                //     var a = (byte)(SmoothStep(t) * 255);

                stops[i].Position = (float)Math.Pow(1 - t, falloffPower);
                // stops[i].Color = Color.FromArgb(a, r, g, b);
                stops[i].Color = Color.FromArgb(200, r, g, b);
            }

            return new CanvasLinearGradientBrush(resourceCreator, stops);
        }
        //平滑转换
        private float SmoothStep(float x)
        {
            return (3 * x * x) - (2 * x * x * x);
        }


        #endregion

        private void Render()
        {
            if (App.Model.SecondSourceRenderTarget != null)
            {
                CanvasControl.Invalidate();//刷新直方图
                修图.BarPage.AdjustBarPage.Render();
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {

       string ssss=
                App.Setting.AdjustArray[0].ToString() + "f," +
                App.Setting.AdjustArray[1].ToString() + "f," +
                App.Setting.AdjustArray[2].ToString() + "f," +
                App.Setting.AdjustArray[3].ToString() + "f," +
                App.Setting.AdjustArray[4].ToString() + "f," +

                App.Setting.AdjustArray[5].ToString() + "f," +
                App.Setting.AdjustArray[6].ToString() + "f," +
                App.Setting.AdjustArray[7].ToString() + "f," +
                App.Setting.AdjustArray[8].ToString() + "f," +

                App.Setting.AdjustArray[9].ToString() + "f," +
                App.Setting.AdjustArray[10].ToString() + "f," +
                App.Setting.AdjustArray[11].ToString() + "f," +
                App.Setting.AdjustArray[12].ToString();

            App.Tip(ssss);//全局提示

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(ssss);
            Clipboard.SetContent(dataPackage);
        }

    }
}
