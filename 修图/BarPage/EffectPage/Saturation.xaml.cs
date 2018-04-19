using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage
{

    public sealed partial class Saturation : Page
    {
        public Saturation()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            SaturationSlider.Value = SaturationNumberPicker.Value = (int)(App.Setting.Saturation * 100);
            SaturationSlider.ValueChanged += SaturationSlider_ValueChanged;
            SaturationNumberPicker.ValueChange += SaturationNumberPicker_ValueChange;

            Render();
        }

        #endregion


        #region Saturation：饱和度


        private void SaturationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SaturationNumberPicker.Value = (int)e.NewValue;
            App.Setting.Saturation = (float)e.NewValue/100;

            Render();
        }
        private void SaturationNumberPicker_ValueChange(object sender, int Value)
        {
            SaturationSlider.Value = Value;
            App.Setting.Saturation = (float)Value / 100;

            Render();
        }


        #endregion


        private void Render()
        {
            //Saturation：饱和度
            App.Model.SecondCanvasImage = Adjust.GetSaturation(App.Model.SecondSourceRenderTarget, App.Setting.Saturation);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

      

    }
}
