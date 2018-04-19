using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
using Windows.UI.Xaml.Media;
using 修图.Library;

namespace 修图.BarPage.EffectPage1
{ 
    public sealed partial class ChromaKey : Page
    {
        public ChromaKey()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            ChromaKeySlider.Value  = (int)(App.Setting.Tolerance * 100);
            ChromaKeySlider.ValueChanged += ChromaKeySlider_ValueChanged;
 
            Render();


            // 0 
            if (App.Setting.InvertAlpha == false) InvertAlpha0.IsChecked = false;
            else InvertAlpha0.IsChecked = true;

            // 1 
            if (App.Setting.InvertAlpha == true) InvertAlpha1.IsChecked = false;
            else InvertAlpha1.IsChecked = true;
        }

        #endregion


        #region ChromaKey：消除颜色


        private void ChromaKeySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Tolerance = (float)e.NewValue / 100;

            Render();
        }


        private void ColorButton_ColorChanged(Color Color)
        {
            Render(); 
        } 



        private void FeatherCheck_Loaded(object sender, RoutedEventArgs e)
        {
            FeatherCheck.Checked += FeatherCheck_Checked;
            FeatherCheck.Unchecked += FeatherCheck_Unchecked;
        }
        private void FeatherCheck_Checked(object sender, RoutedEventArgs e)
        {
            App.Setting.ChromaKeyFeather = true;
            Render();
        } 
        private void FeatherCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Setting.ChromaKeyFeather = false;
            Render();
        }



        private void InvertAlpha_Click(object sender, RoutedEventArgs e)
        {
             ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == InvertAlpha0)
            {
                InvertAlpha0.IsChecked = false;
                App.Setting.InvertAlpha = false;
            }
            else InvertAlpha0.IsChecked = true;

            // 1
            if (tb == InvertAlpha1)
            {
                InvertAlpha1.IsChecked = false;
                App.Setting.InvertAlpha = true;
            }
            else InvertAlpha1.IsChecked = true;
            Render();
        }

        #endregion

        public static void Render()
        {
            //ChromaKey：消除颜色
            App.Model.SecondCanvasImage = Adjust.GetChromaKey(App.Model.SecondSourceRenderTarget, App.Setting.Tolerance, App.Model.ChromaKeyColor, App.Setting.ChromaKeyFeather, App.Setting.InvertAlpha);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

    }
}
