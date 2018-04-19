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
    public sealed partial class GammaTransfer : Page
    {
        public GammaTransfer()
        {
            this.InitializeComponent();
        }
    
 
        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Alpha 
            AlphaAmplitude.Value = App.Setting.AlphaAmplitude * 100;
            AlphaExponent.Value= App.Setting.AlphaExponent * 100;
            AlphaOffset.Value= App.Setting.AlphaOffset * 100;

            AlphaAmplitude.ValueChanged += AlphaAmplitude_ValueChanged;
            AlphaExponent.ValueChanged += AlphaExponent_ValueChanged;
            AlphaOffset.ValueChanged += AlphaOffset_ValueChanged;
            
            // Red 
            RedAmplitude.Value = App.Setting.RedAmplitude * 100;
            RedExponent.Value = App.Setting.RedExponent * 100;
            RedOffset.Value = App.Setting.RedOffset * 100;

            RedAmplitude.ValueChanged += RedAmplitude_ValueChanged;
            RedExponent.ValueChanged += RedExponent_ValueChanged;
            RedOffset.ValueChanged += RedOffset_ValueChanged;
            
            // Green 
            GreenAmplitude.Value = App.Setting.GreenAmplitude * 100;
            GreenExponent.Value = App.Setting.GreenExponent * 100;
            GreenOffset.Value = App.Setting.GreenOffset * 100;

            GreenAmplitude.ValueChanged += GreenAmplitude_ValueChanged;
            GreenExponent.ValueChanged += GreenExponent_ValueChanged;
            GreenOffset.ValueChanged += GreenOffset_ValueChanged;
            
            // Green 
            GreenAmplitude.Value = App.Setting.GreenAmplitude * 100;
            GreenExponent.Value = App.Setting.GreenExponent * 100;
            GreenOffset.Value = App.Setting.GreenOffset * 100;

            GreenAmplitude.ValueChanged += GreenAmplitude_ValueChanged;
            GreenExponent.ValueChanged += GreenExponent_ValueChanged;
            GreenOffset.ValueChanged += GreenOffset_ValueChanged;

             

            //控件事件
            // 0
            if (App.Setting.GammaTransferMode == 0)
            {
                 AlphaButton.IsChecked = false;
                AlphaGrid.Visibility = Visibility.Visible; 
            }
            else
            {
                AlphaButton.IsChecked = true;
                AlphaGrid.Visibility = Visibility.Collapsed;
            }
            // 1
            if (App.Setting.GammaTransferMode == 1)
            {
                RedButton.IsChecked = false;
                RedGrid.Visibility = Visibility.Visible;
            }
            else
            {
                RedButton.IsChecked = true;
                RedGrid.Visibility = Visibility.Collapsed;
            }    
            //2
            if (App.Setting.GammaTransferMode == 2)
            {
                GreenButton.IsChecked = false;
                GreenGrid.Visibility = Visibility.Visible;
            }
            else
            {
                GreenButton.IsChecked = true;
                GreenGrid.Visibility = Visibility.Collapsed;
            }    
            // 3
            if (App.Setting.GammaTransferMode == 3)
            {
                BlueButton.IsChecked = false;
                BlueGrid.Visibility = Visibility.Visible;
            }
            else
            {
                BlueButton.IsChecked = true;
                BlueGrid.Visibility = Visibility.Collapsed;
            }
        }


        private void GammaTransfer_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == AlphaButton)
            {
                App.Setting.GammaTransferMode = 0;

                AlphaButton.IsChecked = false;
                AlphaGrid.Visibility = Visibility.Visible; 
            }
            else
            {
                AlphaButton.IsChecked = true;
                AlphaGrid.Visibility = Visibility.Collapsed;
            }

            // 1
            if (tb == RedButton)
            {
                App.Setting.GammaTransferMode = 1;

                RedButton.IsChecked = false;
                RedGrid.Visibility = Visibility.Visible;
            }
            else
            {
                RedButton.IsChecked = true;
                RedGrid.Visibility = Visibility.Collapsed;
            }

            //2
            if (tb == GreenButton)
            {
                App.Setting.GammaTransferMode = 2;

                GreenButton.IsChecked = false;
                GreenGrid.Visibility = Visibility.Visible;
            }
            else
            {
                GreenButton.IsChecked = true;
                GreenGrid.Visibility = Visibility.Collapsed;
            }

            // 3
            if (tb == BlueButton)
            {
                App.Setting.GammaTransferMode = 3;

                BlueButton.IsChecked = false;
                BlueGrid.Visibility = Visibility.Visible;
            }
            else
            {
                BlueButton.IsChecked = true;
                BlueGrid.Visibility = Visibility.Collapsed;
            }
 
            Render();
        }


        #endregion


        #region GammaTransfer：伽马转换


        // Alpha
        private void AlphaAmplitude_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.AlphaAmplitude = (float)e.NewValue / 100f;
            Render();
        }
        private void AlphaExponent_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.AlphaExponent = (float)e.NewValue / 100f;
            Render();
        }
        private void AlphaOffset_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.AlphaOffset = (float)e.NewValue / 100f;
            Render();
        }


        // Red
         private void RedAmplitude_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.RedAmplitude = (float)e.NewValue / 100f;
            Render();
        }
        private void RedExponent_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.RedExponent = (float)e.NewValue / 100f;
            Render();
        }
        private void RedOffset_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.RedOffset = (float)e.NewValue / 100f;
            Render();
        }


        // Green
        private void GreenAmplitude_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.GreenAmplitude = (float)e.NewValue / 100f;
            Render();
        }
        private void GreenExponent_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.GreenExponent = (float)e.NewValue / 100f;
            Render();
        }
        private void GreenOffset_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.GreenOffset = (float)e.NewValue / 100f;
            Render();
        }

         
        // Blue
        private void BlueAmplitude_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.BlueAmplitude = (float)e.NewValue / 100f;
            Render();
        }
        private void BlueExponent_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.BlueExponent = (float)e.NewValue / 100f;
            Render();
        }
        private void BlueOffset_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
             App.Setting.BlueOffset = (float)e.NewValue / 100f;
            Render();
        }


        #endregion

        private void Render()
        {
            //GammaTransfer：伽马转换
            App.Model.SecondCanvasImage = Adjust.GetGammaTransfer(
                App.Model.SecondSourceRenderTarget,
                App.Setting.AlphaAmplitude,App.Setting.AlphaExponent,App.Setting.AlphaOffset,
                App.Setting.RedAmplitude,App.Setting.RedExponent,App.Setting.RedOffset,
                App.Setting.GreenAmplitude,App.Setting.GreenExponent,App.Setting.GreenOffset,
                App.Setting.BlueAmplitude,App.Setting.BlueExponent,App.Setting.BlueOffset);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }


    }
}
