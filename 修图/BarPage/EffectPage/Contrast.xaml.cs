using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage
{
    public sealed partial class Contrast : Page
    {
        public Contrast()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            ContrastSlider.Value = ContrastNumberPicker.Value = (int)(App.Setting.Contrast * 200);
            ContrastSlider.ValueChanged += ContrastSlider_ValueChanged;
            ContrastNumberPicker.ValueChange += ContrastNumberPicker_ValueChange;

            Render();
        }

        #endregion


        #region Contrast：饱和度


        private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ContrastNumberPicker.Value = (int)e.NewValue;
            App.Setting.Contrast = (float)e.NewValue / 200;

            Render();
        }
        private void ContrastNumberPicker_ValueChange(object sender, int Value)
        {
            ContrastSlider.Value = Value;
            App.Setting.Contrast = (float)Value / 200;

            Render();
        }


        #endregion


        private void Render()
        {
            //Contrast：对比度
            App.Model.SecondCanvasImage = Adjust.GetContrast(App.Model.SecondSourceRenderTarget, App.Setting.Contrast);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

       


    }
}
