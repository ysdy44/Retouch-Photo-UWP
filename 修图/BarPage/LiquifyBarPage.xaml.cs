using Microsoft.Graphics.Canvas;
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
using 修图.Model;

namespace 修图.BarPage
{ 
    public sealed partial class LiquifyBarPage : Page
    {
          
        public LiquifyBarPage()
        {
            this.InitializeComponent();
         }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            {
                ds.Clear(App.Setting.LiquifyColor);
            }


            SizeSlider.Value = SizeNumberPicker.Value = (int)App.Setting.LiquifySize;
            AmountSlider.Value = AmountNumberPicker.Value = (int)App.Setting.LiquifyAmount;
            SizeSlider.ValueChanged += SizeSlider_ValueChanged;
            SizeNumberPicker.ValueChange += SizeNumberPicker_ValueChange;
            AmountSlider.ValueChanged += AmountSlider_ValueChanged;
            AmountNumberPicker.ValueChange += AmountNumberPicker_ValueChange;


            // 0
            if (App.Setting.LiquifyMode == 0) ToolButton0.IsChecked = false;
            else ToolButton0.IsChecked = true;
            // 1
            if (App.Setting.LiquifyMode == 1) ToolButton1.IsChecked = false;
            else ToolButton1.IsChecked = true;
            // 2
            if (App.Setting.LiquifyMode == 2) ToolButton2.IsChecked = false;
            else ToolButton2.IsChecked = true;
            // 3
            if (App.Setting.LiquifyMode == 3) ToolButton3.IsChecked = false;
            else ToolButton3.IsChecked = true;

            Set();

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == ToolButton0)
            {
                ToolButton0.IsChecked = false;
                App.Setting.LiquifyMode = 0;
            }
            else ToolButton0.IsChecked = true;

            // 1
            if (tb == ToolButton1)
            {
                ToolButton1.IsChecked = false;
                App.Setting.LiquifyMode = 1;
            }
            else ToolButton1.IsChecked = true;

            // 2
            if (tb == ToolButton2)
            {
                ToolButton2.IsChecked = false;
                App.Setting.LiquifyMode = 2;
            }
            else ToolButton2.IsChecked = true;

            // 3
            if (tb == ToolButton3)
            {
                ToolButton3.IsChecked = false;
                App.Setting.LiquifyMode = 3;
            }
            else ToolButton3.IsChecked = true;

            Set();

        Tip(App.Setting.LiquifyMode);
    }


        private void Tip(int Tool)
        {
            switch (Tool)
            {
                //Tool：工具
                case 0: App.Tip("Push Tool"); break;
                case 1: App.Tip("Pinch Tool"); break;
                case 2: App.Tip("Punch Tool"); break;
                case 3: App.Tip("Pacified Tool"); break;
            }
        }
        
        
        #endregion

             
        #region Liquify：液化


                private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.LiquifySize = (float)e.NewValue;
            SizeNumberPicker.Value = (int)e.NewValue;
            Set();
        }
        private void SizeNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.LiquifySize = Value;
            SizeSlider.Value = Value;
            Set();
        }



        private void AmountSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.LiquifyAmount = (float)e.NewValue;
            AmountNumberPicker.Value = (int)e.NewValue;
            Set();
        }
        private void AmountNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.LiquifyAmount = Value;
            AmountSlider.Value = Value;

            Set();
        }
        #endregion


        public static void Set()
        {
            switch (App.Setting.LiquifyMode)
            {
                case 0:
                    App.Setting.LiquifyFill(App.Model.VirtualControl);
                    break;
                 case 1:
                    App.Setting.LiquifyFill(App.Model.VirtualControl, App.Model.PunchDisplacement); //Punch：膨胀
                    break;
                case 2:
                    App.Setting.LiquifyFill(App.Model.VirtualControl, App.Model.PinchDisplacement);//Pinch：收缩
                    break;
                case 3:
                    App.Setting.LiquifyFill(App.Model.VirtualControl);
                    break;

                default:
                    break;
            }
        }



         


    }
}
