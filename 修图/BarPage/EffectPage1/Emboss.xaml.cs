using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using 修图.Library;

namespace 修图.BarPage.EffectPage1
{
    public sealed partial class Emboss : Page
    {
        public Emboss()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }

        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            RotationPicker.Angle = App.Setting.EmbossAngle * 2;
            EmbossSlider.Value = EmbossNumberPicker.Value = (int)App.Setting.EmbossAmount*10;
            EmbossSlider.ValueChanged += EmbossSlider_ValueChanged;
            EmbossNumberPicker.ValueChange += EmbossNumberPicker_ValueChange;

            Render(); 
           }

        #endregion


        #region Emboss：浮雕


        private void RotationPicker_AngleChange(object sender, double Angle, double Rotation)
        {
            App.Setting.EmbossAngle = (float)Angle;

            Render();
        }




        private void EmbossSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            EmbossNumberPicker.Value = (int)e.NewValue;
            App.Setting.EmbossAmount = (float)e.NewValue/10;

            Render();
        }
        private void EmbossNumberPicker_ValueChange(object sender, int Value)
        {
            EmbossSlider.Value = Value;
            App.Setting.EmbossAmount = (float)Value/10;

            Render();
        }



        
        #endregion


        private void Render()
        {
            //Emboss：浮雕
            App.Model.SecondCanvasImage = Adjust.GetEmboss(App.Model.SecondSourceRenderTarget, App.Setting.EmbossAmount, App.Setting.EmbossAngle);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }



    }
}
