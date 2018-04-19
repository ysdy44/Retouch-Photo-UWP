using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage
{
    public sealed partial class HighlightsAndShadows : Page
    {
        public HighlightsAndShadows()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局      

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            ShadowsSlider.Value = ShadowsNumberPicker.Value = (int)(App.Setting.Shadows * 200);
            ShadowsSlider.ValueChanged += ShadowsSlider_ValueChanged;
            ShadowsNumberPicker.ValueChange += ShadowsNumberPicker_ValueChange;

            HighlightsSlider.Value = HighlightsNumberPicker.Value = (int)(App.Setting.Highlights * 200);
            HighlightsSlider.ValueChanged += HighlightsSlider_ValueChanged;
            HighlightsNumberPicker.ValueChange += HighlightsNumberPicker_ValueChange;


            ClaritySlider.Value = ClarityNumberPicker.Value = (int)(App.Setting.Clarity * 200);
            ClaritySlider.ValueChanged += ClaritySlider_ValueChanged;
            ClarityNumberPicker.ValueChange += ClarityNumberPicker_ValueChange;

            BlurSlider.Value = BlurNumberPicker.Value = (int)(App.Setting.MaskBlurAmount * 100);
            BlurSlider.ValueChanged += BlurSlider_ValueChanged;
            BlurNumberPicker.ValueChange += BlurNumberPicker_ValueChange;


            // 0
            if (App.Setting.SourceIsLinearGamma == false) Linear.IsChecked = false;
            else Linear.IsChecked = true;
            // 1
            if (App.Setting.SourceIsLinearGamma == true) Gamma.IsChecked = false;
            else Gamma.IsChecked = true;

            Render();
        }



        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == HASTB)
            {
                HASTB.IsChecked = false;
                HASG.Visibility = Visibility.Visible;
            }
            else
            {
                HASTB.IsChecked = true;
                HASG.Visibility = Visibility.Collapsed;
            }

            // 1
            if (tb == CABTB)
            {
                CABTB.IsChecked = false;
                CABG.Visibility = Visibility.Visible;
            }
            else
            {
                CABTB.IsChecked = true;
                CABG.Visibility = Visibility.Collapsed;
            }
        }

        #endregion


        #region HighlightsAndShadows：高光与阴影


        private void ShadowsSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ShadowsNumberPicker.Value = (int)e.NewValue;
            App.Setting.Shadows = (float)e.NewValue / 200;

            Render();
        }
        private void ShadowsNumberPicker_ValueChange(object sender, int Value)
        {
            ShadowsSlider.Value = Value;
            App.Setting.Shadows = (float)Value / 200;

            Render();
        }




        private void HighlightsSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            HighlightsNumberPicker.Value = (int)e.NewValue;
            App.Setting.Highlights = (float)e.NewValue / 200;

            Render();
        }
        private void HighlightsNumberPicker_ValueChange(object sender, int Value)
        {
            HighlightsSlider.Value = Value;
            App.Setting.Highlights = (float)Value / 100;

            Render();
        }


        

         
        private void ClaritySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ClarityNumberPicker.Value = (int)e.NewValue;
            App.Setting.Clarity = (float)e.NewValue / 200;

            Render();
        }
        private void ClarityNumberPicker_ValueChange(object sender, int Value)
        {
            ClaritySlider.Value = Value;
            App.Setting.MaskBlurAmount = (float)Value / 200;

            Render();
        }



        private void BlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BlurNumberPicker.Value = (int)e.NewValue;
            App.Setting.Highlights = (float)e.NewValue / 200;

            Render();
        }
        private void BlurNumberPicker_ValueChange(object sender, int Value)
        {
            BlurSlider.Value = Value;
            App.Setting.Highlights = (float)Value / 100;

            Render();
        }







        private void LinearGamma_Click(object sender, RoutedEventArgs e)
        {

            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Linear)
            {
                Linear.IsChecked = false;
                App.Setting.SourceIsLinearGamma = false;
            }
            else Linear.IsChecked = true;

            // 1
            if (tb == Gamma)
            {
                Gamma.IsChecked = false;
                App.Setting.SourceIsLinearGamma = true;
            }
            else Gamma.IsChecked = true;

            Render();
        }

        #endregion


        private void Render()
        {
            //HighlightsAndShadows：高光阴影
            App.Model.SecondCanvasImage = Adjust.GetHighlightsAndShadows(App.Model.SecondSourceRenderTarget, App.Setting.Shadows, App.Setting.Highlights, App.Setting.Clarity, App.Setting.MaskBlurAmount, App.Setting.SourceIsLinearGamma);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

       

    }
}
