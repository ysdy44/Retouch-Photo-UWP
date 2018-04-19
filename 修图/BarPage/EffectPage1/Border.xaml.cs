using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Graphics.Imaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
using Windows.Foundation;
using 修图.Library;

namespace 修图.BarPage.EffectPage1
{
    public sealed partial class Border : Page
    {
        ICanvasEffect Source;

        public Border()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;


            App.Judge();//判断选区

            Source = new CropEffect
            {
                Source = App.Model.SecondSourceRenderTarget,

                SourceRectangle = App.Model.isAnimated == false ?
               App.GetBounds(App.Model.SecondSourceRenderTarget.GetPixelColors(), App.Model.Width, App.Model.Height) ://当前选区的边界
               App.Model.MaskRect//选区边界
            };
        }

        #region Global：全局 


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件
            // 0 
            if (App.Setting.EdgeBehaviorX == CanvasEdgeBehavior.Clamp) EdgeBehaviorX0.IsChecked = false;
            else EdgeBehaviorX0.IsChecked = true;
            // 1 
            if (App.Setting.EdgeBehaviorX == CanvasEdgeBehavior.Wrap) EdgeBehaviorX1.IsChecked = false;
            else EdgeBehaviorX1.IsChecked = true;
            // 2
            if (App.Setting.EdgeBehaviorX == CanvasEdgeBehavior.Mirror) EdgeBehaviorX2.IsChecked = false;
            else EdgeBehaviorX2.IsChecked = true;

            // 0 
            if (App.Setting.EdgeBehaviorY == CanvasEdgeBehavior.Clamp) EdgeBehaviorY0.IsChecked = false;
            else EdgeBehaviorY0.IsChecked = true;
            // 1 
            if (App.Setting.EdgeBehaviorY == CanvasEdgeBehavior.Wrap) EdgeBehaviorY1.IsChecked = false;
            else EdgeBehaviorY1.IsChecked = true;
            // 2
            if (App.Setting.EdgeBehaviorY == CanvasEdgeBehavior.Mirror) EdgeBehaviorY2.IsChecked = false;
            else EdgeBehaviorY2.IsChecked = true;


            Render();
        }



        #endregion


        #region Border：边界


        private void EdgeBehaviorX_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == EdgeBehaviorX0)
            {
                EdgeBehaviorX0.IsChecked = false;
                App.Setting.EdgeBehaviorX =  CanvasEdgeBehavior.Clamp;//边界
                if (App.Model.BottomVisibility==Visibility.Collapsed) App.Setting.EdgeBehaviorY = CanvasEdgeBehavior.Clamp;
            }
            else EdgeBehaviorX0.IsChecked = true;

            // 1
            if (tb == EdgeBehaviorX1)
            {
                EdgeBehaviorX1.IsChecked = false;
                App.Setting.EdgeBehaviorX = CanvasEdgeBehavior.Wrap;//排列
                if (App.Model.BottomVisibility == Visibility.Collapsed) App.Setting.EdgeBehaviorY = CanvasEdgeBehavior.Wrap;
            }
            else EdgeBehaviorX1.IsChecked = true;

            // 2
            if (tb == EdgeBehaviorX2)
            {
                EdgeBehaviorX2.IsChecked = false;
                App.Setting.EdgeBehaviorX = CanvasEdgeBehavior.Mirror;//对称
                if (App.Model.BottomVisibility == Visibility.Collapsed) App.Setting.EdgeBehaviorY = CanvasEdgeBehavior.Mirror;
            }
            else EdgeBehaviorX2.IsChecked = true;

            Render();
        }





        private void EdgeBehaviorY_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == EdgeBehaviorY0)
            {
                EdgeBehaviorY0.IsChecked = false;
                App.Setting.EdgeBehaviorY = CanvasEdgeBehavior.Clamp;
            }
            else EdgeBehaviorY0.IsChecked = true;

            // 1
            if (tb == EdgeBehaviorY1)
            {
                EdgeBehaviorY1.IsChecked = false;
               App.Setting.EdgeBehaviorY = CanvasEdgeBehavior.Wrap;
            }
            else EdgeBehaviorY1.IsChecked = true;

            // 2
            if (tb == EdgeBehaviorY2)
            {
                EdgeBehaviorY2.IsChecked = false;
                App.Setting.EdgeBehaviorY = CanvasEdgeBehavior.Mirror;
            }
            else EdgeBehaviorY2.IsChecked = true;

            Render();
        }


        #endregion


        private void Render()
        {
            // Border：边界
            App.Model.SecondCanvasImage = Adjust.GetChromaKey(Source, App.Setting.EdgeBehaviorX, App.Setting.EdgeBehaviorY);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }


    }
}
