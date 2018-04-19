using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using 修图.Library;
using System.Numerics;
using Windows.UI;

namespace 修图.BarPage.EffectPage1
{
    public sealed partial class Lighting : Page
    {

      public static  LuminanceToAlphaEffect lta;

        public Lighting()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件 
            lta = new LuminanceToAlphaEffect
            {
                Source = App.Model.SecondSourceRenderTarget,
            };
            Render();

        }

        #endregion


        #region Lighting：暗室灯光


        private void ColorPicker_ColorChanged(Windows.UI.Color Color, Windows.UI.Xaml.Media.SolidColorBrush Brush)
        {
            App.Setting.LightColor = Color;
            Render();
        }
        private void Slider1_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.Light1 = (float)e.NewValue;
            Render();
        }

        private void Slider4_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.LightAngle = (float)(e.NewValue/180*Math.PI);
            Render();
        }



        #endregion


        public static void Render()
        {

            SpotDiffuseEffect sde = new SpotDiffuseEffect//运动传播特效
            {
                Source = lta,
                LightColor = App.Setting.LightColor,
                
                DiffuseAmount = App.Setting.Light1, 
            };

            SpotSpecularEffect sse = new SpotSpecularEffect//运动镜像特效
            {
                Source = lta, 
            }; 

            sde.HeightMapScale = sse.HeightMapScale = App.Setting.Light3;
            sde.Focus = sse.Focus = App.Setting.LightFocus;
            sde.LightColor = sse.LightColor = App.Setting.LightColor;
            sde.LimitingConeAngle = sse.LimitingConeAngle = App.Setting.LightAngle;
            sde.LightTarget = sse.LightTarget = new Vector3((float)App.Setting.LightingStartPoint.X, (float)App.Setting.LightingStartPoint.Y, 0);
            sde.LightPosition = sse.LightPosition = new Vector3((float)App.Setting.LightingEndPoint.X, (float)App.Setting.LightingEndPoint.Y, 0);



            App.Model.SecondCanvasImage = new ArithmeticCompositeEffect
            {
                Source1 = new ArithmeticCompositeEffect
                {
                    Source1 = App.Model.SecondSourceRenderTarget,
                    Source2 = sde,
                    Source1Amount = 0,
                    Source2Amount = 0,
                    MultiplyAmount = 1,
                },
                Source2 = sse,
                Source1Amount = 1,
                Source2Amount = 1,
                MultiplyAmount = 0,
            };


            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }


    }
}
