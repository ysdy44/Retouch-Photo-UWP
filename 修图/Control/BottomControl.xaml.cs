using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.Resources;

namespace 修图.Control
{
    public sealed partial class BottomControl : UserControl
    {

        #region DependencyProperty：依赖属性

        //工具
        public int Tool
        {
            get { return (int)GetValue(ToolProperty); }
            set { SetValue(ToolProperty, value); }
        }

        public static readonly DependencyProperty ToolProperty =
            DependencyProperty.Register("Tool", typeof(int), typeof(BottomControl), new PropertyMetadata(0, new PropertyChangedCallback(ToolOnChang)));

        private static void ToolOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BottomControl Con = (BottomControl)sender;
            Con.Follow((int)e.NewValue);
         }

        #endregion


 
        public BottomControl()
        {
            this.InitializeComponent();
            Follow(Tool);
        }


        private void Follow(int Tool)
        {
            switch (Tool)
            {

                //Paint：绘画 
                case -4: BottomFrame.Navigate(typeof(BarPage.ToolPage.Paint));  App.Model.Text = string.Empty; break;

                    //Liquify：液化 
                case -3: BottomFrame.Navigate(typeof(BarPage.LiquifyBarPage));  App.Model.Text = App.resourceLoader.GetString("/Main/Liquify_"); break;

                //Adjust：调整 
                case -2: BottomFrame.Navigate(typeof(BarPage.AdjustBarPage));  App.Model.Text = App.resourceLoader.GetString("/Main/Adjust_"); break;



                //Tool：工具
                case 0: BottomFrame.Navigate(typeof(BarPage.ToolPage.Hand)); App.Tip(App.resourceLoader.GetString("/Tool/Hand_")); break;
                case 1: BottomFrame.Navigate(typeof(BarPage.ToolPage.Cursor)); App.Tip(App.resourceLoader.GetString("/Tool/Cursor_")); break;
                case 2: BottomFrame.Navigate(typeof(BarPage.ToolPage.Straw)); App.Tip(App.resourceLoader.GetString("/Tool/Straw_")); break;
                case 3: BottomFrame.Navigate(typeof(BarPage.ToolPage.Magic)); App.Tip(App.resourceLoader.GetString("/Tool/Magic_")); break;

                case 4: BottomFrame.Navigate(typeof(BarPage.ToolPage.Rectangle)); App.Tip(App.resourceLoader.GetString("/Tool/Rectangle_")); break;
                case 5: BottomFrame.Navigate(typeof(BarPage.ToolPage.Ellipse)); App.Tip(App.resourceLoader.GetString("/Tool/Ellipse_")); break;
                case 6: BottomFrame.Navigate(typeof(BarPage.ToolPage.Polygon)); App.Tip(App.resourceLoader.GetString("/Tool/Polygon_")); break;
                case 7: BottomFrame.Navigate(typeof(BarPage.ToolPage.Lasso)); App.Tip(App.resourceLoader.GetString("/Tool/Lasso_")); break;

                case 8: BottomFrame.Navigate(typeof(BarPage.ToolPage.MaskPaint)); App.Tip(App.resourceLoader.GetString("/Tool/MaskPaint_")); break;
                case 9: BottomFrame.Navigate(typeof(BarPage.ToolPage.MaskEraser)); App.Tip(App.resourceLoader.GetString("/Tool/MaskEraser_")); break;
                case 10: BottomFrame.Navigate(typeof(BarPage.ToolPage.Smudge)); App.Tip(App.resourceLoader.GetString("/Tool/Smudge_")); break;
                case 11: BottomFrame.Navigate(typeof(BarPage.ToolPage.Mixer)); App.Tip(App.resourceLoader.GetString("/Tool/Mixer_")); break;

                case 12: BottomFrame.Navigate(typeof(BarPage.ToolPage.Paint)); App.Tip(App.resourceLoader.GetString("/Tool/Paint_")); break;
                case 13: BottomFrame.Navigate(typeof(BarPage.ToolPage.Pencil)); App.Tip(App.resourceLoader.GetString("/Tool/Pencil_")); break;
                case 14: BottomFrame.Navigate(typeof(BarPage.ToolPage.Pen)); App.Tip(App.resourceLoader.GetString("/Tool/Pen_")); break;
                case 15: BottomFrame.Navigate(typeof(BarPage.ToolPage.Eraser)); App.Tip(App.resourceLoader.GetString("/Tool/Eraser_")); break;



                //Mask：选区
                case 102: BottomFrame.Navigate(typeof(BarPage.OtherPage.Transform));  App.Model.Text = App.resourceLoader.GetString("/Mask/Paste_"); break;
                case 110: BottomFrame.Navigate(typeof(BarPage.MaskPage.Feather));  App.Model.Text = App.resourceLoader.GetString("/Mask/Feather_"); break;
                case 111: BottomFrame.Navigate(typeof(BarPage.OtherPage.Transform)); App.Model.Text = App.resourceLoader.GetString("/Mask/Transform_"); break;


                //Effect：特效
                case 200: BottomFrame.Navigate(typeof(BarPage.EffectPage.Exposure));  App.Model.Text = App.resourceLoader.GetString("/Effect0/Exposure_"); break;
                case 201: BottomFrame.Navigate(typeof(BarPage.EffectPage.Brightness));  App.Model.Text = App.resourceLoader.GetString("/Effect0/Brightness_"); break;
                case 202: BottomFrame.Navigate(typeof(BarPage.EffectPage.Saturation)); App.Model.Text = App.resourceLoader.GetString("/Effect0/Saturation_"); break;
                case 203: BottomFrame.Navigate(typeof(BarPage.EffectPage.HueRotation));  App.Model.Text = App.resourceLoader.GetString("/Effect0/HueRotation_"); break;
                case 204: BottomFrame.Navigate(typeof(BarPage.EffectPage.Contrast)); App.Model.Text = App.resourceLoader.GetString("/Effect0/Contrast_"); break;
                case 205: BottomFrame.Navigate(typeof(BarPage.EffectPage.Temperature));  App.Model.Text = App.resourceLoader.GetString("/Effect0/Temperature_"); break;
                case 206: BottomFrame.Navigate(typeof(BarPage.EffectPage.HighlightsAndShadows)); App.Model.Text = App.resourceLoader.GetString("/Effect0/HighlightsAndShadows_"); break;

                case 210: BottomFrame.Navigate(typeof(BarPage.EffectPage1.GaussianBlur)); App.Model.Text = App.resourceLoader.GetString("/Effect1/GaussianBlur_"); break;
                case 211: BottomFrame.Navigate(typeof(BarPage.EffectPage1.DirectionalBlur)); App.Model.Text = App.resourceLoader.GetString("/Effect1/DirectionalBlur_"); break;
                case 212: BottomFrame.Navigate(typeof(BarPage.EffectPage1.Sharpen));  App.Model.Text = App.resourceLoader.GetString("/Effect1/Sharpen_"); break;
                case 213: BottomFrame.Navigate(typeof(BarPage.EffectPage1.Shadow));  App.Model.Text = App.resourceLoader.GetString("/Effect1/Shadow_"); break;
                case 214: BottomFrame.Navigate(typeof(BarPage.EffectPage1.ChromaKey)); App.Model.Text = App.resourceLoader.GetString("/Effect1/ChromaKey_"); break;
                case 215: BottomFrame.Navigate(typeof(BarPage.EffectPage1.EdgeDetection));  App.Model.Text = App.resourceLoader.GetString("/Effect1/EdgeDetection_"); break;
                case 216: BottomFrame.Navigate(typeof(BarPage.EffectPage1.Border));  App.Model.Text = App.resourceLoader.GetString("/Effect1/Border_"); break;
                case 217: BottomFrame.Navigate(typeof(BarPage.EffectPage1.Emboss)); App.Model.Text = App.resourceLoader.GetString("/Effect1/Emboss_"); break;
                case 218: BottomFrame.Navigate(typeof(BarPage.EffectPage1.Lighting));  App.Model.Text = App.resourceLoader.GetString("/Effect1/Emboss_"); break;

                case 220: BottomFrame.Navigate(typeof(BarPage.EffectPage2.Colouring)); App.Model.Text = App.resourceLoader.GetString("/Effect2/Colouring_"); break;
                case 221: BottomFrame.Navigate(typeof(BarPage.EffectPage2.Tint));  App.Model.Text = App.resourceLoader.GetString("/Effect2/Tint_"); break;
                case 222: BottomFrame.Navigate(typeof(BarPage.EffectPage2.DiscreteTransfer));  App.Model.Text = App.resourceLoader.GetString("/Effect2/DiscreteTransfer_"); break;
                case 223: BottomFrame.Navigate(typeof(BarPage.EffectPage2.Vignette));  App.Model.Text = App.resourceLoader.GetString("/Effect2/Vignette_"); break;
                case 224: BottomFrame.Navigate(typeof(BarPage.EffectPage2.GammaTransfer));  App.Model.Text = App.resourceLoader.GetString("/Effect2/GammaTransfer_"); break;

                case 230: BottomFrame.Navigate(typeof(BarPage.EffectPage3.Glass));  App.Model.Text = App.resourceLoader.GetString("/Effect3/Glass_"); break;
                case 231: BottomFrame.Navigate(typeof(BarPage.EffectPage3.PinchPunch));  App.Model.Text = App.resourceLoader.GetString("/Effect3/PinchPunch_"); break;
                case 232: BottomFrame.Navigate(typeof(BarPage.EffectPage3.Morphology));  App.Model.Text = App.resourceLoader.GetString("/Effect3/Morphology_"); break;


                //Other：杂项
                case 300: BottomFrame.Navigate(typeof(BarPage.OtherPage.Crop));   App.Model.Text = App.resourceLoader.GetString("/Other/Crop_"); break;
                case 303: BottomFrame.Navigate(typeof(BarPage.OtherPage.Gradient)); App.Model.Text = App.resourceLoader.GetString("/Other/Gradient_"); break;
                case 304: BottomFrame.Navigate(typeof(BarPage.OtherPage.Fade));  App.Model.Text = App.resourceLoader.GetString("/Other/Fade_"); break;
                case 305: BottomFrame.Navigate(typeof(BarPage.OtherPage.Text));  App.Model.Text = App.resourceLoader.GetString("/Other/Text_"); break;
                case 306: BottomFrame.Navigate(typeof(BarPage.OtherPage.Grids));  App.Model.Text = App.resourceLoader.GetString("/Other/Grids_"); break;
                case 307: BottomFrame.Navigate(typeof(BarPage.OtherPage.Fill));  App.Model.Text = App.resourceLoader.GetString("/Other/Fill_"); break;
                case 308: BottomFrame.Navigate(typeof(BarPage.OtherPage.Transform));  App.Model.Text = App.resourceLoader.GetString("/Other/Transform_"); break;
                case 309: BottomFrame.Navigate(typeof(BarPage.OtherPage.Transform3D));  App.Model.Text = App.resourceLoader.GetString("/Other/Transform3D_"); break;


                // Layer：图层
                case 400: BottomFrame.Navigate(typeof(BarPage.OtherPage.Transform));  App.Model.Text = App.resourceLoader.GetString("/Layer/Image_"); break;


                //Geometry：几何
                case 500: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Line));  App.Tip(App.resourceLoader.GetString("/Geometry/Line_"));break;
                case 501: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Free));  App.Tip(App.resourceLoader.GetString("/Geometry/Free_"));break;
                case 502: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Image));  App.Tip(App.resourceLoader.GetString("/Geometry/Image_"));break;
                case 503: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Word));  App.Tip(App.resourceLoader.GetString("/Geometry/Word_"));break;

                case 504: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Circle));  App.Tip(App.resourceLoader.GetString("/Geometry/Circle_"));break;
                case 505: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Rect)); App.Tip(App.resourceLoader.GetString("/Geometry/Rect_")); break;
                case 506: BottomFrame.Navigate(typeof(BarPage.GeometryPage.RoundRect)); App.Tip(App.resourceLoader.GetString("/Geometry/RoundRect_")); break;
                case 507: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Triangle));  App.Tip(App.resourceLoader.GetString("/Geometry/Triangle_"));break;

                case 508: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Diamond));  App.Tip(App.resourceLoader.GetString("/Geometry/Diamond_"));break;
                case 509: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Pentagon));  App.Tip(App.resourceLoader.GetString("/Geometry/Pentagon_"));break;
                case 510: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Star));  App.Tip(App.resourceLoader.GetString("/Geometry/Star_"));break;
                case 511: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Pie));  App.Tip(App.resourceLoader.GetString("/Geometry/Pie_"));break;

                case 512: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Cog));  App.Tip(App.resourceLoader.GetString("/Geometry/Cog_"));break;
                 case 513: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Arrow));  App.Tip(App.resourceLoader.GetString("/Geometry/Arrow_"));break;
                case 514: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Capsule));  App.Tip(App.resourceLoader.GetString("/Geometry/Capsule_"));break;
                case 515: BottomFrame.Navigate(typeof(BarPage.GeometryPage.Heart));  App.Tip(App.resourceLoader.GetString("/Geometry/Heart_"));break;
 

                default:  break;

            }
        }
 

    }
}
