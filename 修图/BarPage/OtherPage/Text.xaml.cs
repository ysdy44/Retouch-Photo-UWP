using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.Geometry;
using 修图.Model;

namespace 修图.BarPage.OtherPage
{
    public sealed partial class Text : Page
    {
        public Text()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
            
        }


        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //控件事件    
            TextBox.Text = App.Setting.Text;
            NumberPicker.Number = (int)App.Setting.TextFormat.FontSize;

            // 0
            if (App.Setting.TextFormat.HorizontalAlignment == CanvasHorizontalAlignment.Left) HorizontalButton0.IsChecked = false;
            else HorizontalButton0.IsChecked = true;
            // 1
            if (App.Setting.TextFormat.HorizontalAlignment == CanvasHorizontalAlignment.Center) HorizontalButton1.IsChecked = false;
            else HorizontalButton1.IsChecked = true;
            // 2
            if (App.Setting.TextFormat.HorizontalAlignment == CanvasHorizontalAlignment.Right) HorizontalButton2.IsChecked = false;
            else HorizontalButton2.IsChecked = true;
            // 3
            if (App.Setting.TextFormat.HorizontalAlignment == CanvasHorizontalAlignment.Justified) HorizontalButton3.IsChecked = false;
            else HorizontalButton3.IsChecked = true;

            // 0
            if (App.Setting.TextFormat.VerticalAlignment == CanvasVerticalAlignment.Top) VerticalButton0.IsChecked = false;
            else VerticalButton0.IsChecked = true;
            // 1
            if (App.Setting.TextFormat.VerticalAlignment == CanvasVerticalAlignment.Center) VerticalButton1.IsChecked = false;
            else VerticalButton1.IsChecked = true;
            // 2
            if (App.Setting.TextFormat.VerticalAlignment == CanvasVerticalAlignment.Bottom) VerticalButton2.IsChecked = false;
            else VerticalButton2.IsChecked = true;
 
             // 0
            if (App.Setting.TextFormat.FontStyle == FontStyle.Normal) FontStyleButton0.IsChecked = false;
            else FontStyleButton0.IsChecked = true;
            // 1
            if (App.Setting.TextFormat.FontStyle == FontStyle.Oblique) FontStyleButton1.IsChecked = false;
            else FontStyleButton1.IsChecked = true;


            if (App.Setting.isUnderline == true)
                UnderLineButton.IsChecked = false;
            else
                UnderLineButton.IsChecked = true;


            Render();
        }
         


        private void HorizontalButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == HorizontalButton0)
            {
                HorizontalButton0.IsChecked = false;
                App.Setting.TextFormat.HorizontalAlignment = CanvasHorizontalAlignment.Left;
            }
            else HorizontalButton0.IsChecked = true;


            // 1
            if (tb == HorizontalButton1)
            {
                HorizontalButton1.IsChecked = false;
                App.Setting.TextFormat.HorizontalAlignment = CanvasHorizontalAlignment.Center;
            }
            else
            {
                HorizontalButton1.IsChecked = true;
            }


            // 2
            if (tb == HorizontalButton2)
            {
                HorizontalButton2.IsChecked = false;
                App.Setting.TextFormat.HorizontalAlignment = CanvasHorizontalAlignment.Right;
            }
            else
            {
                HorizontalButton2.IsChecked = true;
            }

            // 3
            if (tb == HorizontalButton3)
            {
                HorizontalButton3.IsChecked = false;
                App.Setting.TextFormat.HorizontalAlignment = CanvasHorizontalAlignment.Justified;
            }
            else
            {
                HorizontalButton3.IsChecked = true;
            }

            Render(); 
        }
         

        private void VerticalButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == VerticalButton0)
            {
                VerticalButton0.IsChecked = false;
                App.Setting.TextFormat.VerticalAlignment = CanvasVerticalAlignment.Top;
            }
            else VerticalButton0.IsChecked = true;


            // 1
            if (tb == VerticalButton1)
            {
                VerticalButton1.IsChecked = false;
                App.Setting.TextFormat.VerticalAlignment = CanvasVerticalAlignment.Center;
            }
            else
            {
                VerticalButton1.IsChecked = true;
            }


            // 2
            if (tb == VerticalButton2)
            {
                VerticalButton2.IsChecked = false;
                App.Setting.TextFormat.VerticalAlignment = CanvasVerticalAlignment.Bottom;
            }
            else
            {
                VerticalButton2.IsChecked = true;
            }
              
            Render();
        }





        private void FontStyleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == FontStyleButton0)
            {
                FontStyleButton0.IsChecked = false;
                App.Setting.TextFormat.FontStyle = FontStyle.Normal;
            }
            else FontStyleButton0.IsChecked = true;


            // 1
            if (tb == FontStyleButton1)
            {
                FontStyleButton1.IsChecked = false;
                App.Setting.TextFormat.FontStyle = FontStyle.Oblique;
            }
            else  FontStyleButton1.IsChecked = true; 

 
            Render();
        }


        private void UnderLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (UnderLineButton.IsChecked==false)
                App.Setting.isUnderline = true;
            else
                App.Setting.isUnderline = false;

            Render();
        }




        #endregion


        #region Text：文字


        private void ColorButton_ColorChanged(Color Color)
        {
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        private void NumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.TextFormat.FontSize = Value;

            Render();
        }

        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Setting.Text = TextBox.Text;

            Render();
        }

        private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            App.Setting.Text = TextBox.Text;

            Render();
        }

        private void FontPicker_FontChange(object sender, string FontFamily)
        { 
             App.Setting.TextFormat.FontFamily = FontFamily;

            Render();
        }

        #endregion


        public static void Render( )
        { 
                 App.Setting.TextLayout = new CanvasTextLayout(
                    App.Model.VirtualControl,
                    App.Setting.Text,
                    App.Setting.TextFormat,
                    App.Setting.TextW,
                    App.Setting.TextH);

           if( App.Setting.isUnderline ==true) App.Setting.TextLayout.SetUnderline(0, 1000, true);

            if (App.Setting.TextLayout!=null)  App.Model.SecondCanvasGeometry = CanvasGeometry.CreateText(App.Setting.TextLayout);

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        public static void Apply()
        {
            App.Model.isCanUndo = false;//关闭撤销
            
            var index = App.Model.Index;
            if (App.Model.Index < 0) App.Model.Index = 0;//图层索引

            //新建渲染目标=>层
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (CanvasDrawingSession ds = crt.CreateDrawingSession())
            {
                ds.FillGeometry(App.Model.SecondCanvasGeometry, App.Setting.TextX, App.Setting.TextY, App.Model.TextColor);
            }
            Layer l = new Layer { Visual = true, Opacity = 100, CanvasRenderTarget = crt, };
            if (App.Model.isLowView) l.LowView();
            else l.SquareView();
            l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图

            //Undo：撤销
            Undo undo = new Undo();
            undo.AddInstantiation(index);
            App.UndoAdd(undo);

            App.Model.Layers.Insert(index, l);
            App.Model.Index = index;

            App.Model.LayersCount = App.Model.Layers.Count;//更新图层数量以供判断集合撤销
            App.Model.isCanUndo = true;//打开撤销
              }

        }
}
