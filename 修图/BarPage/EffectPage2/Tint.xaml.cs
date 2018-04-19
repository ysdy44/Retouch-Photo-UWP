using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
using Windows.UI.Xaml.Media;
using 修图.Library;

namespace 修图.BarPage.EffectPage2
{ 
    public sealed partial class Tint : Page
    {
        public Tint()
        {
            this.InitializeComponent();
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            Slider.Value = (int)(App.Setting.Tint * 100);
            Slider.ValueChanged += Slider_ValueChanged;

            Render(); 
        }

        #endregion


        #region Tint：色彩


        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Tint =(float)e.NewValue / 100;

            Render();
        }


        private void ColorButton_ColorChanged(Color Color)
        {
            Render();
        }


        #endregion


        public static void Render()
        {
            //Tint：色彩
            App.Model.SecondCanvasImage = Adjust.GetTint(App.Model.SecondSourceRenderTarget, App.Setting.Tint, App.Model.TintColor);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
    }
}
