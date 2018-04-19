using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 修图.Library;


namespace 修图.BarPage.EffectPage3
{
    public sealed partial class Glass : Page
    {
        TurbulenceEffect turbulence;

        public Glass()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
             turbulence = new TurbulenceEffect//柏林噪波
            {
                Octaves = 8,
                Size = new Vector2(App.Model.Width, App.Model.Height),
            };

            Slider.Value= App.Setting.GlassAmount;
            NumberPicker.Value = (int)App.Setting.GlassAmount;
            Slider.ValueChanged += Slider_ValueChanged;
            NumberPicker.ValueChange += NumberPicker_ValueChange;

            Render();
        }


        #endregion


        #region Glass：玻璃


        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.GlassAmount = (float)e.NewValue;
            NumberPicker.Value = (int)App.Setting.GlassAmount;
            Render();
        }

        private void NumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.GlassAmount = Value;
            Slider.Value = App.Setting.GlassAmount;
            Render();
        }


        #endregion


        private void Render()
        {
            App.Model.SecondCanvasImage  = Adjust.GetDisplacementMap(App.Model.SecondSourceRenderTarget,turbulence, App.Setting.GlassAmount);
                
           App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
    }
}
