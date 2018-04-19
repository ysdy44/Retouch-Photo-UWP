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
using Microsoft.Graphics.Canvas.Geometry;


namespace 修图.BarPage.GeometryPage
{ 
    public sealed partial class Cog : Page
    {
        public Cog()
        {
            this.InitializeComponent();
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StrokePicker.StrokeChanged += StrokePicker_StrokeChanged;

            WidthSlider.Value = App.Setting.GeometryWidth;
            WidthNumberPicker.Value = (int)App.Setting.GeometryWidth;
            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;


            CountSlider.Value = CountNumberPicker.Value = App.Setting.CogCount;
            CountSlider.ValueChanged += CountSlider_ValueChanged;
            CountNumberPicker.ValueChange += CountNumberPicker_ValueChange;

            InnerSlider.Value = App.Setting.CogInner * 100;
            InnerNumberPicker.Value = (int)(App.Setting.CogInner * 100);
            InnerSlider.ValueChanged += InnerSlider_ValueChanged;
            InnerNumberPicker.ValueChange += InnerNumberPicker_ValueChange;

            ToothSlider.Value = App.Setting.CogTooth * 100;
            ToothNumberPicker.Value = (int)(App.Setting.CogTooth * 100);
            ToothSlider.ValueChanged += ToothSlider_ValueChanged;
            ToothNumberPicker.ValueChange += ToothNumberPicker_ValueChange;

            NotchSlider.Value = App.Setting.CogNotch * 100;
            NotchNumberPicker.Value = (int)(App.Setting.CogNotch * 100);
            NotchSlider.ValueChanged += NotchSlider_ValueChanged;
            NotchNumberPicker.ValueChange += NotchNumberPicker_ValueChange;


            // 0
            if (App.Setting.GeometryMode == 0)
            {
                Fill.IsChecked = true;
                FillGrid.Visibility = Visibility.Visible;
            }
            else
            {
                Fill.IsChecked = false;
                FillGrid.Visibility = Visibility.Collapsed;
            }

            // 1
            if (App.Setting.GeometryMode == 1)
            {
                Stroke.IsChecked = true;
                StrokeGrid.Visibility = Visibility.Visible;
            }
            else
            {
                Stroke.IsChecked = false;
                StrokeGrid.Visibility = Visibility.Collapsed;
            }

            // 2
            if (App.Setting.GeometryMode == 2)
            {
                Geometry.IsChecked = true;
                GeometryGrid.Visibility = Visibility.Visible;
            }
            else
            {
                Geometry.IsChecked = false;
                GeometryGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void Geometry_Click(object sender, RoutedEventArgs e)
        {

            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Fill)
            {
                Fill.IsChecked = true;
                FillGrid.Visibility = Visibility.Visible;

                App.Setting.GeometryMode = 0;
            }
            else
            {
                Fill.IsChecked = false;
                FillGrid.Visibility = Visibility.Collapsed;
            }

            // 1
            if (tb == Stroke)
            {
                Stroke.IsChecked = true;
                StrokeGrid.Visibility = Visibility.Visible;

                App.Setting.GeometryMode = 1;
            }
            else
            {
                Stroke.IsChecked = false;
                StrokeGrid.Visibility = Visibility.Collapsed;
            }

            // 2
            if (tb == Geometry)
            {
                Geometry.IsChecked = true;
                GeometryGrid.Visibility = Visibility.Visible;

                App.Setting.GeometryMode = 2;
            }
            else
            {
                Geometry.IsChecked = false;
                GeometryGrid.Visibility = Visibility.Collapsed;
            }
        }

        #endregion


        #region Fill&Stroke：全局



        private void StrokePicker_StrokeChanged(CanvasStrokeStyle StrokeStyle)
        {
            App.Setting.GeometryStrokeStyle = StrokeStyle;
        }




        //Width：宽度 
        private void WidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.GeometryWidth = (float)e.NewValue;
            WidthNumberPicker.Value = (int)e.NewValue;
        }
        private void WidthNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.GeometryWidth = Value;
            WidthSlider.Value = Value;
        }

        #endregion


        //Count：边数 
        private void CountSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.CogCount = (int)e.NewValue;
            CountNumberPicker.Value = (int)e.NewValue;
        }
        private void CountNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CogCount = Value;
            CountSlider.Value = Value;
        }

        //Inner：半径 
        private void InnerSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.CogInner = (float)(e.NewValue / 100);
            InnerNumberPicker.Value = (int)e.NewValue;
        }
        private void InnerNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CogInner = Value / 100f;
            InnerSlider.Value = Value;
        }


        //Tooth：牙齿
        private void ToothSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.CogTooth = (float)(e.NewValue / 180 * Math.PI);
            ToothNumberPicker.Value = (int)e.NewValue;
        }
        private void ToothNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CogTooth = (float)(Value / 180 * Math.PI);
            ToothSlider.Value = Value;
        }

        //Notch：切口
        private void NotchSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.CogNotch = (float)(e.NewValue / 180 * Math.PI);
            NotchNumberPicker.Value = (int)e.NewValue;
        }
        private void NotchNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.CogNotch = (float)(Value / 180 * Math.PI);
            NotchSlider.Value = Value;
        }
    }
}
