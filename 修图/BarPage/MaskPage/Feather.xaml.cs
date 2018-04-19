using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;
using System;
using 修图.Model;

namespace 修图.BarPage.MaskPage
{ 
    public sealed partial class Feather : Page
    {
        public Feather()
        {
            this.InitializeComponent();
        }

        #region Global：全局


        //页面
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.Setting.Feather = 0;
            App.Setting.Border = 0;

            FeatherSlider.Value = FeatherNumberPicker.Value = (int)App.Setting.Feather;
            BorderSlider.Value = BorderNumberPicker.Value = App.Setting.Border;

            if (App.Setting.isBorder == false)
            {
                FeatherButton.IsChecked = false;
                FeatherGrid.Visibility = Visibility.Visible;
            }
            else
            {
                FeatherButton.IsChecked = true;
                FeatherGrid.Visibility = Visibility.Collapsed;
            }

            if (App.Setting.isBorder == true)
            {
                BorderButton.IsChecked = false;
                BorderGrid.Visibility = Visibility.Visible;
            }
            else
            {
                BorderButton.IsChecked = true;
                BorderGrid.Visibility = Visibility.Collapsed;
            }


            Render();
        }


        private void FeatherBorderButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == FeatherButton)
            {
                FeatherButton.IsChecked = false;
                FeatherGrid.Visibility = Visibility.Visible;

                App.Setting.isBorder = false;
            }
            else
            {
                FeatherButton.IsChecked = true;
                FeatherGrid.Visibility = Visibility.Collapsed;
            }


            // 1
            if (tb == BorderButton)
            {
                BorderButton.IsChecked = false;
                BorderGrid.Visibility = Visibility.Visible;

                App.Setting.isBorder = true;
            }
            else
            {
                BorderButton.IsChecked = true;
                BorderGrid.Visibility = Visibility.Collapsed;
            }

            Render();
        }


        #endregion


        #region Feather：羽化


        //羽化
        private void FeatherSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Feather = (float)e.NewValue;
            FeatherNumberPicker.Value = (int)e.NewValue;

            Render();
        }
        private void FeatherNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Feather = Value;
            FeatherSlider.Value = Value;

            Render();
        }


        //扩张
        private void BorderSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Border = (int)e.NewValue;
            BorderNumberPicker.Value = (int)e.NewValue;

            Render();
        }
        private void BorderNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.Border = Value;
            BorderSlider.Value = Value;

            Render();
        }



        #endregion


        public static void Render()
        {
            if (App.Setting.isBorder == false) App.Model.SecondCanvasImage = Adjust.GetFeather(App.Model.MaskRenderTarget, App.Setting.Feather);
            else App.Model.SecondCanvasImage = Adjust.GetBorder(App.Model.MaskRenderTarget, App.Setting.Border);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        public static void Apply()
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            //画选区（临时征用底选区渲染目标）
            using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(App.Model.SecondCanvasImage);
            }

            //复制
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.AnimatedControl, App.Model.Width, App.Model.Height);
            crt.SetPixelBytes(App.Model.SecondBottomRenderTarget.GetPixelBytes());

            App.Mask(App.Model.SecondBottomRenderTarget, crt);
        }

    }
}
