using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage2
{
    public sealed partial class Vignette : Page
    {
        public Vignette()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }


        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            VignetteSlider.Value = VignetteNumberPicker.Value = (int)(App.Setting.VignetteAmountr * 100);
            VignetteSlider.ValueChanged += VignetteSlider_ValueChanged;
            VignetteNumberPicker.ValueChange += VignetteNumberPicker_ValueChange;

            Render();
        }

        #endregion


        #region Vignette：装饰图案


        private void VignetteSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            VignetteNumberPicker.Value = (int)e.NewValue;
            App.Setting.VignetteAmountr = (float)e.NewValue / 100;

            Render();
        }
        private void VignetteNumberPicker_ValueChange(object sender, int Value)
        {
            VignetteSlider.Value = Value;
            App.Setting.VignetteAmountr = (float)Value / 100;

            Render();
        }


        #endregion


        private void Render()
        {
            App.Model.SecondCanvasImage = Adjust.GetVignette(App.Model.SecondSourceRenderTarget, App.Setting.VignetteAmountr, App.Model.VignetteColor);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        } 
    }
}
