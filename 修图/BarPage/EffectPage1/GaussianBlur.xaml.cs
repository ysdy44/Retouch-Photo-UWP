using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage1
{
    public sealed partial class GaussianBlur : Page
    {
        public GaussianBlur()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            GaussianBlurSlider.Value = GaussianBlurNumberPicker.Value = (int)App.Setting.BlurAmount;
            GaussianBlurSlider.ValueChanged += GaussianBlurSlider_ValueChanged;
            GaussianBlurNumberPicker.ValueChange += GaussianBlurNumberPicker_ValueChange;

            Render();


            // 0
            if (App.Setting.Optimization == EffectOptimization.Speed) Optimization0.IsChecked = false;
            else Optimization0.IsChecked = true;
            // 1
            if (App.Setting.Optimization == EffectOptimization.Balanced) Optimization1.IsChecked = false;
            else Optimization1.IsChecked = true;
            // 2      
            if (App.Setting.Optimization == EffectOptimization.Quality) Optimization2.IsChecked = false;
            else Optimization2.IsChecked = true;
 
            // 0
            if (App.Setting.BorderMode == EffectBorderMode.Soft) BorderMode0.IsChecked = false;
            else BorderMode0.IsChecked = true;
            // 1
            if (App.Setting.BorderMode == EffectBorderMode.Hard) BorderMode1.IsChecked = false;
            else BorderMode1.IsChecked = true;
        }

        #endregion


        #region GaussianBlur：高斯模糊

        private void GaussianBlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            GaussianBlurNumberPicker.Value = (int)e.NewValue;
            App.Setting.BlurAmount = (float)e.NewValue;

            Render();
        }
        private void GaussianBlurNumberPicker_ValueChange(object sender, int Value)
        {
            GaussianBlurSlider.Value = Value;
            App.Setting.BlurAmount = (float)Value ;

            Render();
        }





        private void Optimization_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == Optimization0)
            {
                Optimization0.IsChecked = false;
                App.Setting.Optimization = EffectOptimization.Speed;
            }
            else Optimization0.IsChecked = true;

            // 1
            if (tb == Optimization1)
            {
                Optimization1.IsChecked = false;
                App.Setting.Optimization = EffectOptimization.Balanced;
            }
            else Optimization1.IsChecked = true;

            // 2
            if (tb == Optimization2)
            {
                Optimization2.IsChecked = false;
                App.Setting.Optimization = EffectOptimization.Quality;
            }
            else Optimization2.IsChecked = true;

            Render();
        }


        private void BorderMode_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == BorderMode0)
            {
                BorderMode0.IsChecked = false;
                App.Setting.BorderMode = EffectBorderMode.Soft;
            }
            else BorderMode0.IsChecked = true;

            // 1
            if (tb == BorderMode1)
            {
                BorderMode1.IsChecked = false;
                App.Setting.BorderMode = EffectBorderMode.Hard;
            }
            else BorderMode1.IsChecked = true;

            Render();
        }
        #endregion


        private void Render()
        {
            //GaussianBlur：高斯模糊
            App.Model.SecondCanvasImage = Adjust.GetGaussianBlur(App.Model.SecondSourceRenderTarget, App.Setting.BlurAmount, App.Setting.Optimization, App.Setting.BorderMode);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

    }
}
