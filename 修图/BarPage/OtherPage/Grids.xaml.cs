using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;
using 修图.Library;
using System.Numerics;
using 修图.Model;

namespace 修图.BarPage.OtherPage
{
    public sealed partial class Grids : Page
    {
        public Grids()
        {
            this.InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.GridsSpace = (float)e.NewValue;
            Render();
        }

        private void ColorButton_ColorChanged(Color Color)
        {
            Render();
        }



        public static void Render()
        {
            using (CanvasDrawingSession ds = App.Model.SecondSourceRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                //左上右下的位置
                float LineL = 0;
                float LineT = 0;
                float LineR = App.Model.Width;
                float LineB = App.Model.Height;


                //线循环
                for (float X = LineL; X < LineR; X += App.Setting.GridsSpace)
                {
                    float xx = LineL + X;
                    ds.DrawLine(xx, LineT, xx, LineB, App.Model.GridsColor);
                }
                for (float Y = LineT; Y < LineB; Y += App.Setting.GridsSpace)
                {
                    float yy = LineT + Y;
                    ds.DrawLine(LineL, yy, LineR, yy, App.Model.GridsColor);
                }
            }

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        public static void Apply()
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);

            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                //左上右下的位置
                float LineL = 0;
                float LineT = 0;
                float LineR = App.Model.Width;
                float LineB = App.Model.Height;


                //线循环
                for (float X = LineL; X < LineR; X += App.Setting.GridsSpace)
                {
                    float xx = LineL + X;
                    ds.DrawLine(xx, LineT, xx, LineB, App.Model.GridsColor);
                }
                for (float Y = LineT; Y < LineB; Y += App.Setting.GridsSpace)
                {
                    float yy = LineT + Y;
                    ds.DrawLine(LineL, yy, LineR, yy, App.Model.GridsColor);
                }
            }
        }
    }

}
