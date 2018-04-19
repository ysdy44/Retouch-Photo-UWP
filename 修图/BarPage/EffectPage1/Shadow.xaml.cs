using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage1
{ 
    public sealed partial class Shadow : Page
    {
        public Shadow()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            AmountSlider.Value = (int)App.Setting.ShadowAmount;
            XSlider.Value = XNumberPicker.Value = (int)App.Setting.ShadowX;
            YSlider.Value = YNumberPicker.Value = (int)App.Setting.ShadowY;
            OpacitySlider.Value = OpacityNumberPicker.Value = (int)(App.Setting.ShadowOpacity * 100);

            Render();
        }

        private void ColorButton_ColorChanged(Color Color)
        {
            App.Model.ShadowColor = Color;

            Render();
        }

        private void ColorButton_StrawChanged()
        {
            flyout.Hide();
        }

        #endregion


        #region Shadow：阴影


        private void AmountSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.ShadowAmount = (float)e.NewValue;
            Render();
        }



        private void XSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            XNumberPicker.Value = (int)e.NewValue;
            App.Setting.ShadowX = (float)e.NewValue;
            Render();
        }
        private void XNumberPicker_ValueChange(object sender, int Value)
        {
            XSlider.Value = Value;
            App.Setting.ShadowX = (float)Value;
            Render();
        }



        private void YSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            YNumberPicker.Value = (int)e.NewValue;
            App.Setting.ShadowY = (float)e.NewValue;
            Render();
        }
        private void YNumberPicker_ValueChange(object sender, int Value)
        {
            YSlider.Value = Value;
            App.Setting.ShadowY = (float)Value;
            Render();
        }



        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            OpacityNumberPicker.Value = (int)e.NewValue;
            App.Setting.ShadowOpacity = (float)e.NewValue / 100;
            Render();
        }
        private void OpacityNumberPicker_ValueChange(object sender, int Value)
        {
            OpacitySlider.Value = Value;
            App.Setting.ShadowOpacity = (float)Value / 100;
            Render();
        }

        #endregion


        public static void Render()
        {
            //Shadow：阴影
            App.Model.SecondCanvasImage = Adjust.GetShadow(App.Model.SecondSourceRenderTarget,App.Setting.ShadowAmount, App.Model.ShadowColor, App.Setting.ShadowX, App.Setting.ShadowY, App.Setting.ShadowOpacity);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

    }
}
