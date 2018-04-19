using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


namespace 修图.BarPage.ToolPage
{
    public sealed partial class Magic : Page
    {
        public Magic()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Slider.Value = App.Setting.MagicTolerance;
            if (NumberPicker != null) NumberPicker.Value = (int)App.Setting.MagicTolerance;

            Slider.ValueChanged += Slider_ValueChanged;
            NumberPicker.ValueChange += NumberPicker_ValueChange;
        }
        private void NumberPicker_ValueChange(object sender, int Value)
        {
            Slider.Value = Value;

            App.Setting.MagicTolerance = (float)Value; 
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (NumberPicker != null) NumberPicker.Value = (int)e.NewValue;

            App.Setting.MagicTolerance = (float)e.NewValue;
        }

   
    }
}
