using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace 修图.BarPage.ToolPage
{
    public sealed partial class Mixer : Page
    {
        public Mixer()
        {
            this.InitializeComponent();
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Width：宽度
            WidthSlider.Value = Math.Sqrt(App.Setting.PaintWidth) * 10;
            WidthNumberPicker.Value = (int)App.Setting.PaintWidth;
            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;

            //Hard：硬度          
            HardSlider.Value = App.Setting.PaintHard * 100;
            HardNumberPicker.Value = (int)App.Setting.PaintHard * 100;
            HardSlider.ValueChanged += HardSlider_ValueChanged;
            HardNumberPicker.ValueChange += HardNumberPicker_ValueChange;

            // Opacity：不透明度
            OpacitySlider.Value = OpacityNumberPicker.Value = (int)(App.Setting.PaintOpacity * 100);
            OpacitySlider.ValueChanged += OpacitySlider_ValueChanged;
            OpacityNumberPicker.ValueChange += OpacityNumberPicker_ValueChange;

            //Space：间隔
            SpaceSlider.Value = SpaceNumberPicker.Value = (int)(App.Setting.PaintSpace * 100);
            SpaceSlider.ValueChanged += SpaceSlider_ValueChanged;
            SpaceNumberPicker.ValueChange += SpaceNumberPicker_ValueChange;

            Set();
  
        //Dry：干湿
        DrySlider.Value = DryNumberPicker.Value = App.Setting.MixerDry;
            DryNumberPicker.ValueChange += DryNumberPicker_ValueChange;
            DrySlider.ValueChanged += DrySlider_ValueChanged;

            DryButton.Content = App.Setting.MixerDry.ToString();
        }


        //Dry：干湿
        private void DryNumberPicker_ValueChange(object sender, int Value)
        {
            DrySlider.Value = App.Setting.MixerDry = Value;

            DryButton.Content = App.Setting.MixerDry.ToString();
        }
        private void DrySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DryNumberPicker.Value = App.Setting.MixerDry = (int)e.NewValue; ;

            DryButton.Content = App.Setting.MixerDry.ToString();
        }


        #endregion


        #region Paint：画笔


        //Width：宽度 
        private void WidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            WidthNumberPicker.ValueChange -= WidthNumberPicker_ValueChange;

            App.Setting.PaintWidth = (float)(e.NewValue * e.NewValue / 100);
            WidthNumberPicker.Value = (int)(e.NewValue * e.NewValue / 100);
            Set();

            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;
        }
        private void WidthNumberPicker_ValueChange(object sender, int Value)
        {
            WidthSlider.ValueChanged -= WidthSlider_ValueChanged;

            App.Setting.PaintWidth = Value;
            WidthSlider.Value = Math.Sqrt(App.Setting.PaintWidth) * 10;
            Set();

            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
        }





        //Hard：硬度 
        private void HardSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.PaintHard = (float)e.NewValue / 100f;
            HardNumberPicker.Value = (int)e.NewValue;
            Set();
        }
        private void HardNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PaintHard = Value / 100f;
            HardSlider.Value = Value;

            Set();
        }



        // Opacity：不透明度 
        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.PaintOpacity = (float)e.NewValue / 100f;
            OpacityNumberPicker.Value = (int)e.NewValue;
            Set();
        }
        private void OpacityNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PaintOpacity = (float)Value / 100f;
            OpacitySlider.Value = Value;
            Set();
        }



        //Space：间隔
        private void SpaceSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            App.Setting.PaintSpace = (float)e.NewValue / 100f;
            SpaceNumberPicker.Value = (int)e.NewValue;
            Set();
        }
        private void SpaceNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PaintSpace = (float)Value / 100f;
            SpaceSlider.Value = Value;
            Set();
        }


        #endregion




        #region Canvas：画布


        Size CanvasSize;

        private void ShowCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasSize = e.NewSize;
        }
        bool isRender = false;
        private void ShowCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (isRender == false)
            {
                isRender = true;

                if (App.Setting.Paint != null)
                {
                    var space = App.Setting.PaintSpace * (float)Math.Sqrt(App.Setting.PaintWidth);
                    for (float x = 10; x < CanvasSize.Width - 10; x += space)
                    {
                        //根据画布的X位置，求Sin高度角度（一条上下弧线）
                        var sinh = x / CanvasSize.Width * Math.PI * 2;
                        float h = 20 * (float)Math.Sin(sinh);//上下浮动

                        //根据画布的X位置，求Sin大小角度（一个上凸曲线）
                        var sins = x / CanvasSize.Width * Math.PI;
                        Vector2 s = new Vector2((float)Math.Sin(sins));//大小浮动
                        ScaleEffect se = new ScaleEffect { Source = App.Setting.PaintShow, Scale = s };

                        args.DrawingSession.DrawImage(se, x, (float)CanvasSize.Height / 2 + h);
                    }
                }
                isRender = false;
            }
        }


        #endregion




        private void Set()
        {
            if (App.Setting.isPaintBitmap == false)
            {
                App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintWidth, App.Setting.MixerColor);
            }
            else
            {
                App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintBitmap, App.Setting.PaintWidth, App.Setting.MixerColor);
            }
            ShowCanvas.Invalidate();
        }

    }
}
