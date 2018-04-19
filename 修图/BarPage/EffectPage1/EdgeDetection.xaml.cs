using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;

namespace 修图.BarPage.EffectPage1
{
    public sealed partial class EdgeDetection : Page
    {
        public EdgeDetection()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局      

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            AmountSlider.Value = AmountNumberPicker.Value = (int)(App.Setting.EdgeAmount * 100);
            AmountSlider.ValueChanged += AmountSlider_ValueChanged;
            AmountNumberPicker.ValueChange += AmountNumberPicker_ValueChange;

            BlurAmountSlider.Value = BlurAmountNumberPicker.Value = (int)(App.Setting.EdgeBlurAmount * 10);
            BlurAmountSlider.ValueChanged += BlurAmountSlider_ValueChanged;
            BlurAmountNumberPicker.ValueChange += BlurAmountNumberPicker_ValueChange;
             

            // 0
            if (App.Setting.OverlayEdges == false) OverlayEdges0.IsChecked = false;
            else OverlayEdges0.IsChecked = true;
            // 1
            if (App.Setting.OverlayEdges == true) OverlayEdges1.IsChecked = false;
            else OverlayEdges1.IsChecked = true;


            // 0
            if (App.Setting.EdgeMode == EdgeDetectionEffectMode.Sobel)EffectMode0.IsChecked = false;
            else EffectMode0.IsChecked = true;
            // 1       
            if (App.Setting.EdgeMode == EdgeDetectionEffectMode.Prewitt) EffectMode1.IsChecked = false;
            else EffectMode1.IsChecked = true;


            // 0
            if (App.Setting.EdgeAlphaMode == CanvasAlphaMode.Premultiplied)AlphaMode0.IsChecked = false;
            else AlphaMode0.IsChecked = true;
            // 1
            if (App.Setting.EdgeAlphaMode == CanvasAlphaMode.Straight) AlphaMode1.IsChecked = false;
            else AlphaMode1.IsChecked = true; 

            Render();
        }



        #endregion


        #region EdgeDetection：边缘检测


        private void AmountSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            AmountNumberPicker.Value = (int)e.NewValue;
            App.Setting.EdgeAmount = (float)e.NewValue / 100;

            Render();
        }
        private void AmountNumberPicker_ValueChange(object sender, int Value)
        {
            AmountSlider.Value = Value;
            App.Setting.EdgeAmount = (float)Value / 100;

            Render();
        }




        private void BlurAmountSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BlurAmountNumberPicker.Value = (int)e.NewValue;
            App.Setting.EdgeBlurAmount = (float)e.NewValue / 10;

            Render();
        }
        private void BlurAmountNumberPicker_ValueChange(object sender, int Value)
        {
            BlurAmountSlider.Value = Value;
            App.Setting.EdgeBlurAmount = (float)Value / 10;

            Render();
        }





        private void OverlayEdges_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == OverlayEdges0)
            {
                OverlayEdges0.IsChecked = false;
                App.Setting.OverlayEdges = false;
            }
            else OverlayEdges0.IsChecked = true;

            // 1
            if (tb == OverlayEdges1)
            {
                OverlayEdges1.IsChecked = false;
                App.Setting.OverlayEdges = true;
            }
            else OverlayEdges1.IsChecked = true;

            Render();
        }










        private void EffectMode_Click(object sender, RoutedEventArgs e)
        {

            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == EffectMode0)
            {
                EffectMode0.IsChecked = false;
                App.Setting.EdgeMode = EdgeDetectionEffectMode.Sobel;//索贝尔
            }
            else EffectMode0.IsChecked = true;

            // 1
            if (tb == EffectMode1)
            {
                EffectMode1.IsChecked = false;
                App.Setting.EdgeMode = EdgeDetectionEffectMode.Prewitt;//普瑞维特
            }
            else EffectMode1.IsChecked = true;

            Render();
        }






        private void AlphaMode_Click(object sender, RoutedEventArgs e)
        {

            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == AlphaMode0)
            {
                AlphaMode0.IsChecked = false;
                App.Setting.EdgeAlphaMode = CanvasAlphaMode.Premultiplied;//自左乘
            }
            else AlphaMode0.IsChecked = true;

            // 1
            if (tb == AlphaMode1)
            {
                AlphaMode1.IsChecked = false;
                App.Setting.EdgeAlphaMode = CanvasAlphaMode.Straight;//直
            }
            else AlphaMode1.IsChecked = true;

            Render();
        }
        #endregion


        private void Render()
        {
            //EdgeDetection：边缘检测
            App.Model.SecondCanvasImage = Adjust.GetEdgeDetection(App.Model.SecondSourceRenderTarget, App.Setting.EdgeAmount, App.Setting.EdgeBlurAmount, App.Setting.EdgeMode, App.Setting.EdgeAlphaMode, App.Setting.OverlayEdges);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

    }
}
