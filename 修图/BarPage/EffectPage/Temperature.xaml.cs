using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage
{ 
    public sealed partial class Temperature : Page
    {
        public Temperature()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            TemperatureSlider.Value = TemperatureNumberPicker.Value = (int)(App.Setting.Temperature * 200);
            TemperatureSlider.ValueChanged += TemperatureSlider_ValueChanged;
            TemperatureNumberPicker.ValueChange += TemperatureNumberPicker_ValueChange;

         TintSlider.Value = TintNumberPicker.Value = (int)(App.Setting.Temperature * 200);
            TintSlider.ValueChanged += TintSlider_ValueChanged;
            TintNumberPicker.ValueChange += TintNumberPicker_ValueChange;

            Render();
        }

        #endregion


        #region Temperature：冷暖


        private void TemperatureSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TemperatureNumberPicker.Value = (int)e.NewValue;
            App.Setting.Temperature = (float)e.NewValue / 200;

            Render();
        }
        private void TemperatureNumberPicker_ValueChange(object sender, int Value)
        {
            TemperatureSlider.Value = Value;
            App.Setting.Temperature = (float)Value / 200;

            Render();
        }



        private void TintSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TintNumberPicker.Value = (int)e.NewValue;
            App.Setting.TemperatureTint = (float)e.NewValue / 200;

            Render();
        }
        private void TintNumberPicker_ValueChange(object sender, int Value)
        {
            TintSlider.Value = Value;
            App.Setting.TemperatureTint = (float)Value / 200;

            Render();
        }


        #endregion


        private void Render()
        {
            //Temperature：冷暖
            App.Model.SecondCanvasImage = Adjust.GetTemperature(App.Model.SecondSourceRenderTarget, App.Setting.Temperature, App.Setting.TemperatureTint);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }



    }
}
