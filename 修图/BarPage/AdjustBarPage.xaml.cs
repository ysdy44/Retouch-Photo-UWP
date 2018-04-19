using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 修图.Library;

namespace 修图.BarPage
{
    public sealed partial class AdjustBarPage : Page
    {
        public AdjustBarPage()
        {
            this.InitializeComponent();
        }

        private void NumberPicker_ValueChange(object sender, int Value)
        {
            Slider.Value = Value;
            App.Setting.AdjustOpacity = Value / 100;

            Render();
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (NumberPicker != null) NumberPicker.Value = (int)e.NewValue;
            App.Setting.AdjustOpacity = (float)e.NewValue / 100;

            Render();
        }




        public static void Render()
        {
            App.Model.SecondCanvasImage = App.Model.SecondSourceRenderTarget;
            float O = App.Setting.AdjustOpacity;
            float A = 1 - App.Setting.AdjustOpacity;

            //Light：光线 
            App.Model.SecondCanvasImage = Adjust.GetBrightness(App.Model.SecondCanvasImage, App.Setting.AdjustArray[0] * O + App.Setting.AdjustOriginArray[0] * A);//亮度
            App.Model.SecondCanvasImage = Adjust.GetExposure(App.Model.SecondCanvasImage, App.Setting.AdjustArray[1] * O + App.Setting.AdjustOriginArray[1] * A);//曝光
            App.Model.SecondCanvasImage = Adjust.GetContrast(App.Model.SecondCanvasImage, App.Setting.AdjustArray[2] * O + App.Setting.AdjustOriginArray[2] * A);//对比度
            App.Model.SecondCanvasImage = Adjust.GetHighlightsAndShadows(App.Model.SecondCanvasImage,
            App.Setting.AdjustArray[3] * O + App.Setting.AdjustOriginArray[3] * A,
            App.Setting.AdjustArray[4] * O + App.Setting.AdjustOriginArray[4] * A);//高光阴影

            //Color：颜色
            App.Model.SecondCanvasImage = Adjust.GetSaturation(App.Model.SecondCanvasImage, App.Setting.AdjustArray[5] * O + App.Setting.AdjustOriginArray[5] * A);//饱和度
            App.Model.SecondCanvasImage = Adjust.GetHueRotation(App.Model.SecondCanvasImage, App.Setting.AdjustArray[6] * O + App.Setting.AdjustOriginArray[6] * A);//色相
            App.Model.SecondCanvasImage = Adjust.GetTemperature(App.Model.SecondCanvasImage, App.Setting.AdjustArray[7] * O + App.Setting.AdjustOriginArray[7] * A);//冷暖
            App.Model.SecondCanvasImage = Adjust.GetVignette(App.Model.SecondCanvasImage, App.Setting.AdjustArray[8] * O + App.Setting.AdjustOriginArray[8] * A, Colors.Black);//装饰图案

            //Gamma：伽马
            App.Model.SecondCanvasImage = Adjust.GetGammaTransfer(App.Model.SecondCanvasImage,
            App.Setting.AdjustArray[9] * O + App.Setting.AdjustOriginArray[9] * A,
            App.Setting.AdjustArray[10] * O + App.Setting.AdjustOriginArray[10] * A,
            App.Setting.AdjustArray[11] * O + App.Setting.AdjustOriginArray[11] * A,
            App.Setting.AdjustArray[12] * O + App.Setting.AdjustOriginArray[12] * A);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }


    }
}
