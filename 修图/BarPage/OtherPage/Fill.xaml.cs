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

namespace 修图.BarPage.OtherPage
{
    public sealed partial class Fill : Page
    {
        public Fill()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.Judge();//判断选区，改变是否动画与选区矩形
            
            // 0 
            if (App.Setting.FillMode == 0)   FillButton.IsChecked = false;
            else FillButton.IsChecked = true;

            // 1  
            if (App.Setting.FillMode == 1) StrokeButton.IsChecked = false;
            else StrokeButton.IsChecked = true;

            // 1  
            if (App.Setting.FillMode == 2) TurbulenceButton.IsChecked = false;
            else TurbulenceButton.IsChecked = true;

            Render();
        }


        private void ColorButton_ColorChanged(Color Color)
        {
            Render();
        }


        #endregion


        #region Fill：填充


        private void FiilStroke_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb ==FillButton)
            {
                FillButton.IsChecked = false;

                App.Setting.FillMode = 0;
            }
            else FillButton.IsChecked = true;

            // 1
            if (tb == StrokeButton)
            {
                StrokeButton.IsChecked = false;

                App.Setting.FillMode = 1;
            }
            else StrokeButton.IsChecked = true;

            // 2
            if (tb == TurbulenceButton)
            {
                TurbulenceButton.IsChecked = false;

                App.Setting.FillMode = 2;
            }
            else TurbulenceButton.IsChecked = true;
            
            Render();
        }


        #endregion


        public static void Render()
        {
            using (CanvasDrawingSession ds = App.Model.SecondSourceRenderTarget.CreateDrawingSession())
            {
                ds.Clear(App.Model.FillColor);

                if (App.Setting.FillMode == 0)//填充模式
                {
                    if (App.Model.isAnimated == true)//如果选区存在，就扣掉选区
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationIn);
                }
                else if (App.Setting.FillMode == 1)//描边模式
                {
                    if (App.Model.isAnimated == true)//如果选区存在，就描边
                        ds.DrawImage(new LuminanceToAlphaEffect//亮度转不透明度
                        {
                            Source = new EdgeDetectionEffect//边缘检测
                            {
                                Amount = 1,
                                Source = App.Model.MaskRenderTarget
                            },
                        }, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationIn);
                    else
                        ds.DrawImage(new LuminanceToAlphaEffect//亮度转不透明度
                        {
                            Source = new EdgeDetectionEffect//边缘检测
                            {
                                Amount = 1,
                                Source = App.GrayWhiteGrid,
                            },
                        }, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationIn);
                }
                else if (App.Setting.FillMode == 2)//杂色模式
                {
                    ds.DrawImage( new TurbulenceEffect//画柏林噪声
                          {
                              Octaves = 4,
                              Size = new Vector2(App.Model.Width, App.Model.Height)
                          }, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationIn);

                    if (App.Model.isAnimated == true)//如果选区存在，就扣掉选区
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationIn);
                }
            }

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

    }
}
