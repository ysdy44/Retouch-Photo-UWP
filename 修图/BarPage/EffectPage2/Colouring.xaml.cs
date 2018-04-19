using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;


namespace 修图.BarPage.EffectPage2
{ 
    public sealed partial class Colouring : Page
    {
        public Colouring()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }

        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            HueRotationSlider.Value = HueRotationNumberPicker.Value = (int)(App.Setting.HueAngle * 100);
            HueRotationSlider.ValueChanged += HueRotationSlider_ValueChanged;
            HueRotationNumberPicker.ValueChange += HueRotationNumberPicker_ValueChange;

            Render();
        }

        #endregion


        #region Colouring：上色


        private void HueRotationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            HueRotationNumberPicker.Value = (int)e.NewValue;
            App.Setting.HueAngle = (float)e.NewValue / 100;

            Render();
        }
        private void HueRotationNumberPicker_ValueChange(object sender, int Value)
        {
            HueRotationSlider.Value = Value;
            App.Setting.HueAngle = (float)Value / 100;

            Render();
        }


        #endregion


        private void Render()
        {
            //Colouring：上色（褐色加色调）
            App.Model.SecondCanvasImage = Adjust.GetHueRotation(Adjust.GetSepia(App.Model.SecondSourceRenderTarget), App.Setting.HueAngle);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

    }
}
