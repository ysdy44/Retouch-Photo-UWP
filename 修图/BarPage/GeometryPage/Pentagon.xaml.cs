using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas.Geometry;

namespace 修图.BarPage.GeometryPage
{ 
    public sealed partial class Pentagon : Page
    {
        public Pentagon()
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


            CountSlider.Value =  CountNumberPicker.Value = App.Setting.PentagonCount;
            CountSlider.ValueChanged += CountSlider_ValueChanged;
            CountNumberPicker.ValueChange += CountNumberPicker_ValueChange;

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
            App.Setting.PentagonCount = (int)e.NewValue;
            CountNumberPicker.Value = (int)e.NewValue;
        }
        private void CountNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PentagonCount = Value;
            CountSlider.Value = Value;
        }



    }
}
