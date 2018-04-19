using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage1
{
    public sealed partial class Sharpen : Page
    {
        public Sharpen()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }

        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            SharpenSlider.Value = SharpenNumberPicker.Value = (int)(App.Setting.Sharpen * 100);
            SharpenSlider.ValueChanged += SharpenSlider_ValueChanged;
            SharpenNumberPicker.ValueChange += SharpenNumberPicker_ValueChange;

            Render();
        }

        #endregion


        #region Sharpen：锐化


        private void SharpenSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SharpenNumberPicker.Value = (int)e.NewValue;
            App.Setting.Sharpen = (float)e.NewValue / 10;

            Render();
        }
        private void SharpenNumberPicker_ValueChange(object sender, int Value)
        {
            SharpenSlider.Value = Value;
            App.Setting.Sharpen = (float)Value / 10;

            Render();
        }


        #endregion


        private void Render()
        {
            //Sharpen：锐化
            App.Model.SecondCanvasImage = Adjust.GetSharpen(App.Model.SecondSourceRenderTarget, App.Setting.Sharpen);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

    }
}
