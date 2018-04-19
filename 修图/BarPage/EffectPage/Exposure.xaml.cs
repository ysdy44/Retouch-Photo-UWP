using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage
{
    public sealed partial class Exposure : Page
    {
        public Exposure()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            ExposureSlider.Value = ExposureNumberPicker.Value = (int)(App.Setting.Exposure * 100);
            ExposureSlider.ValueChanged += ExposureSlider_ValueChanged;
            ExposureNumberPicker.ValueChange += ExposureNumberPicker_ValueChange;

            Render();
        }

        #endregion


        #region Exposure：曝光


        private void ExposureSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ExposureNumberPicker.Value = (int)e.NewValue;
            App.Setting.Exposure = (float)e.NewValue / 100;

            Render();
        }
        private void ExposureNumberPicker_ValueChange(object sender, int Value)
        {
            ExposureSlider.Value = Value;
            App.Setting.Exposure = (float)Value / 100;

            Render();
        }


        #endregion


        private void Render()
        {
            //Exposure：曝光
            App.Model.SecondCanvasImage =Adjust.GetExposure(App.Model.SecondSourceRenderTarget, App.Setting.Exposure);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
         

    }
}
