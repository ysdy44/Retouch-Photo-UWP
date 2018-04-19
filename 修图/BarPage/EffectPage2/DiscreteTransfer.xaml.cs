using Microsoft.Graphics.Canvas;
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

namespace 修图.BarPage.EffectPage2
{ 
    public sealed partial class DiscreteTransfer : Page
    {
         public DiscreteTransfer()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }

        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            // 0
            if (App.Setting.isTable == false)
            {
                DiscreteTransfer0.IsChecked = false;

                Content.Visibility = Visibility.Collapsed;
                Content1.Visibility = Visibility.Visible;

                RedSlider.Value = App.Setting.RedTable[1] * 100;
                GreenSlider.Value = App.Setting.GreenTable[1] * 100;
                BlueSlider.Value = App.Setting.BlueTable[1] * 100;
            }
            else DiscreteTransfer0.IsChecked = true;
            // 1
            if (App.Setting.isTable == true)
            {
                DiscreteTransfer1.IsChecked = false;

                Content.Visibility = Visibility.Visible;
                Content1.Visibility = Visibility.Collapsed;

               AlphaSlider.Value = (App.Setting.RedTable[1] + App.Setting.GreenTable[1] + App.Setting.BlueTable[1] )/3* 100;
            }
            else DiscreteTransfer1.IsChecked = true;

            Render();
        }

        #endregion


        #region DiscreteTransfer：离散转让


        private void DiscreteTransfer_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == DiscreteTransfer0)
            {
                DiscreteTransfer0.IsChecked = false;
                App.Setting.isTable = false;

                Content.Visibility = Visibility.Collapsed;
                Content1.Visibility = Visibility.Visible;

                RedSlider.Value = App.Setting.RedTable[1] * 100;
                GreenSlider.Value = App.Setting.GreenTable[1] * 100;
                BlueSlider.Value = App.Setting.BlueTable[1] * 100;
            }
            else DiscreteTransfer0.IsChecked = true;

            // 1
            if (tb == DiscreteTransfer1)
            {
                DiscreteTransfer1.IsChecked = false;
                App.Setting.isTable = true;

                Content.Visibility = Visibility.Visible;
                Content1.Visibility = Visibility.Collapsed;

                AlphaSlider.Value = (App.Setting.RedTable[1] + App.Setting.GreenTable[1] + App.Setting.BlueTable[1]) / 3 * 100;
            }
            else DiscreteTransfer1.IsChecked = true;
             
        }



        private void AlphaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.RedTable[1] = App.Setting.GreenTable[1] = App.Setting.BlueTable[1] = (float)e.NewValue / 100f;
            Render();
        }




        private void RedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.RedTable[1] = (float)e.NewValue / 100f;
            Render();
        }
        private void GreenSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.GreenTable[1] = (float)e.NewValue / 100f;
            Render();
        }
        private void BlueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.BlueTable[1] = (float)e.NewValue / 100f;
            Render();
        }


        #endregion


        private void Render()
        {
            //DiscreteTransfer：离散转让
            App.Model.SecondCanvasImage = GetDiscreteTransfer(
                App.Model.SecondSourceRenderTarget,
                App.Setting.RedDisable,
                App.Setting.GreenDisable,
                App.Setting.BlueDisable,
                App.Setting.RedTable,
                App.Setting.GreenTable,
                App.Setting.BlueTable);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        //离散转让方法
        public ICanvasImage GetDiscreteTransfer(ICanvasImage canvasImage, bool RedDisable, bool GreenDisable, bool BlueDisable, float[] RedTable, float[] GreenTable, float[] BlueTable)
        {
            return new DiscreteTransferEffect
            {
                Source = canvasImage,

                RedDisable = RedDisable,
                GreenDisable = GreenDisable,
                BlueDisable = BlueDisable,

                RedTable = RedTable,
                GreenTable = GreenTable,
                BlueTable = BlueTable,
            };
        }
    }
}
