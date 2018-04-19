using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Brushes;
using 修图.Library;
using Windows.UI;

namespace 修图.BarPage.EffectPage3
{ 
    public sealed partial class PinchPunch : Page
    {
         static Transform2DEffect DisScale;
        static CanvasRenderTarget DisTarget;

        public PinchPunch()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var Dis = Library.Image.Displacement(400, 400); 
            DisTarget = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);

            Slider.ValueChanged += Slider_ValueChanged;
            NumberPicker.ValueChange += NumberPicker_ValueChange;
        }


        #endregion


        #region PinchPunch：膨胀收缩


        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.PinchPunchAmount = (float)e.NewValue;
            NumberPicker.Value = (int)App.Setting.PinchPunchAmount;
            Render();
        }

        private void NumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PinchPunchAmount = (float)Value;
             Slider.Value = App.Setting.PinchPunchAmount;
            Render();
        }


        #endregion


        public static void Render()
        {
            if (App.Setting.PinchPunchRadius < 0) App.Setting.PinchPunchRadius = 1;
            else if (App.Setting.PinchPunchRadius > 1000) App.Setting.PinchPunchRadius = 1000;

            //偏移
            float W = (float)App.Model.PunchDisplacement.Size.Width;
            float H = (float)App.Model.PunchDisplacement.Size.Height;
             Matrix3x2 m = Matrix3x2.CreateTranslation(-W/ 2, -H/ 2)
                    * Matrix3x2.CreateScale(App.Setting.PinchPunchRadius / W* 2, App.Setting.PinchPunchRadius / H* 2)
            * Matrix3x2.CreateTranslation(App.Model.Width / 2, App.Model.Height / 2);

            //判断
            if (App.Setting.PinchPunchAmount > 0) DisScale = new Transform2DEffect { Source = App.Model.PunchDisplacement, TransformMatrix = m, };
            else DisScale = new Transform2DEffect { Source = App.Model.PinchDisplacement, TransformMatrix = m, };

            //绘画
            using (var ds = DisTarget.CreateDrawingSession())
            {
                ds.Clear(App.Setting.LiquifyColor);
                ds.DrawImage(DisScale);
            }

            //置换贴图
            App.Model.SecondCanvasImage = Adjust.GetDisplacementMap(App.Model.SecondSourceRenderTarget, DisTarget, Math.Abs(App.Setting.PinchPunchAmount));

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
    }
}
