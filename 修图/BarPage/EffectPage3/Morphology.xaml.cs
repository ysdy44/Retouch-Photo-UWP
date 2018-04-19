using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage3
{ 
    public sealed partial class Morphology : Page
    {
        public Morphology()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            Slider.Value = NumberPicker.Value = (int)(App.Setting.MorphologyAmount);
            Slider.ValueChanged += Slider_ValueChanged;
            NumberPicker.ValueChange += NumberPicker_ValueChange;

            Render();
        }

        #endregion


        #region Morphology：形态学


        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
           NumberPicker.Value = (int)e.NewValue;
            App.Setting.MorphologyAmount = (float)e.NewValue ;

            Render();
        }
        private void NumberPicker_ValueChange(object sender, int Value)
        {
          Slider.Value = Value;
            App.Setting.MorphologyAmount = (float)Value ;

            Render();
        } 


        #endregion


        private void Render()
        {
            //Morphology：形态学
            App.Model.SecondCanvasImage = Adjust.GetMorphology(App.Model.SecondSourceRenderTarget, App.Setting.MorphologyAmount);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

    }
}
