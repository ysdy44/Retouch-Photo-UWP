using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
namespace 修图.BarPage.ToolPage
{
    public sealed partial class Smudge : Page
    {
        public Smudge()
        {
            this.InitializeComponent();
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Width：宽度
            WidthSlider.Value = WidthNumberPicker.Value = (int)App.Setting.PaintWidth;
            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;
        }


        #endregion



        //Width：宽度 
        private void WidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.PaintWidth = (float)e.NewValue;
            WidthNumberPicker.Value = (int)e.NewValue;
        }
        private void WidthNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PaintWidth = Value;
            WidthSlider.Value = Value;
        }
    }
}
