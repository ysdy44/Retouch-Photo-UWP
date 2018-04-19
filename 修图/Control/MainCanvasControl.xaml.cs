using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using 修图.Library;
using 修图.Model;

namespace 修图.Control
{
    public sealed partial class MainCanvasControl : UserControl
    {
        //套索
        Lasso2 lasso;

        //Delegate
        public delegate void LayerHandler();
        public event LayerHandler Layers;

        #region DependencyProperty：依赖属性


        //刷新
        public int Refresh
        {
            get { return (int)GetValue(RefreshProperty); }
            set { SetValue(RefreshProperty, value); }
        }

        public static readonly DependencyProperty RefreshProperty =
            DependencyProperty.Register("Refresh", typeof(int), typeof(MainCanvasControl), new PropertyMetadata(0, new PropertyChangedCallback(RefreshOnChang)));

        private static void RefreshOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MainCanvasControl Con = (MainCanvasControl)sender;

            Con.CanvasVirtualControl222.Invalidate();//刷新画布内容 

            int t = App.Model.Tool;

            //Crop：裁切
            if (t == 300) Con.Crop_Move_Delta();

            //Gradient：渐变
            else if (t == 303) Con.Gradient_Move_Delta();

            //Fade：渐隐
            else if (t == 304) Con.Fade_Move_Delta();

            //Text：文字
            else if (t == 305) Con.Text_Move_Delta();

            //Mask：粘贴   //Transform：变换选区   //Transform：变换    //Layer：图片  
            else if (t == 102 || t == 111 || t == 308 || t == 400) Con.Transform_Move_Delta();

        }

        //工具
        public int Tool
        {
            get { return (int)GetValue(ToolProperty); }
            set { SetValue(ToolProperty, value); }
        }

        public static readonly DependencyProperty ToolProperty =
            DependencyProperty.Register("Tool", typeof(int), typeof(MainCanvasControl), new PropertyMetadata(0, new PropertyChangedCallback(ToolOnChang)));

        private static void ToolOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MainCanvasControl Con = (MainCanvasControl)sender;


            int OldTool = (int)e.OldValue;
            int NewTool = (int)e.NewValue;

            //Cursor：光标
            if (NewTool == 1 || OldTool == 1)
            {
                App.Model.isReRender = true;//重新渲染
                Con.CanvasVirtualControl222.Invalidate();
            }


            //Straw：取色
            if (OldTool == 2)
            {
                if (Con.StrawGrid.Opacity > 0)
                {
                    Con.isStraw = false;
                    Con.Straw_Complete(Con.Point);

                    //动画：消失
                    Con.StrawFade.Begin();
                }
            }


            //Polyline：多边形  
            if (NewTool == 6)
            {
                Con.PolylineWhite.Points.Clear();
                Con.PolylineBlack.Points.Clear();
            }
            if (OldTool == 6)
            {
                if (Con.PolylineHead.Opacity > 0)
                {
                    Con.inPolygon = false;
                    Con.Lasso_Complete(Con.Point);

                    //动画：消失
                    Con.PolylineHeadFade.Begin();
                    Con.PolylineFootFade.Begin();
                }
            }


            //铅笔Pencil
            if (NewTool == 13)
            {
                Con.Pencil_Move_Delta(false);
                Con.Pencil_Move_Complete(false);
            }
            if (OldTool == 13) Con.Pencil_Move_Start(false);



            //钢笔Pen
            if (OldTool == 14)
                Con.CanvasVirtualControl222.Invalidate();



            //暗室灯光Lighting
            if (NewTool == 218)
            {
                Con.LightingChange();
                Con.Lighting_Move_Delta();
                Con.Lighting_Move_Complete();
            }
            if (OldTool == 218) Con.Lighting_Move_Start();


            //PinchPunch：膨胀收缩
            if (NewTool == 231)
            {
                Con.PinchPunchChange();
                Con.PinchPunch_Move_Delta();
                Con.PinchPunch_Move_Complete();
            }
            if (OldTool == 231) Con.PinchPunch_Move_Start();




            //Crop：裁切
            if (NewTool == 300)
            {
                Con.CropChange();
                Con.Crop_Move_Delta();
                Con.Crop_Move_Complete();
            }
            if (OldTool == 300) Con.Crop_Move_Start();



            //Gradient：渐变
            if (NewTool == 303)
            {
                Con.GradientChange();
                Con.Gradient_Move_Delta();
                Con.Gradient_Move_Complete();
            }
            if (OldTool == 303) Con.Gradient_Move_Start();


            //Fade：渐隐
            if (NewTool == 304)
            {
                Con.FadeChange();
                Con.Fade_Move_Delta();
                Con.Fade_Move_Complete();
            }
            if (OldTool == 304) Con.Fade_Move_Start();


            //Text：文字
            if (NewTool == 305)
            {
                Con.TextChange();
                Con.Text_Move_Delta();
                Con.Text_Move_Complete();
            }
            if (OldTool == 305) Con.Text_Move_Start();




            //Mask：粘贴   //Transform：变换选区   //Transform：变换    //Layer：图片  
            if (NewTool == 102 || NewTool == 111 || NewTool == 308 || NewTool == 400)
            {
                Con.TransformChange();
                Con.Transform_Move_Delta();
                Con.Transform_Move_Complete();
            }
            if (OldTool == 102 || OldTool == 111 || OldTool == 308 || OldTool == 400) Con.Transform_Move_Start();

        }

        #endregion

        public MainCanvasControl()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;

            //DPI：真正的DPI
            App.Model.DPIReady = this.CanvasVirtualControl.Dpi;

            //DPI：使得DPI为96
            this.CanvasVirtualControl.DpiScale *= App.Model.DPI / this.CanvasVirtualControl.Dpi;
            this.CanvasAnimatedControl.DpiScale *= App.Model.DPI / this.CanvasAnimatedControl.Dpi;

            App.Model.InkCanvas = this.InkCanvas;
            App.Model.VirtualControl = this.CanvasVirtualControl;
            App.Model.AnimatedControl = this.CanvasAnimatedControl;
        }

        #region Global：全局

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //画布位置居中，并与左右边界保持1/8的边距
            App.Model.CanvasWidth = App.Model.GridWidth / 8 * 7;
            App.Model.CanvasHeight = App.Model.CanvasWidth / App.Model.Width * App.Model.Height;
            App.Model.X = (App.Model.GridWidth - App.Model.CanvasWidth) / 2;
            App.Model.Y = (App.Model.GridHeight - App.Model.CanvasHeight) / 2;
        }

        private void CanvasVirtualControl222_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            App.Model.GridWidth = e.NewSize.Width;
            App.Model.GridHeight = e.NewSize.Height;
        }



        #endregion

        #region Drop：拖放


        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (App.Model.Tool < 100)
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                    items = items.OfType<StorageFile>().Where(s =>
                     s.FileType.ToUpper().Equals(".JPG") ||
                    s.FileType.ToUpper().Equals(".JPEG") ||
                    s.FileType.ToUpper().Equals(".PNG") ||
                    s.FileType.ToUpper().Equals(".GIF") ||
                    s.FileType.ToUpper().Equals(".BMP")
                    ).ToList() as IReadOnlyList<IStorageItem>;

                    if (items != null)//空或没有
                    {
                        foreach (var item in items)
                        {
                            var file = item as StorageFile;

                            using (IRandomAccessStream stream = await file.OpenReadAsync())
                            {
                                App.Model.SecondCanvasBitmap = await CanvasVirtualBitmap.LoadAsync(App.Model.VirtualControl, stream);
                                App.InitializeOther();
                                Layers();
                            }
                        }
                    }
                    else App.Tip(App.resourceLoader.GetString("/Home/PicturesUnsupported_"));//不支持的图片格式
                }
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = App.resourceLoader.GetString("DropAcceptable_");//可以接受的图片
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }


        #endregion


        #region Virtual & Animated：虚拟 & 动画


        private void CanvasVirtualControl_CreateResources(CanvasVirtualControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }
        async Task CreateResourcesAsync(CanvasVirtualControl sender)//File：文件关联
        {
            App.Model.PunchDisplacement = await CanvasBitmap.LoadAsync(App.Model.VirtualControl, "Icon/PunchDisplacement.png");//Punch：膨胀
            App.Model.PinchDisplacement = await CanvasBitmap.LoadAsync(App.Model.VirtualControl, "Icon/PinchDisplacement.png");//Pinch：收缩

            App.Model.isCanUndo = false;//关闭撤销

            //层
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            Layer l = new Layer { CanvasRenderTarget = crt };
            if (App.Model.isLowView) l.LowView();
            else l.SquareView();
            l.SetWriteableBitmap(App.Model.VirtualControl);
            App.Model.Layers.Add(l);
            App.Model.Index = 0;

            //初始化
            App.Initialize(App.Model.VirtualControl, App.Model.Width, App.Model.Height);

            App.Model.isCanUndo = true;//打开撤销      
        }


        private void CanvasVirtualControl_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            //绘画
            foreach (var region in args.InvalidatedRegions)
            {
                using (CanvasDrawingSession ds = sender.CreateDrawingSession(region))
                {
                    int t = App.Model.Tool;

                    //如果重新渲染
                    if (App.Model.isReRender == true)
                    {
                        App.Model.isReRender = false;

                        //渲染：图层Layers一层层渲染到RenderTarget
                        using (CanvasDrawingSession dds = App.Model.MainRenderTarget.CreateDrawingSession())
                        {
                            if (t == -3) LiquifyDraw(dds, args); //Liquify：液化
                            else if ((t >= 0 && t < 100) || t == -4 || (t >= 500 && t < 600)) MainDraw(dds, args);//Tool：工具
                            else if (t == 110 || t == 111) MaskDraw(dds, args);//Feather：羽化 //Transform：选区变换 
                            //Effevt：特效   //Fade：渐变  //Adjust：调整  //Transform3D：变换3D 
                            else if ((t >= 200 && t < 300) || t == 304 || t == -2 || t == 309) EffectDraw(dds, args);
                            else if (t == 303 || t == 306 || t == 307) OtherDraw(dds, args);//Gradient：渐变 //Fill：填充 
                            else if (t == 305) GeometryrDraw(dds, args); //Text：文字 
                        }
                    }


                    Matrix3x2 m = App.Model.Matrix;
                    if (t == 300)//Crop：裁切 
                    {
                        ds.DrawImage(Adjust.GetShadow(App.GrayWhiteGrid, m));
                        ds.DrawImage(new Transform2DEffect { Source = App.Model.SecondCanvasImage, TransformMatrix = m, InterpolationMode = CanvasImageInterpolation.NearestNeighbor, });
                    }
                    else if (t == 308 || t == 102 || t == 400) TransformDraw(ds, m);//Transform：变换   //Mask：粘贴  //Layer：图层 
                    else ds.DrawImage(Adjust.GetShadow(App.Model.MainRenderTarget, m)); //变换：根据Transform2DEffect确定RenderTarget的缩放 ，以适配画布



                    if (App.Model.CanvasWidth > 100 && App.Model.CanvasHeight > 100)
                    {
                        if (App.Model.isLine == true) LineDraw(ds);//Line：画线     
                        if (App.Model.isRuler == true) RulerDraw(ds);//Ruler：画标尺
                    }
                    if (t == 14 && App.Setting.PenVectorList.Count > 0) PenDraw(ds);//Pen：画钢笔
                    if (t == 1) CursorDraw(ds);//Cursor：画光标        
                }
            }
        }






        //动画控件
        private void CanvasAnimatedControl_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if (App.Model.isAnimated == true)
                lasso.Draw(sender, args.DrawingSession, App.Model.CanvasWidth, App.Model.CanvasHeight, ContainX + (float)App.Model.X, ContainY + (float)App.Model.Y);


            //Geometry：几何预先渲染   //Straw：临时取色
            if (isSingle == true && App.Model.StrawVisibility == Visibility.Visible)
            {
                CanvasGeometry Geometry;
                Vector2 Start = PointStart.ToVector2();
                Vector2 End = Point.ToVector2();
                switch (App.Model.Tool)
                {
                    //Line：直线
                    case 500:
                        args.DrawingSession.DrawLine(Start, End, App.Setting.LiquifyColor);
                        break;
                    //Free：自由线
                    case 501:
                        if (FreeList.Count > 3)
                        {
                            Geometry = CanvasGeometry.CreatePolygon(sender, FreeList.ToArray());
                            args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        }
                        break;
                    //Image：图像
                    case 502:
                        if (App.Setting.isImageRatio == true)
                        {
                            float us = Math.Abs(Start.X - End.X) + Math.Abs(Start.Y - End.Y);
                            float cale = us / (App.Setting.ImageWidth + App.Setting.ImageHeight);

                            float W = App.Setting.ImageWidth * cale;
                            float H = App.Setting.ImageHeight * cale;

                            float X = Start.X < End.X ? Start.X : Start.X - W;
                            float Y = Start.Y < End.Y ? Start.Y : Start.Y - H;

                            args.DrawingSession.DrawRectangle(X, Y, W, H, App.Setting.LiquifyColor);
                        }
                        else args.DrawingSession.DrawRectangle(new Rect(PointStart, Point), App.Setting.LiquifyColor);
                        break;
                    //Word：字词
                    case 503:
                        var g = 修图.Library.Geometry.CreateWord(sender, Start, End, App.Setting.WordText, App.Setting.WordFontFamily, App.Setting.WordFontStyle);
                        args.DrawingSession.DrawGeometry(g.Geometry, g.Vect, App.Setting.LiquifyColor);

                        Rect rect = g.Geometry.ComputeBounds();
                        rect.X += g.Vect.X;
                        rect.Y += g.Vect.Y;
                        args.DrawingSession.DrawRectangle(rect, App.Setting.LiquifyColor);
                        break;


                    //Circle：圆
                    case 504:
                        args.DrawingSession.DrawLine(Start, End, App.Setting.LiquifyColor);
                        args.DrawingSession.DrawCircle(Start, (End - Start).Length(), App.Setting.LiquifyColor);
                        break;
                    //Rect：矩形
                    case 505:
                        args.DrawingSession.DrawRectangle(new Rect(PointStart, Point),  App.Setting.LiquifyColor);
                        break;
                    //RoundRect：圆角矩形
                    case 506:
                        float Radius = App.Setting.RoundRectRadius / App.Model.SX;
                        args.DrawingSession.DrawRoundedRectangle(new Rect(PointStart, Point), Radius, Radius, App.Setting.LiquifyColor);
                        break;
                    //Triangle：三角形
                    case 507:
                        Geometry = 修图.Library.Geometry.CreateTriangle(sender, Start, End);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break;


                    //Diamond：菱形
                    case 508:
                        Geometry = 修图.Library.Geometry.CreateDiamond(sender, Start, End);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break;
                    //Pentagon：多边形
                    case 509:
                        Geometry = 修图.Library.Geometry.CreatePentagon(sender, Start, End, App.Setting.PentagonCount);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break;
                    //Star：星星
                    case 510:
                        Geometry = 修图.Library.Geometry.CreateStar(sender, Start, End, App.Setting.StarCount, App.Setting.StarInner);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break;
                    //Pie：饼图
                    case 511:
                        args.DrawingSession.DrawCircle(Start, (End - Start).Length(), App.Setting.LiquifyColor);
                        Geometry = 修图.Library.Geometry.CreatePie(sender, Start, End, App.Setting.PieInner, App.Setting.PieSweep);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break;


                    //Cog：齿轮
                    case 512:
                        Geometry = 修图.Library.Geometry.CreateCog(sender, Start, End, App.Setting.CogCount, App.Setting.CogInner, App.Setting.CogTooth, App.Setting.CogNotch);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break;
                    //Arrow：箭头
                    case 513:
                        Geometry = 修图.Library.Geometry.CreateArrow(sender, Start, End, App.Setting.ArrowHead / App.Model.SX);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break;
                    //Capsule：胶囊
                    case 514:
                        Geometry = 修图.Library.Geometry.CreateCapsule(sender, Start, End, App.Setting.CapsuleRadius / App.Model.SX);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break;
                    //Heart：心型
                    case 515:
                        Geometry = 修图.Library.Geometry.CreateHeart(sender, Start, End);
                        args.DrawingSession.DrawGeometry(Geometry, App.Setting.LiquifyColor);
                        break; 





                    default:
                        break;
                }
            }


        }
        private void CanvasAnimatedControl_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            if (App.Model.isAnimated == true)
            {
                //套索：更新颜色
                if (App.Model.isUpdate == true) lasso.Update();

                if (App.Model.isReStroke == true)
                {
                    lasso.Set(sender, App.Model.SX, App.Model.SY, App.Model.MaskAnimatedTarget);

                    App.Model.isReStroke = false;
                }
            }
        }
        private void CanvasAnimatedControl_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            lasso = new Lasso2(sender, 6, 1);//套索

            //sender.TargetElapsedTime*= 4;//Animated：刷新速度

            App.Model.MaskAnimatedTarget = new CanvasRenderTarget(sender, App.Model.Width, App.Model.Height);

            App.Model.isReStroke = true;
        }


        #endregion

        #region Second ：第二界面


        //Line：网格线
        Color LineColor = Color.FromArgb(90, 255, 255, 255);//字体颜色
        Color LineSignColor = Color.FromArgb(180, 255, 255, 255);//高亮字体颜色
        //Line：画网格线
        private void LineDraw(CanvasDrawingSession ds)
        {
            //左上右下的位置
            float LineL = (float)App.Model.X;
            float LineT = (float)App.Model.Y;
            float LineR = LineL + (float)App.Model.CanvasWidth;
            float LineB = LineT + (float)App.Model.CanvasHeight;


            //间隔
            float space = (float)(10 * App.Model.YS);
            while (space < 10) space *= 5; //大则小
            while (space > 100) space /= 5;//小则大
            float spaceFive = space * 5;//五倍


            //线循环
            for (float X = 0; X < App.Model.CanvasWidth; X += space)
            {
                float xx = LineL + X;
                ds.DrawLine(xx, LineT, xx, LineB, LineColor);
            }
            for (float Y = 0; Y < App.Model.CanvasHeight; Y += space)
            {
                float yy = LineT + Y;
                ds.DrawLine(LineL, yy, LineR, yy, LineColor);
            }


            //实线循环
            for (float X = 0; X < App.Model.CanvasWidth; X += spaceFive)
            {
                float xx = LineL + X;
                ds.DrawLine(xx, LineT, xx, LineB, LineColor);
            }
            for (float Y = 0; Y < App.Model.CanvasHeight; Y += spaceFive)
            {
                float yy = LineT + Y;
                ds.DrawLine(LineL, yy, LineR, yy, LineColor);
            }
        }

        //Ruler：画标尺线
        Color RulerColor;//字体颜色
        Color RulerBackColor;//背景颜色
        Color RulerSignColor;//高亮字体颜色
        float RulerSpace = 20;//标尺刻度空间
        float RulerHalf = 10;//一半的标尺刻度空间
        CanvasTextFormat RulerTextFormat = new CanvasTextFormat()//字体格式
        {
            FontSize = 12,
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center,
        };
        //Ruler：画标尺线
        private void RulerDraw(CanvasDrawingSession ds)
        {
             RulerColor = App.Model.Foreground.Color;
            RulerBackColor = App.Model.PanelColor.Color;
            RulerSignColor = App.Model.SignForeground.Color;


            float GridWidth = (float)App.Model.GridWidth;
            float GridHeight = (float)App.Model.GridHeight;

            //画背景颜色
            ds.FillRectangle(0, 0, RulerSpace, RulerSpace, RulerBackColor);//左上角
            ds.FillRectangle(RulerSpace, 0, GridWidth, RulerSpace, RulerBackColor);//水平条
            ds.FillRectangle(0, RulerSpace, RulerSpace, GridHeight, RulerBackColor);//垂直条
            ds.DrawLine(RulerSpace, RulerSpace, GridWidth, RulerSpace, RulerColor);//水平线
            ds.DrawLine(RulerSpace, RulerSpace, RulerSpace, GridHeight, RulerColor);//垂直线


            //间隔
            float space = (float)(10 * App.Model.YS);
            while (space < 10) space *= 5; //大则小
            while (space > 100) space /= 5;//小则大
            float spaceFive = space * 5;//五倍


            //水平线循环
            for (float X = (float)App.Model.X; X < App.Model.GridWidth; X += space) ds.DrawLine(X, RulerHalf, X, RulerSpace, RulerColor);
            for (float X = (float)App.Model.X; X > RulerSpace; X -= space) ds.DrawLine(X, RulerHalf, X, RulerSpace, RulerColor);
            //垂直线循环
            for (float Y = (float)App.Model.Y; Y < App.Model.GridHeight; Y += space) ds.DrawLine(RulerHalf, Y, RulerSpace, Y, RulerColor);
            for (float Y = (float)App.Model.Y; Y > RulerSpace; Y -= space) ds.DrawLine(RulerHalf, Y, RulerSpace, Y, RulerColor);


            //水平实线循环
            for (float X = (float)App.Model.X; X < App.Model.GridWidth; X += spaceFive) ds.DrawLine(X, RulerHalf, X, RulerSpace, RulerSignColor);
            for (float X = (float)App.Model.X; X > RulerSpace; X -= spaceFive) ds.DrawLine(X, RulerHalf, X, RulerSpace, RulerSignColor);
            //垂直实线循环
            for (float Y = (float)App.Model.Y; Y < App.Model.GridHeight; Y += spaceFive) ds.DrawLine(RulerHalf, Y, RulerSpace, Y, RulerSignColor);
            for (float Y = (float)App.Model.Y; Y > RulerSpace; Y -= spaceFive) ds.DrawLine(RulerHalf, Y, RulerSpace, Y, RulerSignColor);


            //水平文字循环
            for (float X = (float)App.Model.X; X < App.Model.GridWidth; X += spaceFive) ds.DrawText(((int)(Math.Round((X - App.Model.X) / App.Model.XS))).ToString(), X, RulerHalf, RulerSignColor, RulerTextFormat);
            for (float X = (float)App.Model.X; X > RulerSpace; X -= spaceFive) ds.DrawText(((int)(Math.Round((X - App.Model.X) / App.Model.XS))).ToString(), X, RulerHalf, RulerSignColor, RulerTextFormat);
            //垂直文字循环
            for (float Y = (float)App.Model.Y; Y < App.Model.GridHeight; Y += spaceFive) ds.DrawText(((int)(Math.Round((Y - App.Model.Y) / App.Model.YS))).ToString(), RulerHalf, Y, RulerSignColor, RulerTextFormat);
            for (float Y = (float)App.Model.Y; Y > RulerSpace; Y -= spaceFive) ds.DrawText(((int)(Math.Round((Y - App.Model.Y) / App.Model.YS))).ToString(), RulerHalf, Y, RulerSignColor, RulerTextFormat);


            //画水平线垂直线
            foreach (float y in App.Setting.RulerVerticalList)
            {
                float Y = (float)(App.Model.Y + y * App.Model.YS);
                ds.DrawLine(RulerSpace, Y, GridWidth - RulerSpace, Y, TransformVerticalColor);//水平线
                                                                                              //画圆圈
                ds.FillCircle(RulerHalf, Y, 8, RulerSignColor);
                ds.FillCircle(RulerHalf, Y, 7, TransformVerticalColor);
                //文字
                ds.DrawText(((int)y).ToString(), GridWidth - RulerHalf, Y, RulerSignColor, RulerTextFormat);
            }
            foreach (float x in App.Setting.RulerHorizontalList)
            {
                float X = (float)(App.Model.X + x * App.Model.XS);
                ds.DrawLine(X, RulerSpace, X, GridHeight - RulerSpace, TransformHorizontalColor);//垂直线
                //画圆圈
                ds.FillCircle(X, RulerHalf, 8, RulerSignColor);
                ds.FillCircle(X, RulerHalf, 7, TransformHorizontalColor);
                //文字
                ds.DrawText(((int)x).ToString(), X, GridHeight - RulerHalf, RulerSignColor, RulerTextFormat);
            }
        }



        //Cursor：画光标
        private void CursorDraw(CanvasDrawingSession ds)
        {
            Color Blue = Color.FromArgb(255, 54, 135, 230);

            if (isCursorMove == false)//没有处于光标移动模式：画蓝框
            {
                Vector2 s = App.Model.CanvasToScreen(CursorStart);
                Vector2 e = App.Model.CanvasToScreen(CursorEnd);

                float x = Math.Min(s.X, e.X);
                float y = Math.Min(s.Y, e.Y);
                float w = Math.Abs(s.X - e.X);
                float h = Math.Abs(s.Y - e.Y);

                ds.FillRectangle(x, y, w, h, Color.FromArgb(90, 54, 135, 230));//透明蓝色
                ds.DrawRectangle(x, y, w, h, Blue);//蓝色
            }

            //画所有的集合的矩形
            ds.DrawRectangle(App.Model.CanvasToScreen(new Rect(App.Setting.CursorGroupStart, App.Setting.CursorGroupEnd)), Blue);//蓝色
        }

        //Pen：画钢笔
        private void PenDraw(CanvasDrawingSession ds)
        {
            Color Blue = Color.FromArgb(255, 54, 135, 230);
            Color Light = Color.FromArgb(255, 216, 233, 253);
            Color Shadow = Color.FromArgb(70, 127, 127, 127);


            //画线
            CanvasPathBuilder pathBuilder = new CanvasPathBuilder(App.Model.VirtualControl);
            pathBuilder.BeginFigure(App.Model.CanvasToScreen(App.Setting.PenVectorList[0].Vect));
            for (int i = 0; i < App.Setting.PenVectorList.Count - 1; i++)//0 to 9
            {
                pathBuilder.AddCubicBezier(App.Model.CanvasToScreen(App.Setting.PenVectorList[i].Left), App.Model.CanvasToScreen(App.Setting.PenVectorList[i + 1].Right), App.Model.CanvasToScreen(App.Setting.PenVectorList[i + 1].Vect));
            }
            pathBuilder.EndFigure(CanvasFigureLoop.Open);
            ds.DrawGeometry(CanvasGeometry.CreatePath(pathBuilder), Blue, 2);


            //画控制点
            for (int i = 0; i < App.Setting.PenVectorList.Count; i++)
            {
                Vector2 v = App.Model.CanvasToScreen(App.Setting.PenVectorList[i].Vect);
                Vector2 vr = App.Model.CanvasToScreen(App.Setting.PenVectorList[i].Right);
                Vector2 vl = App.Model.CanvasToScreen(App.Setting.PenVectorList[i].Left);

                //控点
                if (App.Setting.PenVectorList[i].isChecked == true)
                {
                    //主点
                    ds.FillEllipse(v, 10, 10, Shadow);
                    ds.FillEllipse(v, 8, 8, Colors.White);//白色
                    ds.FillEllipse(v, 6, 6, Blue);//蓝色

                    if (i > 0)
                    {
                        //右控制点
                        ds.DrawLine(vr, v, Blue, 1);//画线
                        ds.FillEllipse(vr, 8, 8, Shadow);
                        ds.FillEllipse(vr, 7, 7, Colors.White);//白色
                        ds.FillEllipse(vr, 6, 6, Blue);//蓝色
                    }
                    if (i < App.Setting.PenVectorList.Count - 1)
                    {
                        //左控制点
                        ds.DrawLine(vl, v, Blue, 1);//画线
                        ds.FillEllipse(vl, 8, 8, Shadow);
                        ds.FillEllipse(vl, 7, 7, Colors.White);//白色
                        ds.FillEllipse(vl, 6, 6, Blue);//蓝色
                    }
                }
                else
                {
                    //主点
                    ds.FillEllipse(v, 8, 8, Shadow);
                    ds.FillEllipse(v, 7, 7, Blue);//蓝色
                    ds.FillEllipse(v, 6, 6, Colors.White);//白色 
                }
            }


            //PenRect：钢笔矩形
            if (isPen == true)
            {
                Vector2 s = App.Model.CanvasToScreen(PenStart);
                Vector2 e = App.Model.CanvasToScreen(PenEnd);

                float x = Math.Min(s.X, e.X);
                float y = Math.Min(s.Y, e.Y);
                float w = Math.Abs(s.X - e.X);
                float h = Math.Abs(s.Y - e.Y);

                ds.FillRectangle(x, y, w, h, Color.FromArgb(90, 54, 135, 230));//透明蓝色
                ds.DrawRectangle(x, y, w, h, Blue);//蓝色
            }
        }



        //Transform：变换
        Color TransformHorizontalColor = Color.FromArgb(255, 14, 255, 25);
        Color TransformVerticalColor = Color.FromArgb(255, 255, 0, 31);
        //Transform：变换   //Mask：粘贴  //Layer：图层 
        private void TransformDraw(CanvasDrawingSession ds, Matrix3x2 m)
        {
            //特效渲染目标（上中下）            
            ds.DrawImage(Adjust.GetShadow(App.Model.MainRenderTarget, m));

            ds.DrawImage(new Transform2DEffect { Source = App.Model.SecondBottomRenderTarget, TransformMatrix = m, InterpolationMode = CanvasImageInterpolation.NearestNeighbor, });
            if (App.Model.SecondCanvasImage != null) ds.DrawImage(
                new Transform2DEffect
                {
                    Source = App.Model.SecondCanvasImage,
                    TransformMatrix = m,
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                });
            ds.DrawImage(new Transform2DEffect { Source = App.Model.SecondTopRenderTarget, TransformMatrix = m, InterpolationMode = CanvasImageInterpolation.NearestNeighbor, });

            //Snapping：吸附
            if (App.Setting.TransformSnapping == true && isSingle == true)
            {
                //水平对齐    
                TransformHorizontalLineDraw(ds, TransformHorizontalLeft, (float)App.Model.X);
                TransformHorizontalLineDraw(ds, TransformHorizontalCenter, (float)(App.Model.CanvasWidth / 2 + App.Model.X));
                TransformHorizontalLineDraw(ds, TransformHorizontalRight, (float)(App.Model.CanvasWidth + App.Model.X));
                //垂直对齐   
                TransformVerticalLineDraw(ds, TransformVerticalTop, (float)App.Model.Y);
                TransformVerticalLineDraw(ds, TransformVerticalCenter, (float)(App.Model.CanvasHeight / 2 + App.Model.Y));
                TransformVerticalLineDraw(ds, TransformVerticalBottom, (float)(App.Model.CanvasHeight + App.Model.Y));
            }
        }
        private void TransformHorizontalLineDraw(CanvasDrawingSession ds, float CanvasX, float ScreenX)
        {
            if (App.Setting.TransformX == CanvasX)
            {
                ds.DrawLine(ScreenX, 0, ScreenX, (float)App.Model.GridHeight, TransformHorizontalColor);
                ds.DrawLine(ScreenX, (float)App.Model.Y, ScreenX, (float)(App.Model.CanvasHeight + App.Model.Y), TransformHorizontalColor, 2);
            }
        }
        private void TransformVerticalLineDraw(CanvasDrawingSession ds, float CanvasY, float ScreenY)
        {
            if (App.Setting.TransformY == CanvasY)
            {
                ds.DrawLine(0, ScreenY, (float)App.Model.GridWidth, ScreenY, TransformVerticalColor);
                ds.DrawLine((float)App.Model.X, ScreenY, (float)(App.Model.CanvasWidth + App.Model.X), ScreenY, TransformVerticalColor, 2);
            }
        }



        //Main：主渲染
        private void MainDraw(CanvasDrawingSession ds, CanvasRegionsInvalidatedEventArgs args)
        {
            ICanvasImage ci = App.GrayWhiteGrid;
            if (App.Model.Tool == 1)
            {
                for (int i = App.Model.Layers.Count - 1; i >= 0; i--)//自下而上渲染
                {
                    ci = App.RenderTransform(App.Model.Layers[i], ci);//渲染
                }
            }
            else
            {
                for (int i = App.Model.Layers.Count - 1; i >= 0; i--)//自下而上渲染
                {
                    ci = App.Render(App.Model.Layers[i], ci);//渲染
                }
            }
            ds.DrawImage(ci);

            //画蓝色半透明选区
            if (App.Model.isMask == true) ds.DrawImage(new OpacityEffect
            {
                Source = App.Model.MaskRenderTarget,
                Opacity = App.Setting.MaskOpacity
            });
        }

        //Liquify：液化
        private void LiquifyDraw(CanvasDrawingSession ds, CanvasRegionsInvalidatedEventArgs args)
        {
            ds.DrawImage(App.GrayWhiteGrid);
            App.Model.SecondCanvasImage = Adjust.GetDisplacementMap(App.Model.SecondSourceRenderTarget, App.Model.SecondBottomRenderTarget, 100);
            ds.DrawImage(App.Model.SecondCanvasImage);
        }

        //Mask：选区
        private void MaskDraw(CanvasDrawingSession ds, CanvasRegionsInvalidatedEventArgs args)
        {
            ds.DrawImage(App.GrayWhiteGrid);

            //选区渲染目标（上）
            ds.DrawImage(App.Model.SecondTopRenderTarget);

            ds.DrawImage(new OpacityEffect//半透明蓝色选区
            {
                Source = App.Model.SecondCanvasImage,
                Opacity = App.Setting.MaskOpacity
            });
        }

        //Effect：特效
        private void EffectDraw(CanvasDrawingSession ds, CanvasRegionsInvalidatedEventArgs args)
        {
            //特效渲染目标（上中下）
            ds.DrawImage(App.Model.SecondBottomRenderTarget);
            if (App.Model.SecondCanvasImage != null) ds.DrawImage(App.Model.SecondCanvasImage);
            ds.DrawImage(App.Model.SecondTopRenderTarget);
        }

        //Other：杂项
        private void OtherDraw(CanvasDrawingSession ds, CanvasRegionsInvalidatedEventArgs args)
        {
            //杂项渲染目标（上中下）
            ds.DrawImage(App.Model.SecondBottomRenderTarget);
            ds.DrawImage(App.Model.SecondSourceRenderTarget);
            ds.DrawImage(App.Model.SecondTopRenderTarget);
        }

        //Geometry：几何
        private void GeometryrDraw(CanvasDrawingSession ds, CanvasRegionsInvalidatedEventArgs args)
        {
            //杂项渲染目标（上中下）
            ds.DrawImage(App.Model.SecondBottomRenderTarget);
            if (App.Model.SecondCanvasGeometry != null) ds.FillGeometry(App.Model.SecondCanvasGeometry, App.Setting.TextX, App.Setting.TextY, App.Model.TextColor);
            ds.DrawImage(App.Model.SecondTopRenderTarget);
        }


        #endregion



        #region Point：指针事件


        Point Point;//指针移动事件，即使点的位置
        bool isThumb = false;//拇指正在移动，移动事件不起作用

        bool isSingle = false;//是否单指
        bool isDouble = false;
        bool isRight = false;

        public List<uint> FingerCollection = new List<uint> { };//可视指集
        Point FingerSingle;//单数点
        Point FingerDouble;//双数点
        Point FingerCenter { get => new Point((FingerSingle.X + FingerDouble.X) / 2, (FingerSingle.Y + FingerDouble.Y) / 2); }//中心点
        double FingerDistance { get => Math.Sqrt((FingerSingle.X - FingerDouble.X) * (FingerSingle.X - FingerDouble.X) + (FingerSingle.Y - FingerDouble.Y) * (FingerSingle.Y - FingerDouble.Y)); }//双指距离

        Point FingerStart;//起始点
        Point FingerStartSingle;//起始单数点
        Point FingerStartDouble;//起始双数点

        //指针进入
        public void Control_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //Ruler：标尺线
            if (App.Model.isRuler == true)
                RulerHorizontal.Visibility = RulerVertical.Visibility = Visibility.Visible;
            else
                RulerHorizontal.Visibility = RulerVertical.Visibility = Visibility.Collapsed;
        }
        //指针退出
        public void Control_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Control_PointerReleased(sender,e);
        }



        //指针按下
        public void Control_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.Model.Pressure = Judge.Pressure(e, Control);

            Point p = Judge.Position(e, Control);

            if (Judge.IsTouch(e, Control))
            {
                FingerCollection.Add(e.Pointer.PointerId);
                if (e.Pointer.PointerId % 2 == 0) FingerDouble = p;
                if (e.Pointer.PointerId % 2 == 1) FingerSingle = p;

                if (FingerCollection.Count > 1)
                {
                    if (isDouble == false) //如果双指未开始
                    {
                        FingerStartDouble = FingerDouble;
                        FingerStartSingle = FingerSingle;
                    }
                }
                else
                {
                    if (isSingle == false && isThumb == false)//如果单指未开始
                        FingerStart = p;
                }
            }
            else
            {
                if (Judge.IsRight(e, Control))
                {
                    if (isRight == false)
                    {
                        isRight = true;
                        Right_Start(p);
                    }
                }
                if (Judge.IsLeft(e, Control) || Judge.IsPen(e, Control))
                {
                    if (isSingle == false && isThumb == false)
                    {
                        isSingle = true;
                        Single_Start(p);
                    }
                }
            }
        }
        //指针松开
        public void Control_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Point p = Judge.Position(e, Control);

            if (isRight == true)
            {
                isRight = false;
                Right_Complete(p);
            }
            if (isDouble == true)
            {
                isDouble = false;
                Double_Complete(p, FingerDistance);

                isSingle = false;//阻止单指结束
            }
            else if (isSingle == true && isThumb == false)
            {
                isSingle = false;
                Single_Complete(p);
            }

            FingerCollection.Clear();
        }




        //指针移动
        public void Control_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            App.Model.Pressure = Judge.Pressure(e, Control);

            Point p = Point = Judge.Position(e, Control);

            //Ruler：标尺线
            if (App.Model.isRuler == true)
            {
                RulerHorizontal.X1 = RulerHorizontal.X2 = p.X;
                RulerVertical.Y1 = RulerVertical.Y2 = p.Y;
            }

            if (Judge.IsTouch(e, Control))
            {
                if (e.Pointer.PointerId % 2 == 0) FingerDouble = p;
                if (e.Pointer.PointerId % 2 == 1) FingerSingle = p;

                if (FingerCollection.Count > 1)
                {
                    if (isDouble == false) //如果双指未开始
                    {
                        if (Math.Abs(FingerStartDouble.X - FingerDouble.X) > 2 ||
                            Math.Abs(FingerStartDouble.Y - FingerDouble.Y) > 2 ||
                            Math.Abs(FingerStartSingle.X - FingerSingle.X) > 2 ||
                            Math.Abs(FingerStartSingle.Y - FingerSingle.Y) > 2)//中点移动4
                        {
                            isDouble = true;
                            Double_Start(FingerCenter, FingerDistance);
                        }
                    }
                    else if (isDouble == true)
                    {
                        Double_Delta(FingerCenter, FingerDistance);
                    }
                }
                else
                {
                    if (isSingle == false && isThumb == false) //如果单指未开始
                    {
                        var d = Method.两点距(FingerStart, p);
                        if (d > 2 && d < 12)//点移动4到20
                        {
                            isSingle = true;
                            Single_Start(p);
                        }
                    }
                    else if (isSingle == true && isThumb == false)
                    {
                        Single_Delta(p);
                    }
                }
            }
            else
            {
                if (isRight == true)
                    Right_Delta(p);

                if (isSingle == true && isThumb == false)
                    Single_Delta(p);
            }
        }
        //滚轮变化
        private void Control_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            Wheel_Changed(Judge.Position(e, Control), Judge.WheelDelta(e, Control));
        }


        #endregion

        #region Event：封装事件


        Point PointStart;//开始点p（封装事件可以使用，用作开始的点）

        //是否按下
        bool isLine, isFree, isImage, isWord;
        bool isCircle, isRect, isRoundRect, isTriangle;
        bool isDiamond, isPentagon, isStar, isPie;
        bool isCog, isArrow, isCapsule, isHeart;

        bool isLiquify;
        bool isRuler;

        bool isHand,isCursor,isStraw,isMagic;
        bool isRectangle, isEllipse,isPolygon, isLasso, isContains;//Contains：包含（是否点中套索区域，决定是否移动套索图形）
        bool isMaskPaint, isMaskEraser, isSmudge, isMixer;
        bool isPaint, isPen, isEraser;

        bool isLighting;

        bool isCrop,isGradient,isFade,isText,isTransform;


        //单指&&左键&&笔
        private void Single_Start(Point p)
        {
            PointStart = p;
            CanvasPointStart = App.Model.ScreenToCanvas(p);
            int x = (int)CanvasPointStart.X;
            int y = (int)CanvasPointStart.Y;

            //Ruler：标尺线
            if (App.Model.isRuler == true && (p.X < RulerSpace || p.Y < RulerSpace))
            {
                Ruler_Start(p); isRuler = true; return;
            }

            //Straw：临时取色
            if (App.Model.StrawVisibility == Visibility.Collapsed)
            {
                Straw_Start(p); return;
            }


            //Contains：包含（选区模式为主，工具为4或5或7，点击在套索图形内）
            if (App.Model.MaskMode == 0)
            {
                if (App.Model.Tool == 4 || App.Model.Tool == 5 || App.Model.Tool == 7)
                {
                    if (x > 0 && x < App.Model.Width && y > 0 && y < App.Model.Height)
                    {
                        if (App.Model.MaskRenderTarget.GetPixelColors(x, y, 1, 1).Single().A > 10)
                        {
                            Contains_Start(p);
                            isContains = true;
                            return;
                        }
                    }
                }
            }


            switch (App.Model.Tool)
            {
                case -3: if (isLiquify == false) Liquify_Start(p); isLiquify = true; return;

                case 0: if (isHand == false) Right_Start(p); isHand = true; return;
                case 1: if (isCursor == false) Cursor_Start(p); isCursor = true; return;
                case 2: if (isStraw == false) Straw_Start(p); isStraw = true; return;
                case 3: if (isMagic == false) Magic_Start(p); isMagic = true; return;

                case 4: if (isRectangle == false) Rectangle_Start(p); isRectangle = true; return;
                case 5: if (isEllipse == false) Ellipse_Start(p); isEllipse = true; return;
                case 6: if (isPolygon == false) Polygon_Start(p); isPolygon = true; return;
                case 7: if (isLasso == false) Lasso_Start(p); isLasso = true; return;

                case 8: if (isMaskPaint == false) MaskPaint_Start(p); isMaskPaint = true; return;
                case 9: if (isMaskEraser == false) MaskEraser_Start(p); isMaskEraser = true; return;

                 case 14: if (isPen == false) Pen_Start(p); isPen = true; return;
                case 218: if (isLighting == false) Lighting_Start(p); isLighting = true; return;
 
                case 300: if (isCrop == false) Crop_Start(p); isCrop = true; return;
                case 303: if (isGradient == false) Gradient_Start(p); isGradient = true; return;
                case 304: if (isFade == false) Fade_Start(p); isFade = true; return;
                case 305: if (isText == false) Text_Start(p); isText = true; return;
                case 308: case 102: case 400: case 111: if (isTransform == false) Transform_Start(p); isTransform = true; return;
             }


            //Layer：所有需要当前图层可视的操作
            if (App.Model.Layers[App.Model.Index].Visual == true)
            {
                switch (App.Model.Tool)
                {
                    case 10: if (isSmudge == false) Smudge_Start(p); isSmudge = true; return;
                    case 11: if (isMixer == false) Mixer_Start(p); isMixer = true; return;

                    case 12: case -4: if (isPaint == false) Paint_Start(p); isPaint = true; return;
                    case 15: if (isEraser == false) Eraser_Start(p); isEraser = true; return;


                    case 500: if (isLine == false) Line_Start(p); isLine = true; return;
                    case 501: if (isFree == false) Free_Start(p); isFree = true; return;
                    case 502: if (isImage == false) Image_Start(p); isImage = true; return;
                    case 503: if (isWord == false) Word_Start(p); isWord = true; return;

                    case 504: if (isCircle == false) Circle_Start(p); isCircle = true; return;
                    case 505: if (isRect == false) Rect_Start(p); isRect = true; return;
                    case 506: if (isRoundRect == false) RoundRect_Start(p); isRoundRect = true; return;
                    case 507: if (isTriangle == false) Triangle_Start(p); isTriangle = true; return;

                    case 508: if (isDiamond == false) Diamond_Start(p); isDiamond = true; return;
                    case 509: if (isPentagon == false) Pentagon_Start(p); isPentagon = true; return;
                    case 510: if (isStar == false) Star_Start(p); isStar = true; return;
                    case 511: if (isPie == false) Pie_Start(p); isPie = true; return;

                    case 512: if (isCog == false) Cog_Start(p); isCog = true; return;
                    case 513: if (isArrow == false) Arrow_Start(p); isArrow = true; return;
                    case 514: if (isCapsule == false) Capsule_Start(p); isCapsule = true; return;
                    case 515: if (isHeart == false) Heart_Start(p); isHeart = true; return;

                    default: return;
                }
            }
            else App.Tip(App.resourceLoader.GetString("/Layer/Hided_"));

        }
        private void Single_Delta(Point p)
        {
            //Ruler：标尺线
            if (isRuler == true)
            {
                Ruler_Delta(p); return;
            }

            //Straw：临时取色
            if (App.Model.StrawVisibility == Visibility.Collapsed)
            {
                Straw_Delta(p);
                return;
            }

            if (isContains == true)
            {
                Contains_Delta(p);
                return;
            }


            switch (App.Model.Tool)
            {
                case -3: if (isLiquify == true) Liquify_Delta(p); return;


                case 0: if (isHand == true) Right_Delta(p); return;
                case 1: if (isCursor == true) Cursor_Delta(p); return;
                case 2: if (isStraw == true) Straw_Delta(p); return;
                case 3: if (isMagic == true) Magic_Delta(p); return;

                case 4: if (isRectangle == true) Rectangle_Delta(p); return;
                case 5: if (isEllipse == true) Ellipse_Delta(p); return;
                case 6: if (isPolygon == true) Polygon_Delta(p); return;
                case 7: if (isLasso == true) Lasso_Delta(p); return;

                case 8: if (isMaskPaint == true) MaskPaint_Delta(p); return;
                case 9: if (isMaskEraser == true) MaskEraser_Delta(p); return;

                case 10: if (isSmudge == true) Smudge_Delta(p); return;
                case 11: if (isMixer == true) Mixer_Delta(p); return;
                case 12: case -4: if (isPaint == true) Paint_Delta(p); return;
                case 14: if (isPen == true) Pen_Delta(p); return;
                case 15: if (isEraser == true) Eraser_Delta(p); return;


                case 218: if (isLighting == true) Lighting_Delta(p); return;


                case 300: if (isCrop == true) Crop_Delta(p); return;
                case 303: if (isGradient == true) Gradient_Delta(p); return;
                case 304: if (isFade == true) Fade_Delta(p); return;
                case 305: if (isText == true) Text_Delta(p); return;
                case 308: case 102: case 400: case 111: if (isTransform == true) Transform_Delta(p); return;


                case 500: if (isLine == true) Line_Delta(p); return;
                case 501: if (isFree == true) Free_Delta(p); return;
                case 502: if (isImage == true) Image_Delta(p); return;
                case 503: if (isWord == true) Word_Delta(p); return;

                case 504: if (isCircle == true) Circle_Delta(p); return; 
                case 505: if (isRoundRect == true) RoundRect_Delta(p); return;
                case 506: if (isRect == true) Rect_Delta(p); return;
                case 507: if (isTriangle == true) Triangle_Delta(p); return;

                case 508: if (isDiamond == true) Diamond_Delta(p); return;
                case 509: if (isPentagon == true) Pentagon_Delta(p); return;
                case 510: if (isStar == true) Star_Delta(p); return;
                case 511: if (isPie == true) Pie_Delta(p); return;

                case 512: if (isCog == true) Cog_Delta(p); return;
                case 513: if (isArrow == true) Arrow_Delta(p); return;
                case 514: if (isCapsule == true) Capsule_Delta(p); return;
                case 515: if (isHeart == true) Heart_Delta(p); return;

                default: return;
            }
        }
        private void Single_Complete(Point p)
        {
            //Ruler：标尺线
            if (isRuler == true)
            {
                Ruler_Complete(p);
                isRuler = false;
                return;
            }

            //Straw：临时取色
            if (App.Model.StrawVisibility == Visibility.Collapsed)
            {
                Straw_Complete(p);
                return;
            }

            if (isContains == true)
            {
                Contains_Complete(p);
                isContains = false;
                return;
            }

            switch (App.Model.Tool)
            {

                case -3: if (isLiquify == true) Liquify_Complete(p); isLiquify = false; return;

                case 0: if (isHand == true) Right_Complete(p); isHand = false; return;
                case 1: if (isCursor == true) Cursor_Complete(p); isCursor = false; return;
                case 2: if (isStraw == true) Straw_Complete(p); isStraw = false; return;
                case 3: if (isMagic == true) Magic_Complete(p); isMagic = false; return;

                case 4: if (isRectangle == true) Rectangle_Complete(p); isRectangle = false; return;
                case 5: if (isEllipse == true) Ellipse_Complete(p); isEllipse = false; return;
                case 6: if (isPolygon == true) Polygon_Complete(p); isPolygon = false; return;
                case 7: if (isLasso == true) Lasso_Complete(p); isLasso = false; return;

                case 8: if (isMaskPaint == true) MaskPaint_Complete(p); isMaskPaint = false; return;
                case 9: if (isMaskEraser == true) MaskEraser_Complete(p); isMaskEraser = false; return;

                case 10: if (isSmudge == true) Smudge_Complete(p); isSmudge = false; return;
                case 11: if (isMixer == true) Mixer_Complete(p); isMixer = false; return;
                case 12: case -4: if (isPaint == true) Paint_Complete(p); isPaint = false; return;
                case 14: if (isPen == true) Pen_Complete(p); isPen = false; return;
                case 15: if (isEraser == true) Eraser_Complete(p); isEraser = false; return;


                case 218: if (isLighting == true) Lighting_Complete(p); isLighting = false; return;


                case 300: if (isCrop == true) Crop_Complete(p); isCrop = false; return;
                case 303: if (isGradient == true) Gradient_Complete(p); isGradient = false; return;
                case 304: if (isFade == true) Fade_Complete(p); isFade = false; return;
                case 305: if (isText == true) Text_Complete(p); isText = false; return;
                case 308: case 102: case 400: case 111: if (isTransform == true) Transform_Complete(p); isTransform = false; return;


                case 500: if (isLine == true) Line_Complete(p); isLine = false; return;
                case 501: if (isFree == true) Free_Complete(p); isFree = false; return;
                case 502: if (isImage == true) Image_Complete(p); isImage = false; return;
                case 503: if (isWord == true) Word_Complete(p); isWord = false; return;

                case 504: if (isCircle == true) Circle_Complete(p); isCircle = false; return;
                case 505: if (isRect == true) Rect_Complete(p); isRect = false; return;
                case 506: if (isRoundRect == true) RoundRect_Complete(p); isRoundRect = false; return;
                case 507: if (isTriangle == true) Triangle_Complete(p); isTriangle = false; return;

                case 508: if (isDiamond == true) Diamond_Complete(p); isDiamond = false; return;
                case 509: if (isPentagon == true) Pentagon_Complete(p); isPentagon = false; return;
                case 510: if (isStar == true) Star_Complete(p); isStar = false; return;
                case 511: if (isPie == true) Pie_Complete(p); isPie = false; return;

                case 512: if (isCog == true) Cog_Complete(p); isCog = false; return;
                case 513: if (isArrow == true) Arrow_Complete(p); isArrow = false; return;
                case 514: if (isCapsule == true) Capsule_Complete(p); isCapsule = false; return;
                case 515: if (isHeart == true) Heart_Complete(p); isHeart = false; return;

                default: return;
            }
        }




        //移动事件
        private void Move_Start()
        {
            App.Model.isAnimated = false;

            switch (App.Model.Tool)
            {
                case 6: Polygon_Move_Start(); return;
                case 13: Pencil_Move_Start(); return;

                case 218: Lighting_Move_Start(); return;
                case 231: PinchPunch_Move_Start(); return;

                case 300: Crop_Move_Start(); return;
                case 303: Gradient_Move_Start(); return;
                case 304: Fade_Move_Start(); return;
                case 305: Text_Move_Start(); return;
                case 308: case 102: case 400: case 111: Transform_Move_Start(); return;
            }
        }
        private void Move_Delta()
        {
            switch (App.Model.Tool)
            {
                case 6: Polygon_Move_Delta(); return;
                case 13: Pencil_Move_Delta(); return;

                case 218: Lighting_Move_Delta(); return;
                case 231: PinchPunch_Move_Delta(); return;

                case 300: Crop_Move_Delta(); return;
                case 303: Gradient_Move_Delta(); return;
                case 304: Fade_Move_Delta(); return;
                case 305: Text_Move_Delta(); return;
                case 308: case 102: case 400: case 111: Transform_Move_Delta(); return;

                default: return;
            }
        }
        private void Move_Complete()
        {
            App.Model.isAnimated = true;

            switch (App.Model.Tool)
            {
                case 6: Polygon_Move_Complete(); return;
                case 13: Pencil_Move_Complete(); return;

                case 218: Lighting_Move_Complete(); return;
                case 231: PinchPunch_Move_Complete(); return;

                case 300: Crop_Move_Complete(); return;
                case 303: Gradient_Move_Complete(); return;
                case 304: Fade_Move_Complete(); return;
                case 305: Text_Move_Complete(); return;
                case 308: case 102: case 400: case 111: Transform_Move_Complete(); return;

                default: return;
            }
        }


        #endregion

        #region Move：移动事件


        Point CanvasPointStart;//初始点在画布上的位置

        double DoubleDistanceStart;//开始双指距离
        double CanvasWidthStart;// 开始画布宽度
        double CanvasHeightStart;// 开始画布高度

        double WheelScale = 1;//滚轮缩放比例


        //双指
        private void Double_Start(Point p, double d)
        {
            CanvasPointStart = App.Model.ScreenToCanvas(p);//映射到画布上的点
            DoubleDistanceStart = d;
            CanvasWidthStart = App.Model.CanvasWidth;
            CanvasHeightStart = App.Model.CanvasHeight;

            App.Model.TipVisibility = Visibility.Visible;//Tip：全局提示

            Move_Start();//移动事件
        }
        private void Double_Delta(Point p, double d)
        {
            //缩放
            double scale = d / DoubleDistanceStart;
            var Width = CanvasWidthStart * scale;
            var Height = CanvasHeightStart * scale;
            if ((Width > 70 && Height > 70) || scale > 1)//防止画布过小崩溃
            {
                App.Model.CanvasWidth = Width;
                App.Model.CanvasHeight = Height;

                //移动对齐到画布上的点
                App.Model.X = p.X - CanvasPointStart.X * App.Model.XS;
                App.Model.Y = p.Y - CanvasPointStart.Y * App.Model.YS;
            }

           App.Model.Refresh++;
            App.Model.isReStroke = true;//重新设置描边

            App.Model.Tip = ((int)(App.Model.XS * 100)).ToString() + "%";//Tip：全局提示

            Move_Delta();//移动事件
        }
        private void Double_Complete(Point p, double d)
        {
            App.Model.TipVisibility = Visibility.Collapsed;//Tip：全局提示

            Move_Complete();//移动事件
        }

        //缓存：性能优化
        double RightCacheX;
        double RightCacheY;
        //右键
        private void Right_Start(Point p)
        {
            CanvasPointStart = App.Model.ScreenToCanvas(p);

            //缓存
            RightCacheX = App.Model.X;
            RightCacheY = App.Model.Y;

            Move_Start();//移动事件
        }
        private void Right_Delta(Point p)
        {
            //移动对齐
            App.Model.X = p.X - CanvasPointStart.X * App.Model.XS;
            App.Model.Y = p.Y - CanvasPointStart.Y * App.Model.YS;

            //缓存
            if (Math.Abs(RightCacheX - App.Model.X) + Math.Abs(RightCacheY - App.Model.Y) > 10)
            {
                RightCacheX = App.Model.X;
                RightCacheY = App.Model.Y;

                App.Model.Refresh++;
                App.Model.isReStroke = true;//重新设置描边
            }

            Move_Delta();//移动事件
        }
        private void Right_Complete(Point p)
        {
            App.Model.Refresh++;
            App.Model.isReStroke = true;//重新设置描边

            Move_Complete();//移动事件
        }


        //滚轮
        private void Wheel_Changed(Point p, double d)
        {
            //缩放
            if (d > 0) WheelScale = 1.1;
            else if (d < 0) WheelScale = 1 / 1.1;

            var Width = App.Model.CanvasWidth * WheelScale;
            var Height = App.Model.CanvasHeight * WheelScale;
            if ((Width > 70 && Height > 70) || WheelScale > 1)//防止画布过小崩溃
            {
                App.Model.CanvasWidth = Width;
                App.Model.CanvasHeight = Height;

                //当滚轮被按下时被识别为右键按下
                if (isRight == true) Right_Delta(p);
                else
                {
                    App.Model.X = (p.X - (p.X - App.Model.X) * WheelScale);
                    App.Model.Y = (p.Y - (p.Y - App.Model.Y) * WheelScale);
                }
            }

          App.Model.Refresh++;
            App.Model.isReStroke = true;//重新设置描边

            App.Tip(((int)(App.Model.XS * 100)).ToString() + "%");//Tip：全局提示

            Move_Delta();//移动事件
        }


        #endregion


        #region DrawRectEllipse：圆环矩形


        Rect DrawRect;//一个矩形，负责更新虚拟画布，以及绘画圆环

        //返回一个安全区域，使更新区域不超过虚拟画布的边界
        private Rect DrawRectVirtual(Rect r)
        {
            double X = r.X > 0 ? r.X : 0;
            double Y = r.Y > 0 ? r.Y : 0;
            double W = X + r.Width < this.CanvasVirtualControl222.ActualWidth ? r.Width : this.CanvasVirtualControl222.ActualWidth - X;
            double H = Y + r.Height < this.CanvasVirtualControl222.ActualHeight ? r.Height : this.CanvasVirtualControl222.ActualHeight - Y;

            return new Rect(X, Y, W, H);
        }

        // 设置屏幕绘画矩形区域（P：Position 、R：radius ）
        private void DrawRectSet(Point P, double R)
        {
            DrawRect.Width = App.Model.XS * R * 2;
            DrawRect.Height = App.Model.YS * R * 2;

            DrawRectFollow(P);
        }
        // 跟随屏幕绘画矩形区域
        private void DrawRectFollow(Point P)
        {
            DrawRect.X = P.X - DrawRect.Width / 2;
            DrawRect.Y = P.Y - DrawRect.Height / 2;
        }



        //Ellipse DrawEllipse;//屏幕上的圆环

        // 设置绘画圆环
        private void DrawEllipseSet()
        {
            DrawEllipse.Width = DrawRect.Width / 2;
            DrawEllipse.Height = DrawRect.Height / 2;

            DrawEllipseFollow();
            DrawEllipse.Visibility = Visibility.Visible;
        }
        // 跟随绘画圆环
        private void DrawEllipseFollow()
        {
            Canvas.SetLeft(DrawEllipse, DrawRect.X + DrawRect.Width / 4);
            Canvas.SetTop(DrawEllipse, DrawRect.Y + DrawRect.Height / 4);
        }
        private void DrawEllipseFollow(Point p)
        {
            Canvas.SetLeft(DrawEllipse, p.X- DrawRect.Width / 4);
            Canvas.SetTop(DrawEllipse, p.Y- DrawRect.Height / 4);
        }
        // 跟随绘画圆环
        private void DrawEllipseCollapsed()
        {
            DrawEllipse.Visibility = Visibility.Collapsed;
        }





        #endregion

        #region Tip ：提示


        private void Tip(Point p, string s)
        {
            Canvas.SetLeft(TipGrid, p.X - TipGrid.ActualWidth / 2);
            Canvas.SetTop(TipGrid, p.Y - 34);

            TipText.Text = s;
            TipGrid.Visibility = Visibility.Visible;
        }

        private void Tip(double x, double y, string s)
        {
            Canvas.SetLeft(TipGrid, x - TipGrid.ActualWidth / 2);
            Canvas.SetTop(TipGrid, y - 34);

            TipText.Text = s;
            TipGrid.Visibility = Visibility.Visible;
        }



        private void Tip(Point p, float n)
        {
            Canvas.SetLeft(TipGrid, p.X - TipGrid.ActualWidth / 2);
            Canvas.SetTop(TipGrid, p.Y - 34);

            var m = (float)((int)(n * 100)) / 100;
            TipText.Text = m.ToString();
            TipGrid.Visibility = Visibility.Visible;
        }

        private void Tip(double x, double y, float n)
        {
            Canvas.SetLeft(TipGrid, x - TipGrid.ActualWidth / 2);
            Canvas.SetTop(TipGrid, y - 34);

            var m = (float)((int)(n * 100)) / 100;
            TipText.Text = m.ToString() + "%";
            TipGrid.Visibility = Visibility.Visible;
        }

        private void Tip(double x, double y, float n, string s)
        {
            Canvas.SetLeft(TipGrid, x - TipGrid.ActualWidth / 2);
            Canvas.SetTop(TipGrid, y - 34);

            var m = (float)((int)(n * 100)) / 100;
            TipText.Text = m.ToString() + s;
            TipGrid.Visibility = Visibility.Visible;
        }


        #endregion

        private void CanvastoBlend()
        {
            var ds = App.Model.MainRenderTarget.CreateDrawingSession();

            //
                // 摘要：正常绘画
                //    
                //默认值，正常的颜色叠加
                ds.Blend = CanvasBlend.SourceOver;

                //
                // 摘要：源图像素被覆盖（用来绘画可能整个图层都会消失）
                //    
                //目标透明源图不透明情况下，源图变为透明度目标；其他情况下正常
                ds.Blend = CanvasBlend.Copy;

                //
                // 摘要：取亮度最小值（用来绘画会过载崩溃）
                //    
                //目标更暗源图更亮时正常，目标更亮源图更暗时没有反应，目标不透明时没有反应
                ds.Blend = CanvasBlend.Min;

                //
                // 摘要：像素亮度的叠加（很迷的效果）
                //    
                //目标源图不透明有亮度叠加像素亮度，目标黑色任何源图没有反应，目标不透明时正常
                ds.Blend = CanvasBlend.Add;

                ds.DrawImage(App.Setting.Paint);
         }


        #region Event封装事件：直线Line


        Vector2 GeometryStart;//起始点

        private void Line_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Line_Delta(Point p)
        {

        }
        private void Line_Complete(Point p)
        {
             Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                ds.DrawLine(GeometryStart, v, App.Model.StrokeColor, App.Setting.GeometryWidth,App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：自由线Free


        List<Vector2> FreeList = new List<Vector2> { };

        private void Free_Start(Point p)
        {
            // GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();
            FreeList.Add(p.ToVector2());

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Free_Delta(Point p)
        {
            FreeList.Add(p.ToVector2());
        }
        private void Free_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                for (int i = 0; i < FreeList.Count; i++)
                {
                    FreeList[i] = App.Model.ScreenToCanvas(FreeList[i]);
                }
                CanvasGeometry g = CanvasGeometry.CreatePolygon(App.Model.VirtualControl, FreeList.ToArray());
                FreeList.Clear();

                ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }






        #endregion

        #region Event封装事件：图像Image
         

        private void Image_Start(Point p)
        {
            if (App.Setting.Image != null)
            {
                GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);
            }
        }
        private void Image_Delta(Point p)
        {

        }
        private void Image_Complete(Point p)
        {
            if (App.Setting.Image!=null)
            {
                 Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
                using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                {
                    float X, Y, W, H;

                    if (App.Setting.isImageRatio == true)
                    {
                        float us = Math.Abs(GeometryStart.X - v.X) + Math.Abs(GeometryStart.Y - v.Y);
                        float cale = us / (App.Setting.ImageWidth + App.Setting.ImageHeight);

                        W = App.Setting.ImageWidth * cale;
                        H = App.Setting.ImageHeight * cale;

                        X = GeometryStart.X < v.X ? GeometryStart.X : GeometryStart.X - W;
                        Y = GeometryStart.Y < v.Y ? GeometryStart.Y : GeometryStart.Y - H;
                    }
                    else
                    {
                        W = Math.Abs(GeometryStart.X - v.X);
                        H = Math.Abs(GeometryStart.Y - v.Y);

                        X = GeometryStart.X < v.X ? GeometryStart.X : v.X;
                        Y = GeometryStart.Y < v.Y ? GeometryStart.Y : v.Y;
                    }

                    ds.DrawImage(new ScaleEffect
                    {
                        Source = App.Setting.Image,
                        Scale = new Vector2
                         (
                             W / App.Setting.ImageWidth,
                           H / App.Setting.ImageHeight
                         )
                    }, X, Y);
                }

                App.Model.isReRender = true;//重新渲染
                CanvasVirtualControl222.Invalidate();//画布刷新

                App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
            }
        }


        #endregion


        #region Event封装事件：单词Word



        private void Word_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Word_Delta(Point p)
        {
         }
        private void Word_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                var g = 修图.Library.Geometry.CreateWord(App.Model.VirtualControl, GeometryStart, v, App.Setting.WordText, App.Setting.WordFontFamily, App.Setting.WordFontStyle);

                ds.FillGeometry(g.Geometry, g.Vect, App.Model.GeometryColor);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }
          

        #endregion

        #region Event封装事件：圆Circle


        private void Circle_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();
         
            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Circle_Delta(Point p)
        {

        }
        private void Circle_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                float radius = (v - GeometryStart).Length();

                if (App.Setting.GeometryMode != 1) ds.FillCircle(GeometryStart, radius, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawCircle(GeometryStart, radius, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：矩形Rect


        private void Rect_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Rect_Delta(Point p)
        {

        }
        private void Rect_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                float x = Math.Min(GeometryStart.X, v.X);
                float y = Math.Min(GeometryStart.Y, v.Y);
                float w = Math.Abs(GeometryStart.X - v.X);
                float h = Math.Abs(GeometryStart.Y - v.Y);

                if (App.Setting.GeometryMode != 1) ds.FillRectangle(x, y, w, h, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawRectangle(x, y, w, h, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：圆角矩形RoundRect


        private void RoundRect_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void RoundRect_Delta(Point p)
        {

        }
        private void RoundRect_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                float x = Math.Min(GeometryStart.X, v.X);
                float y = Math.Min(GeometryStart.Y, v.Y);
                float w = Math.Abs(GeometryStart.X - v.X);
                float h = Math.Abs(GeometryStart.Y - v.Y);

                if (App.Setting.GeometryMode != 1) ds.FillRoundedRectangle(x, y, w, h, App.Setting.RoundRectRadius, App.Setting.RoundRectRadius, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawRoundedRectangle(x, y, w, h, App.Setting.RoundRectRadius, App.Setting.RoundRectRadius, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：三角形Triangle

         
         private void Triangle_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Triangle_Delta(Point p)
        {

        }
        private void Triangle_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                CanvasGeometry g = 修图.Library.Geometry.CreateTriangle(App.Model.VirtualControl, GeometryStart,v);

                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：菱形Diamond


        private void Diamond_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Diamond_Delta(Point p)
        {

        }
        private void Diamond_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                CanvasGeometry g = 修图.Library.Geometry.CreateDiamond(App.Model.VirtualControl, GeometryStart, v);

                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion


        #region Event封装事件：五边形Pentagon


        private void Pentagon_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Pentagon_Delta(Point p)
        {

        }
        private void Pentagon_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                CanvasGeometry g = 修图.Library.Geometry.CreatePentagon(App.Model.VirtualControl, GeometryStart, v, App.Setting.PentagonCount);

                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：星形Star


        private void Star_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Star_Delta(Point p)
        {

        }
        private void Star_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                CanvasGeometry g = 修图.Library.Geometry.CreateStar(App.Model.VirtualControl, GeometryStart, v, App.Setting.StarCount, App.Setting.StarInner);
                 
                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }


            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：饼图Pie


        private void Pie_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Pie_Delta(Point p)
        {

        }
        private void Pie_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                CanvasGeometry g = 修图.Library.Geometry.CreatePie(App.Model.VirtualControl, GeometryStart, v, App.Setting.PieInner, App.Setting.PieSweep);

                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：齿轮Cog


        private void Cog_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Cog_Delta(Point p)
        {

        }
        private void Cog_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                CanvasGeometry g = 修图.Library.Geometry.CreateCog(App.Model.VirtualControl, GeometryStart, v,App.Setting.CogCount, App.Setting.CogInner, App.Setting.CogTooth, App.Setting.CogNotch);
 
                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion


        #region Event封装事件：箭头Arrow


        private void Arrow_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Arrow_Delta(Point p)
        {

        }
        private void Arrow_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                 CanvasGeometry g = 修图.Library.Geometry.CreateArrow(App.Model.VirtualControl, GeometryStart, v, App.Setting.ArrowHead);

                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：胶囊Capsule


        private void Capsule_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Capsule_Delta(Point p)
        {

        }
        private void Capsule_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                CanvasGeometry g = 修图.Library.Geometry.CreateCapsule(App.Model.VirtualControl, GeometryStart, v, App.Setting.CapsuleRadius);

                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion


        #region Event封装事件：心形Heart


        private void Heart_Start(Point p)
        {
            GeometryStart = App.Model.ScreenToCanvas(p).ToVector2();

            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
        }
        private void Heart_Delta(Point p)
        {

        }
        private void Heart_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                CanvasGeometry g = 修图.Library.Geometry.CreateHeart(App.Model.VirtualControl, GeometryStart, v);

                if (App.Setting.GeometryMode != 1) ds.FillGeometry(g, App.Model.GeometryColor);
                if (App.Setting.GeometryMode != 0) ds.DrawGeometry(g, App.Model.StrokeColor, App.Setting.GeometryWidth, App.Setting.GeometryStrokeStyle);
            }

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion




        #region Liquify：液化


        // 绘画画笔
        private void LiquifyDraw(Vector2 v)
        {
             using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            { 
                //绘画
                ds.DrawImage(App.Setting.Liquify, v.X - App.Setting.LiquifySize, v.Y - App.Setting.LiquifySize);

                //圆环矩形
                DrawRectFollow(App.Model.CanvasToScreen(v.ToPoint()));// 跟随屏幕绘画矩形区域
                DrawEllipseFollow();// 跟随绘画圆环
                App.Model.isReRender = true;//重新渲染
                CanvasVirtualControl222.Invalidate(DrawRectVirtual(DrawRect));//画布局部刷新
             }
        }
         private void LiquifyDraw(Vector2 start, Vector2 end)
        {
            Vector2 space = LiquifyOld - end;

            using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            {
                //两向量的间距，获取插值点数量
                int count = (int)(Math.Abs((start - end).Length()) / (App.Setting.LiquifySize / 2));
                //首末差距的向量
                Vector2 spaceveector = new Vector2((end.X - start.X) / count, (end.Y - start.Y) / count);

                //绘画
                ds.DrawImage(App.Setting.Liquify, start.X - App.Setting.LiquifySize, start.Y - App.Setting.LiquifySize);

                //圆环矩形
                DrawRectFollow(App.Model.CanvasToScreen(start.ToPoint()));// 跟随屏幕绘画矩形区域
                DrawEllipseFollow();// 跟随绘画圆环
                App.Model.isReRender = true;//重新渲染
                CanvasVirtualControl222.Invalidate(DrawRectVirtual(DrawRect));//画布局部刷新

                //插值点，画直线
                for (int i = 0; i < count; i++)
                {
                    start += spaceveector;
                    ds.DrawImage(App.Setting.Liquify, start.X - App.Setting.LiquifySize, start.Y - App.Setting.LiquifySize);

                    //圆环矩形
                    DrawRectFollow(App.Model.CanvasToScreen(start.ToPoint()));// 跟随屏幕绘画矩形区域
                    DrawEllipseFollow();// 跟随绘画圆环
                    App.Model.isReRender = true;//重新渲染
                    CanvasVirtualControl222.Invalidate(DrawRectVirtual(DrawRect));//画布局部刷新
                }
            }
        }


        #endregion

        #region Event封装事件：液化Liquify


        Vector2 LiquifyOld;//旧向
        private void Liquify_Start(Point p)
        {
            LiquifyOld = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.LiquifySize;

             //圆环&矩形
            DrawRectSet(p, R);  // 设置屏幕绘画矩形区域
            DrawEllipseSet(); // 设置绘画圆环 
        }
        private void Liquify_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.LiquifySize;

            if (App.Setting.LiquifyMode == 0) App.Setting.LiquifyDraw(App.Model.VirtualControl, LiquifyOld, v);//改变颜色分量R与B
            LiquifyDraw(LiquifyOld, v); // 绘画

            LiquifyOld = v;
        }
        private void Liquify_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            LiquifyDraw (v); // 绘画

            DrawEllipseCollapsed(); //圆环矩形
            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion

        #region Event封装事件：标尺线Ruler


        private void Ruler_Start(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

             if (p.X < 20)//左侧垂直标尺
            {
                 for (int i = 0; i < App.Setting.RulerVerticalList.Count; i++) //遍历寻找点击的点
                {
                    if (Math.Abs(v.Y - App.Setting.RulerVerticalList[i]) < 20 * App.Model.SY)//判断距离
                    {
                        App.Setting.RulerVerticalIndex = i;
                        App.Setting.isRulerVertical = false;//关闭垂直移动状态
                         return;
                    }
                }
                App.Setting.isRulerVertical = true;//进入垂直移动状态
                App.Setting.RulerVerticalList.Add(v.Y);//添加
            }
            if (p.Y < 20)//顶侧水平标尺
            {
                for (int i = 0; i < App.Setting.RulerHorizontalList.Count; i++)
                {
                    if (Math.Abs(v.X - App.Setting.RulerHorizontalList[i]) < 20 * App.Model.SX)//判断距离
                    {
                        App.Setting.RulerHorizontalIndex = i;
                        App.Setting.isRulerHorizontal = false;//关闭水平移动状态
                        return;
                    }
                }
                App.Setting.isRulerHorizontal = true;//进入水平移动状态
                App.Setting.RulerHorizontalList.Add(v.X);//添加
            }

            CanvasVirtualControl222.Invalidate();//画布刷新
        }
        private void Ruler_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            //添加
            if (App.Setting.isRulerVertical == true)
                App.Setting.RulerVerticalList[App.Setting.RulerVerticalList.Count - 1] = v.Y;//最后一个跟随
            if (App.Setting.isRulerHorizontal == true)
                App.Setting.RulerHorizontalList[App.Setting.RulerHorizontalList.Count - 1] = v.X;//最后一个跟随

            //移动
            if (App.Setting.RulerVerticalIndex >= 0)
                App.Setting.RulerVerticalList[App.Setting.RulerVerticalIndex] = v.Y;//垂直选中点跟随
            if (App.Setting.RulerHorizontalIndex >= 0)
                App.Setting.RulerHorizontalList[App.Setting.RulerHorizontalIndex] = v.X;//水平选中点跟随

            CanvasVirtualControl222.Invalidate();//画布刷新
         }
        private void Ruler_Complete(Point p)
        {
            //移动
            if (App.Setting.RulerVerticalIndex >= 0 && p.Y < 20)
                App.Setting.RulerVerticalList.RemoveAt(App.Setting.RulerVerticalIndex);//垂直选中点移除
            if (App.Setting.RulerHorizontalIndex >= 0 && p.X < 20)
                App.Setting.RulerHorizontalList.RemoveAt(App.Setting.RulerHorizontalIndex);//水平选中点移除

            App.Setting.isRulerVertical = false;//退出垂直移动状态
            App.Setting.isRulerHorizontal = false;//退出水平移动状态

            App.Setting.RulerVerticalIndex = -1;//清空索引
            App.Setting.RulerHorizontalIndex = -1;//清空索引

            CanvasVirtualControl222.Invalidate();//画布刷新
        }


        #endregion


        #region Event封装事件：光标Cursor


         bool isCursorMove = false;//光标处于移动图层状态还是画矩形状态

        private void Cursor_Start(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            //初始化
            CursorStart = CursorEnd = v;//Rect：矩形
            isCursorMove = false;

            //判断
            for (int i = 0; i < App.Model.Layers.Count; i++)
            {
                Layer l = App.Model.Layers[i];

                if (l.Opacity>10&&l.Visual==true)
                {
                    //负的矩阵，获得实际的点击的位置
                    Vector2 lv = new Vector2(v.X - l.Vect.X, v.Y - l.Vect.Y);//LayerVector:当前图层
                    int X = (int)lv.X;
                    int Y = (int)lv.Y;

                    if (X >= 0 && X < App.Model.Width && Y >= 0 && Y < App.Model.Height)
                    {
                        if (l.CanvasRenderTarget.GetPixelColors(X, Y, 1, 1).Single().A > 10)
                        {
                            //选定
                            App.Model.Index = i;
                            isCursorMove = true;
                            CursorMove_Start(v);
                             break;
                        }
                    }
                }
             }
        }
        private void Cursor_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            if (isCursorMove == true)
            {
                CursorMove_Delta(v);//Move：移动

                App.Model.isReRender = true;//重新渲染 
                CanvasVirtualControl222.Invalidate();//画布刷新
            }
            else
            {
                CursorRect_Delta(v);//Rect：矩形

                CanvasVirtualControl222.Invalidate();//画布刷新
            }
        }
        private void Cursor_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            if (isCursorMove == true) CursorMove_Complete(v);//Move：移动
            else CursorRect_Complete(v);//Rect：矩形

            //如果点击点的距离太近就清空
            if (Math.Abs(p.X-PointStart.X)<10&& Math.Abs(p.Y - PointStart.Y) < 10)
            {
                 foreach (var layer in App.Model.Layers)
                {
                    layer.isSelected = false;
                }
            }
            isCursorMove = false;
            CanvasVirtualControl222.Invalidate();
        }


        #endregion

        #region Cursor：移动


        //Move
        Vector2 CursorOld;//移动旧点

        private void CursorMove_Start(Vector2 v)
        {
            CursorOld = v;//旧点

            App.Setting.CursorGroupStart = new Point(App.Model.Layers[App.Model.Index].VectRect.Left, App.Model.Layers[App.Model.Index].VectRect.Top);
            App.Setting.CursorGroupEnd = new Point(App.Model.Layers[App.Model.Index].VectRect.Right, App.Model.Layers[App.Model.Index].VectRect.Bottom);

            App.Setting.CursorGroup();//更改选中组的矩形点
        }
        private void CursorMove_Delta(Vector2 v)
        {
            Vector2 changed = v - CursorOld;

            //移动选中的图层
            foreach (var layer in App.Model.Layers)
            {
                if (layer.isSelected==true || App.Model.Layers.IndexOf(layer)== App.Model.Index) layer.Vect += changed;
            }
            //移动显示矩形
            App.Setting.CursorGroupStart.X += changed.X;
            App.Setting.CursorGroupStart.Y += changed.Y;
            App.Setting.CursorGroupEnd.X += changed.X;
            App.Setting.CursorGroupEnd.Y += changed.Y;
 
            CursorOld = v;
        }
        private void CursorMove_Complete(Vector2 v)
        {
            App.Model.isCursor = false;//光标不可用

            foreach (var l in App.Model.Layers)
            {
                if (l.isSelected == true && (l.Vect.X != 0 || l.Vect.Y != 0)) App.Model.isCursor = true;//光标可用
             }
            if (App.Model.Layers[App.Model.Index].Vect.X!=0) App.Model.isCursor = true;//光标可用
         }


        #endregion
 
        #region Cursor：矩形


        //Rect：矩形
        Vector2 CursorStart = new Vector2();
        Vector2 CursorEnd = new Vector2();
        
        private void CursorRect_Delta(Vector2 v)
        {
            CursorEnd = v;//移动钢笔矩形


            //遍历图层决定是否选中
            foreach (var l in App.Model.Layers)//判断钢笔矩形内的点
            {
                if (Math.Min(CursorStart.X, CursorEnd.X) < l.VectRect.Left &&
                    Math.Min(CursorStart.Y, CursorEnd.Y) < l.VectRect.Top &&
                    Math.Max(CursorStart.X, CursorEnd.X) > l.VectRect.Right &&
                     Math.Max(CursorStart.Y, CursorEnd.Y) > l.VectRect.Bottom)
                {
                    l.isSelected = true;
                }
                else l.isSelected = false;
            }
           
            App.Setting.CursorGroup();//更改选中组的矩形点

            //判断光标可用
            App.Model.isCursor = false;//光标可用
            foreach (var l in App.Model.Layers)
            {
                if (l.isSelected == true && (l.Vect.X != 0 || l.Vect.Y != 0))
                    App.Model.isCursor = true;//光标可用
            }
        }
        private void CursorRect_Complete(Vector2 v)
        {
            CursorRect_Delta(v);

            //清空数据
            CursorStart.X = CursorStart.Y = CursorEnd.X = CursorEnd.Y = 0;
         }


        #endregion

        #region Event封装事件：吸管Straw


        private void Straw_Start(Point p)
        {
            StrawMargin(p);
            StrawColor(p);
            StrawImage(p);
            StrawShow.Begin();

            isStrawImage = true;//Image：吸管缩略图
        }
        private void Straw_Delta(Point p)
        {
            StrawMargin(p);
            StrawColor(p);
            StrawImage(p);
        }
        private void Straw_Complete(Point p)
        {
            //Straw：临时取色
            if (App.Model.StrawVisibility == Visibility.Collapsed)
                App.Model.StrawVisibility = Visibility.Visible;

            StrawMargin(p);
            StrawColor(p);
            StrawImage(p);
            StrawFade.Begin();

            isStrawImage = false;
        }


        #endregion

        #region Straw：吸管


        //Grid：拖动
        Thickness StrawGridThickness = new Thickness();
        private void StrawMargin(Point p)
        {
            StrawGridThickness.Top = Point.Y - StrawGrid.Height;
            StrawGridThickness.Left = Point.X - StrawGrid.Width / 2;
            StrawGrid.Margin = StrawGridThickness;
        }



        //Color：变色
        private void StrawColor(Point p)//主取色
        {
            Point CP = App.Model.ScreenToCanvas(p);
            Color cooo;

            int X = (int)CP.X;
            int Y = (int)CP.Y;
            if (X > 0 && X < App.Model.Width && Y > 0 && Y < App.Model.Height)
            {
                //当前图层还是全局图层
                if (App.Setting.isStrawCurrent==false)     cooo = App.Model.MainRenderTarget.GetPixelColors(X, Y, 1, 1).Single();
                else cooo = App.Model.CurrentRenderTarget.GetPixelColors(X, Y, 1, 1).Single();
            }
            else
                cooo = App.Model.Background.Color;

            if (App.Model.StrawVisibility == Visibility.Collapsed)
            {
                switch (App.Model.Tool)
                {
                    case -4:  case 12:
                        App.Model.Color = cooo;
                         if (App.Setting.isPaintBitmap == false)
                        {
                            App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintWidth, App.Model.Color);
                        }
                        else
                        {
                            App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintBitmap, App.Setting.PaintWidth, App.Model.Color);
                        }
                        break;

                    case 14:
                        App.Model.PenColor = cooo;
                        break;

                    case 213:
                        App.Model.ShadowColor = cooo;
                        修图.BarPage.EffectPage1.Shadow.Render();//阴影
                         break;

                    case 214:
                        App.Model.ChromaKeyColor = cooo;
                        修图.BarPage.EffectPage1.ChromaKey.Render();//消除颜色
                        break;

                    case 221:
                        App.Model.VignetteColor = cooo;
                        修图.BarPage.EffectPage2.Tint.Render();//色彩匹配
                        break;

                    case 305:
                        App.Model.TextColor = cooo;
                        修图.BarPage.OtherPage.Text.Render();//文本
                        break;

                    case 303:
                        App.Setting.GradientSetCurrent(cooo);
                         修图.BarPage.OtherPage.Gradient.Render();//渐变
                        break;

                    case 307:
                        App.Model.FillColor = cooo;
                        修图.BarPage.OtherPage.Fill.Render();//填充
                        break;


                    case 503://Word：字词
                        App.Model.GeometryColor = cooo;
                        break;
                    case 500:case 501://Line：直线 //Free：自由线
                        App.Model.StrokeColor = cooo;
                        break;

                    //Gemology：几何
                    case 504:case 505:case 506:case 507:
                    case 508:case 509:case 510:case 511:
                    case 512:case 513:case 514:case 515:
                        if (App.Setting.GeometryMode == 0) App.Model.GeometryColor = cooo;
                        else App.Model.StrokeColor = cooo;
                        break;

                    default: App.Model.Color = cooo; break;
                }
            }
            else  App.Model.Color = cooo; 
        }
 

        //Image：缩略图
        bool isStrawImage;//是否开始渲染缩略图
         Point StrawPoint;//吸管吸取的源图点
        private void StrawImage(Point p)
        {
            StrawPoint = App.Model.ScreenToImage(p);
  
             StrawCanvas.Invalidate();
        }
        private void StrawCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (isStrawImage == true)
                args.DrawingSession.DrawImage(
                    new ScaleEffect
                    {
                        Source = App.Model.MainRenderTarget,
                        Scale = new Vector2((float)App.Model.XS * 2f)
                    },
                    (float)(-2 * StrawPoint.X + StrawCanvasGrid.ActualWidth / 2),
                    (float)(-2 * StrawPoint.Y + StrawCanvasGrid.ActualHeight / 2));
        }

        #endregion

        #region Event封装事件：魔棒Magic


        private void Magic_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);
        }
        private void Magic_Delta(Point p)
        {
        }
        private void Magic_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
 
            //给动画控件复制一份
            CanvasRenderTarget anTarget = new CanvasRenderTarget(App.Model.AnimatedControl, App.Model.Width, App.Model.Height);
            anTarget.SetPixelBytes(App.Model.CurrentRenderTarget.GetPixelBytes());

            //得到魔棒选区
            int x = (int)v.X;
            int y = (int)v.Y;

            if (x >= 0 && x < App.Model.Width && y >= 0 && y < App.Model.Height)
            {
                 ICanvasImage ci = Adjust.GetMagicWandr(App.Model.CurrentRenderTarget, x, y, App.Setting.MagicTolerance / 100f);
                ICanvasImage aci = Adjust.GetMagicWandr(anTarget, x,y, App.Setting.MagicTolerance / 100f);

                App.Mask(ci, aci, App.Model.MaskMode);//设置选区

                App.Model.isReStroke = true;//重新设置描边
                App.Judge();//判断选区，改变是否动画与选区矩形
            }
        }


        #endregion


        #region Event封装事件：包含Contain


        float ContainX;
        float ContainY;

        private void Contains_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            ContainX = 0;
            ContainY = 0;

            App.Model.isUpdate = false;
        }
        private void Contains_Delta(Point p)
        {
            ContainX = (float)(p.X - PointStart.X);
            ContainY = (float)(p.Y - PointStart.Y);

            App.Model.isReStroke = true;//重新设置描边 
        }
        private void Contains_Complete(Point p)
        {
            //更改数据
            App.Model.isAnimated = false;
            float X = ContainX * App.Model.SX;
            float Y = ContainY * App.Model.SY;
            ContainX = 0;
            ContainY = 0;


            //绘画
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (var ds = crt.CreateDrawingSession())
            {
                ds.DrawImage(App.Model.MaskRenderTarget, X, Y);
            }
            CanvasRenderTarget art = new CanvasRenderTarget(App.Model.AnimatedControl, App.Model.Width, App.Model.Height);
            using (var ds = art.CreateDrawingSession())
            {
                ds.DrawImage(App.Model.MaskAnimatedTarget, X, Y);
            }
            App.Mask(crt, art);


            //判断
            App.Model.isReStroke = true;//重新设置描边 
            App.Model.isUpdate = true;
            App.Judge(App.Model.MaskRect.X + X, App.Model.MaskRect.Y + Y, App.Model.MaskRect.Width, App.Model.MaskRect.Height);
        }


        #endregion

        #region Event封装事件：矩形Rectangle


        private void Rectangle_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            PointStart.X = p.X;
            PointStart.Y = p.Y;

            RectangleWhite.RadiusX = RectangleWhite.RadiusY = RectangleBlack.RadiusX = RectangleBlack.RadiusY = App.Setting.Radius;
            RectangleWhite.Width = RectangleWhite.Height = RectangleBlack.Width = RectangleBlack.Height = 0;

            RectangleWhite.Margin = RectangleBlack.Margin = new Thickness(PointStart.X, PointStart.Y, 0, 0);
        }
        private void Rectangle_Delta(Point p)
        {
            double x, y, w, h;

            double xd = Math.Abs(PointStart.X - p.X);//PointsXDistance：两点的横距离
            double yd = Math.Abs(PointStart.Y - p.Y);//PointsYDistance：两点的纵距离

            double half = (xd + yd) / 2;//横距离纵距离平均数

            //中心
            if (App.Setting.isCenter == true)
            {
                //正方形
                if (App.Setting.isSquare == true )
                {
                    w = 2 * half;
                    h = 2 * half;
                    x = PointStart.X - half;
                    y = PointStart.Y - half;
                }
                else
                {
                    w = 2 * xd;
                    h = 2 * yd;
                    x = PointStart.X - xd;
                    y = PointStart.Y - yd;
                }
            }
            else
            {
                //正方形
                if (App.Setting.isSquare == true )
                {
                    w = half;
                    h = half;

                    if (p.X > PointStart.X) x = PointStart.X;
                    else x = PointStart.X - half;
                    if (p.Y > PointStart.Y) y = PointStart.Y;
                    else y = PointStart.Y - half;
                }
                else
                {
                    w = xd;
                    h = yd;
                    x = Math.Min(PointStart.X, p.X);
                    y = Math.Min(PointStart.Y, p.Y);
                }
            }

            RectangleWhite.Width = RectangleBlack.Width = w;
            RectangleWhite.Height = RectangleBlack.Height = h;
            RectangleWhite.Margin = RectangleBlack.Margin = new Thickness(x, y, 0, 0);    
        }
        private void Rectangle_Complete(Point p)
        {
            float x, y, w, h;

            Vector2 psv = App.Model.ScreenToCanvas(PointStart).ToVector2();//PointStartVector：开始点的向量
            Vector2 pv = App.Model.ScreenToCanvas(p).ToVector2();//PointVector：点的向量

            Vector2 dv = Vector2.Abs(psv - pv); ;//DistanceVector：两点的横距离的向量
            float half = (dv.X + dv.Y) / 2; //横向量纵向量平均数

            if (Vector2.Distance(PointStart.ToVector2(), p.ToVector2()) > 10)
            {
                //中心
                if (App.Setting.isCenter == true)
                {
                    //正方形
                    if (App.Setting.isSquare == true )
                    {
                        x = psv.X - half;
                        y = psv.Y - half;
                        w = 2 * half;
                        h = 2 * half;
                    }
                    else
                    {
                        x = psv.X - dv.X;
                        y = psv.Y - dv.Y;
                        w = 2 * dv.X;
                        h = 2 * dv.Y;
                    }
                }
                else
                { 
                        //正方形
                        if (App.Setting.isSquare == true )
                        {
                        w = half;
                        h = half;

                        if (p.X > PointStart.X) x = psv.X;
                        else x = psv.X - half;
                        if (p.Y > PointStart.Y) y = psv.Y;
                        else y = psv.Y - half;
                    }
                    else
                    {
                        x = Math.Min(psv.X, pv.X);
                        y = Math.Min(psv.Y, pv.Y);
                        w = dv.X;
                        h = dv.Y;
                    }
                }

                //几何图形
                float Radius = App.Setting.Radius / (float)App.Model.XS;//矩形圆角半径
                CanvasGeometry CanvasGeometry = CanvasGeometry.CreateRoundedRectangle(App.Model.VirtualControl, x, y, w, h, Radius, Radius);
                CanvasGeometry AnimatedGeometry = CanvasGeometry.CreateRoundedRectangle(App.Model.AnimatedControl, x, y, w, h, Radius, Radius);

                //几何
                App.Mask(CanvasGeometry, AnimatedGeometry, App.Model.MaskMode); //改变选区与套索
            }
            else App.MaskClear();//清空


            App.Model.isReStroke = true;//重新设置描边
            App.Judge();//判断选区，改变是否动画与选区矩形
            RectangleWhite.Width = RectangleWhite.Height = RectangleBlack.Width = RectangleBlack.Height = 0;
            RectangleWhite.Margin = RectangleBlack.Margin = new Thickness(0);
        }

        #endregion

        #region Event封装事件：圆形Ellipse


        private void Ellipse_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            EllipseWhite.Width = 0; EllipseBlack.Width = 0;
            EllipseWhite.Height = 0; EllipseBlack.Height = 0;

            Thickness margin = new Thickness(PointStart.X, PointStart.Y, 0, 0);
            EllipseWhite.Margin = margin; EllipseBlack.Margin = margin;
        }
        private void Ellipse_Delta(Point p)
        {
            double x, y, w, h;

            double xd = Math.Abs(PointStart.X - p.X);//PointsXDistance：两点的横距离
            double yd = Math.Abs(PointStart.Y - p.Y);//PointsYDistance：两点的纵距离

            double half = (xd + yd) / 2;//横距离纵距离平均数

            //中心
            if (App.Setting.isCenter == true)
            {
                //正方形
                if (App.Setting.isSquare == true )
                {
                    w = 2 * half;
                    h = 2 * half;
                    x = PointStart.X - half;
                    y = PointStart.Y - half;
                }
                else
                {
                    w = 2 * xd;
                    h = 2 * yd;
                    x = PointStart.X - xd;
                    y = PointStart.Y - yd;
                }
            }
            else
            { 
                    //正方形
                    if (App.Setting.isSquare == true )
                    {
                    w = half;
                    h = half;

                    if (p.X > PointStart.X) x = PointStart.X;
                    else x = PointStart.X - half;
                    if (p.Y > PointStart.Y) y = PointStart.Y;
                    else y = PointStart.Y - half;
                }
                else
                {
                    w = xd;
                    h = yd;
                    x = Math.Min(PointStart.X, p.X);
                    y = Math.Min(PointStart.Y, p.Y);
                }
            }

            EllipseWhite.Width = EllipseBlack.Width = w;
            EllipseWhite.Height = EllipseBlack.Height = h;
            EllipseWhite.Margin = EllipseBlack.Margin = new Thickness(x, y, 0, 0);
        }
        private void Ellipse_Complete(Point p)
        {
            float x, y, w, h;

            Vector2 psv = App.Model.ScreenToCanvas(PointStart).ToVector2();//PointStartVector：开始点的向量
            Vector2 pv = App.Model.ScreenToCanvas(p).ToVector2();//PointVector：点的向量

            Vector2 dv = Vector2.Abs(psv - pv); ;//DistanceVector：两点的横距离的向量
            float half = (dv.X + dv.Y) / 2; //横向量纵向量平均数

            if (Vector2.Distance(PointStart.ToVector2(), p.ToVector2()) > 10)
            {
                //中心
                if (App.Setting.isCenter == true)
                {
                    //正方形
                    if (App.Setting.isSquare == true )
                    {
                        x = psv.X;
                        y = psv.Y;
                        w = half;
                        h = half;
                    }
                    else
                    {
                        x = psv.X;
                        y = psv.Y;
                        w = dv.X;
                        h = dv.Y;
                    }
                }
                else
                {
                    //正方形
                    if (App.Setting.isSquare == true)
                    {
                        w = half / 2;
                        h = half / 2;

                        if (p.X > PointStart.X) x = psv.X + w;
                        else x = psv.X - w;
                        if (p.Y > PointStart.Y) y = psv.Y + h;
                        else y = psv.Y - h;
                    }
                    else
                    {
                        x = (psv.X + pv.X) / 2;
                        y = (psv.Y + pv.Y) / 2;
                        w = dv.X / 2;
                        h = dv.Y / 2;
                    }
                }


                //几何图形
                CanvasGeometry CanvasGeometry = CanvasGeometry.CreateEllipse(App.Model.VirtualControl, x, y, w, h);
                CanvasGeometry AnimatedGeometry = CanvasGeometry.CreateEllipse(App.Model.AnimatedControl, x, y, w, h);

                //几何
                App.Mask(CanvasGeometry, AnimatedGeometry, App.Model.MaskMode); //改变选区与套索
            }
            else App.MaskClear();//清空

            App.Model.isReStroke = true;//重新设置描边
            App.Judge();//判断选区，改变是否动画与选区矩形
            EllipseWhite.Margin = new Thickness(0); EllipseBlack.Margin = new Thickness(0);
            EllipseWhite.Width = 0; EllipseBlack.Width = 0;
            EllipseWhite.Height = 0; EllipseBlack.Height = 0;
        }

        #endregion

        #region Event封装事件：多边形Polygon


        bool inPolygon = false;//判断是否处于多边形状态

        private void Polygon_Start(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            if (inPolygon == true)
            {
                //末点跟随
                PolylineWhite.Points[PolylineWhite.Points.Count - 1] = p;
                PolylineBlack.Points[PolylineBlack.Points.Count - 1] = p;
                 VectorList[VectorList.Count - 1] = v;

                // 动画：折线末跟随消失又出现
                PolylineFoot.Margin = new Thickness(p.X - 8, p.Y - 8, 0, 0);
                PolylineFootReleased.Begin();
            }
            else if (inPolygon == false)
            {
                //新建
                Lasso_Start(p);

                //动画：折线首跟随显现
                PolylineHead.Margin = new Thickness(p.X - 8, p.Y - 8, 0, 0);
                PolylineHeadShow.Begin();
                Lasso_Delta(p);
            }
        }
        private void Polygon_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            if (VectorList.Count >= 2)//如果点集合有两个点以上
            {
                //最后一个点跟随鼠标
                PolylineWhite.Points[PolylineWhite.Points.Count - 1] = p;
                PolylineBlack.Points[PolylineBlack.Points.Count - 1] = p;
                VectorList[VectorList.Count - 1] = v;

                //动画：折线末跟随
                PolylineFoot.Margin = new Thickness(p.X - 8, p.Y - 8, 0, 0);
            }
        }
        private void Polygon_Complete(Point p)
        {
            if (inPolygon == true)//如果进行过一轮（多边_按下），现在是第二次（多边_松开）
            {
                //动画：折线末跟随消失又出现
                PolylineFoot.Margin = new Thickness(p.X - 8, p.Y - 8, 0, 0);
                PolylineFootPressed.Begin();
            }

            //如果点与首点的距离小于等于10\点集合数目大于等于3\如果进行过一轮（多边_按下），现在是第二次（多边_松开）
            if (Math.Abs(PolylineWhite.Points.First().X - PolylineWhite.Points.Last().X) <= 10
                && Math.Abs(PolylineWhite.Points.First().Y - PolylineWhite.Points.Last().Y) <= 10
               && VectorList.Count >= 3 && inPolygon == true)//多边形状态结束
            {
                inPolygon = false;
                Lasso_Complete(p);

                //动画：消失
                PolylineHeadFade.Begin();
                PolylineFootFade.Begin();
            }
            else//所有的、杂项的情况下：处于多边形状态
            {
                inPolygon = true;
                Lasso_Delta(p); //将点打入集合
            }
        }


        #endregion

        #region Move.《移动事件》Polygon：多边形


        private void Polygon_Move_Start()
        {
             PolylineWhite.Visibility = Visibility.Collapsed;
            PolylineBlack.Visibility = Visibility.Collapsed;
        }
        private void Polygon_Move_Delta()
        {
            VectorToPoint();//Polygon：画布向量转换到点集合
        }
        private void Polygon_Move_Complete()
        {
             PolylineWhite.Visibility = Visibility.Visible;
            PolylineBlack.Visibility = Visibility.Visible;
        }


        #endregion

        #region Polygon：多边形


        private void VectorToPoint()//画布向量转换到点集合
        {
            if (VectorList.Count > 0)
            {
                for (int i = 0; i < VectorList.Count; i++)
                {
                    PolylineBlack.Points[i] = PolylineWhite.Points[i] = App.Model.CanvasToScreen(VectorList[i].ToPoint());
                }
                PolylineHead.Margin = new Thickness(PolylineWhite.Points.First().X - 8, PolylineWhite.Points.First().Y - 8, 0, 0);
                PolylineFoot.Margin = new Thickness(PolylineWhite.Points.Last().X - 8, PolylineWhite.Points.Last().Y - 8, 0, 0);
            }
        }
        private void PointToVector() //点集合转换到画布向量
        {
            if (PolylineWhite.Points.Count > 0)
            {
                for (int i = 0; i < PolylineWhite.Points.Count; i++)
                {
                    VectorList[i] = App.Model.ScreenToCanvas(PolylineWhite.Points[i]).ToVector2();
                }
            }
        }

        #endregion

        #region Event封装事件：套索Lasso


         List<Vector2> VectorList = new List<Vector2> { };

        private void Lasso_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            PolylineWhite.Visibility = Visibility.Visible;
            PolylineBlack.Visibility = Visibility.Visible;
            VectorList.Clear();
            PolylineWhite.Points.Clear();
            PolylineBlack.Points.Clear();

            PolylineWhite.Points.Add(p);
            PolylineBlack.Points.Add(p);

            VectorList.Add(App.Model.ScreenToCanvas(p).ToVector2());//点的向量
        }

        private void Lasso_Delta(Point p)
        {
            PolylineWhite.Points.Add(p);
            PolylineBlack.Points.Add(p);

            VectorList.Add(App.Model.ScreenToCanvas(p).ToVector2());//点的向量
        }
        private void Lasso_Complete(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            //几何图形
            CanvasGeometry CanvasGeometry = CanvasGeometry.CreatePolygon(App.Model.VirtualControl, VectorList.ToArray());
            CanvasGeometry AnimatedGeometry = CanvasGeometry.CreatePolygon(App.Model.AnimatedControl, VectorList.ToArray());

            //几何
            App.Mask(CanvasGeometry, AnimatedGeometry, App.Model.MaskMode); //改变选区与套索
 
            App.Model.isReStroke = true;//重新设置描边
            App.Judge();//判断选区，改变是否动画与选区矩形

             PolylineWhite.Visibility = Visibility.Collapsed;
            PolylineBlack.Visibility = Visibility.Collapsed;
            VectorList.Clear();
            PolylineWhite.Points.Clear();
            PolylineBlack.Points.Clear();
        }

        #endregion


        #region MaskPaintDraw：选区绘画


        // 绘画画笔
        private void MaskPaintDraw(Vector2 v, float R)
        {
            using (CanvasDrawingSession ds = App.Model.MaskRenderTarget.CreateDrawingSession())
            {
                using (CanvasDrawingSession ads = App.Model.MaskAnimatedTarget.CreateDrawingSession())
                { 
                    //绘画
                    ds.FillCircle(v, R, App.Setting.MaskColor);
                    ads.FillCircle(v, R, App.Setting.MaskColor);
                }
            }

             App.Model.isReStroke = true;//重新设置描边
        }

        private void MaskPaintDraw(Vector2 start, Vector2 end, float R)
        {
             using (CanvasDrawingSession ds = App.Model.MaskRenderTarget.CreateDrawingSession())
            {
                using (CanvasDrawingSession ads = App.Model.MaskAnimatedTarget.CreateDrawingSession())
                { 
                    //两向量的间距，获取插值点数量
                    int count = (int)(Math.Abs((start - end).Length()) / (R / 5));
                    //首末差距的向量
                    Vector2 spaceveector = new Vector2((end.X - start.X) / count, (end.Y - start.Y) / count);
                    
                    //插值点，画直线
                    for (int i = 0; i < count; i++)
                    {
                        start += spaceveector;

                        //绘画
                        ds.FillCircle(start, R, App.Setting.MaskColor);
                        ads.FillCircle(start, R, App.Setting.MaskColor);
                    }
                }
            }

            App.Model.isReStroke = true;//重新设置描边
        }

        #endregion

        #region Event封装事件：选区绘画MaskPaint


        Vector2 MaskPaintVectorOld;

        private void MaskPaint_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            MaskPaintVectorOld = App.Model.ScreenToCanvas(p).ToVector2();
            float R = 修图.BarPage.ToolPage.MaskPaint.Amount;

            //选区绘画
            MaskPaintDraw(MaskPaintVectorOld, R);

            App.Model.isAnimated = true;
 
        }

        private void MaskPaint_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            float R = 修图.BarPage.ToolPage.MaskPaint.Amount;

            //选区绘画
            MaskPaintDraw(MaskPaintVectorOld, v, R);
            MaskPaintVectorOld = v;
        }
        private void MaskPaint_Complete(Point p)
        {
            App.Model.isReStroke = true;//重新设置描边
            App.Model.isAnimated = true;
         }

        #endregion

        #region MaskEraserDraw：选区绘画


         CanvasCommandList MaskEraserVirtualCommandList;
        CanvasCommandList MaskEraserAnimatedCommandList;

        //设置画笔
         private void MaskEraserSet(float R)
        {
            MaskEraserVirtualCommandList = new CanvasCommandList(App.Model.VirtualControl);
            using (CanvasDrawingSession ds = MaskEraserVirtualCommandList.CreateDrawingSession())
            {
                ds.FillCircle(R, R, R, App.Setting.MaskColor);
            }
            MaskEraserAnimatedCommandList = new CanvasCommandList(App.Model.AnimatedControl);
            using (CanvasDrawingSession ds = MaskEraserAnimatedCommandList.CreateDrawingSession())
            {
                ds.FillCircle(R, R, R, App.Setting.MaskColor);
            }
        }



        // 绘画画笔
        private void MaskEraserDraw(Vector2 v, float R)
        {
             using (CanvasDrawingSession ds = App.Model.MaskRenderTarget.CreateDrawingSession())
            {
                using (CanvasDrawingSession ads = App.Model.MaskAnimatedTarget.CreateDrawingSession())
                {
                    //绘画
                    ds.DrawImage(MaskEraserVirtualCommandList, v.X - R, v.Y - R, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);
                    ads.DrawImage(MaskEraserAnimatedCommandList, v.X - R, v.Y - R, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);
                }
            }

            App.Model.isReStroke = true;//重新设置描边
        }

        private void MaskEraserDraw(Vector2 start, Vector2 end, float R)
        {
            using (CanvasDrawingSession ds = App.Model.MaskRenderTarget.CreateDrawingSession())
            {
                using (CanvasDrawingSession ads = App.Model.MaskAnimatedTarget.CreateDrawingSession())
                {
                    //两向量的间距，获取插值点数量
                    int count = (int)(Math.Abs((start - end).Length()) / (R / 5));
                    //首末差距的向量
                    Vector2 spaceveector = new Vector2((end.X - start.X) / count, (end.Y - start.Y) / count);
                    
                    //插值点，画直线
                    for (int i = 0; i < count; i++)
                    {
                        start += spaceveector;

                        //绘画
                         ds.DrawImage(MaskEraserVirtualCommandList, start.X - R, start.Y - R, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);
                        ads.DrawImage(MaskEraserAnimatedCommandList, start.X - R, start.Y - R, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);
                    }
                }
            }

            App.Model.isReStroke = true;//重新设置描边
        }

        #endregion

        #region Event封装事件：选区绘画MaskEraser


        Vector2 MaskEraserVectorOld;

        private void MaskEraser_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            MaskEraserVectorOld = App.Model.ScreenToCanvas(p).ToVector2();
            float R = 修图.BarPage.ToolPage.MaskEraser.Amount;

            //选区绘画
            MaskEraserSet(R);
            MaskEraserDraw(MaskEraserVectorOld, R);
        }

        private void MaskEraser_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            float R = 修图.BarPage.ToolPage.MaskEraser.Amount;

            //选区绘画
            MaskEraserDraw(MaskEraserVectorOld, v, R);
            MaskEraserVectorOld = v;
        }
        private void MaskEraser_Complete(Point p)
        {
            App.Model.isReStroke = true;//重新设置描边
            App.Model.isAnimated = true;
        }

        #endregion

        #region Event封装事件：涂抹Smudge


        Undo SmudgeUndo;

        private void Smudge_Start(Point p)
        {
            //Undo：撤销
            SmudgeUndo = new Undo();
            SmudgeUndo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(SmudgeUndo);
             
            //Paint：绘画
            PaintOldVector = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.PaintWidth;

            //圆环&矩形
            DrawRectSet(p, R);  // 设置屏幕绘画矩形区域
            DrawEllipseSet(); // 设置绘画圆环 
        }
         private void Smudge_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.PaintWidth;

            SmudgeDraw(PaintOldVector, v, R); // 绘画

            PaintOldVector = v;
        }
        private void Smudge_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
         //   PaintDraw(PaintOldVect); // 绘画

            DrawEllipseCollapsed(); //圆环矩形 
            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }




        private void SmudgeDraw(Vector2 start, Vector2 end, float R)
        {
            //虚拟画布渲染矩形
            Rect CanvasRect = new Rect
                   (
                      Math.Min(start.X, end.X) - R,
                      Math.Min(start.Y, end.Y) - R,
                      Math.Abs(start.X - end.X) + R + R,
                      Math.Abs(start.Y - end.Y) + R + R
                   );
            Rect VirtualRect = DrawRectVirtual(App.Model.CanvasToScreen(CanvasRect));




            CropEffect ce = new CropEffect
            {
                Source = SmudgeUndo.Targe,
                SourceRectangle = CanvasRect
            };
            CanvasCommandList ccl = new CanvasCommandList(App.Model.VirtualControl);
            using (CanvasDrawingSession ds = ccl.CreateDrawingSession())
            {
                Vector2 vect = (end + start) / 2;
                CanvasRadialGradientBrush SmudgeBrush = new CanvasRadialGradientBrush(App.Model.VirtualControl, Colors.Black, Colors.Transparent);

                SmudgeBrush.Center = vect;
                SmudgeBrush.RadiusX = SmudgeBrush.RadiusY = R;
                //绘画
                ds.FillCircle(vect, R, Colors.Black);
                ds.DrawImage(ce, 0, 0, SmudgeUndo.Targe.GetBounds(App.Model.VirtualControl), 0.5f, CanvasImageInterpolation.HighQualityCubic, CanvasComposite.SourceIn);
            }
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                //绘画
                ds.DrawImage(ccl, end - start);
            }


            //圆环矩形
            DrawRectFollow(App.Model.CanvasToScreen(end.ToPoint()));// 跟随屏幕绘画矩形区域
            DrawEllipseFollow();// 跟随绘画圆环
            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate(VirtualRect);//画布局部刷新
        }


        #endregion

        #region Event封装事件：混色Mixer


        Undo MixerUndo;
        private void Mixer_Start(Point p)
        {
            //Undo：撤销
            MixerUndo = new Undo();
            MixerUndo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(MixerUndo);

           //Paint：绘画
            PaintOldVector = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.PaintWidth;

            //Mixer：涂抹
            App.Setting.MixerColors.Clear(); //颜色清空
            App.Setting.MixerColors.Add(StrawGetColor(PaintOldVector, MixerUndo.Targe));// Straw：吸管      
            App.Setting.MixerBlend(); //颜色混合

             if (App.Setting.isPaintBitmap == false)
            {
                App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintWidth, App.Setting.MixerColor);//Paint：绘画
            }
            else
            {
                App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintBitmap, App.Setting.PaintWidth, App.Setting.MixerColor);//Paint：绘画
            }

            //圆环&矩形
            DrawRectSet(p, R);  // 设置屏幕绘画矩形区域
            DrawEllipseSet(); // 设置绘画圆环 
        }
        private void Mixer_Delta(Point p)
        {          
            //Mixer：涂抹
            App.Setting.MixerColors.Add(StrawGetColor(p,MixerUndo.Targe));// Straw：吸管      
            App.Setting.MixerBlend(); //颜色混合
            App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintWidth, App.Setting.MixerColor);//Paint：绘画
  
            Paint_Delta(p);//Paint：绘画
        }
        private void Mixer_Complete(Point p)
        {
            App.Setting.MixerColors.Clear(); //颜色清空

            Paint_Complete(p);//Paint：绘画
        }





        //Mixer：涂抹
        private Color StrawGetColor(Point p, CanvasRenderTarget Target)//得到底图颜色
        {
            Point CP = App.Model.ScreenToCanvas(p);
            Color color;

            int X = (int)CP.X;
            int Y = (int)CP.Y;
            if (X > 0 && X < App.Model.Width && Y > 0 && Y < App.Model.Height)
                color = Target.GetPixelColors(X, Y, 1, 1).Single();
            else
                color = Colors.Transparent;
            return color;
        }
        private Color StrawGetColor(Vector2 v, CanvasRenderTarget Target)//得到底图颜色
        {
            Color color;

            int X = (int)v.X;
            int Y = (int)v.Y;
            if (X > 0 && X < App.Model.Width && Y > 0 && Y < App.Model.Height)
                color = Target.GetPixelColors(X, Y, 1, 1).Single();
            else
                color = Colors.Transparent;
            return color;
        }




        #endregion

        #region Paint：绘画

  
        // 绘画画笔
        private void PaintDraw(Vector2 v)
        {
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                //绘画
                 ds.DrawImage(new Transform2DEffect
                {
                     Source = App.Setting.Paint,
                     TransformMatrix =Matrix3x2.CreateScale(App.Model.PressureVector2) *Matrix3x2.CreateRotation(App.Model.Radius)
                 }, v);

                //圆环矩形
                DrawRectFollow(App.Model.CanvasToScreen(v.ToPoint()));// 跟随屏幕绘画矩形区域
                DrawEllipseFollow();// 跟随绘画圆环
                App.Model.isReRender = true;//重新渲染
                CanvasVirtualControl222.Invalidate(DrawRectVirtual(DrawRect));//画布局部刷新
             }
        }

        private void PaintDraw(Vector2 start, Vector2 end,float R)
        {
            //虚拟画布渲染矩形
            Rect VirtualRect = DrawRectVirtual
            (
                App.Model.CanvasToScreen
                (
                    new Rect
                    (
                       Math.Min(start.X, end.X) - R,
                       Math.Min(start.Y, end.Y) - R,
                       Math.Abs(start.X - end.X) + R + R,
                       Math.Abs(start.Y - end.Y) + R + R
                    )
                )
            );

            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                Vector2 vector = start - end;//向量变换，用来计算旋转角度
                App.Model.Radius = 修图.Library.Method.Tanh(vector);//旋转角度
 
                //两向量的间距，获取插值点数量
                int count = (int)(Math.Abs(vector.Length()) / (App.Setting.PaintSpace * R));
                //首末差距的向量
                Vector2 spaceveector = new Vector2((end.X - start.X) / count, (end.Y - start.Y) / count);

                //插值点，画直线
                for (int i = 0; i < count; i++)
                {
                    start += spaceveector;
                    ds.DrawImage(new Transform2DEffect
                    {
                        Source = App.Setting.Paint,
                        TransformMatrix =Matrix3x2.CreateScale(App.Model.PressureVector2) *Matrix3x2.CreateRotation(App.Model.Radius)
                    }, start);
                }
            }

            //圆环矩形
            DrawRectFollow(App.Model.CanvasToScreen(end.ToPoint()));// 跟随屏幕绘画矩形区域
            DrawEllipseFollow();// 跟随绘画圆环
            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate(VirtualRect);//画布局部刷新
        }


        #endregion

        #region Event封装事件：绘画Paint


        Vector2 PaintOldVector;//旧向    

        private void Paint_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);

            PaintOldVector = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.PaintWidth;

            PaintDraw(PaintOldVector); // 绘画

            //圆环&矩形
            DrawRectSet(p, R);  // 设置屏幕绘画矩形区域
            DrawEllipseSet(); // 设置绘画圆环 
        }
        private void Paint_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.PaintWidth;

            if ((Math.Abs((v - PaintOldVector).Length())>(App.Setting.PaintSpace *R)))
            {
             PaintDraw(PaintOldVector, v, R); // 绘画
             PaintOldVector = v;
            }
            else
            {
                DrawEllipseFollow(p);//跟随圆环
            }
        }
        private void Paint_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

             DrawEllipseCollapsed(); //圆环矩形 
            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }


        #endregion


        #region InkCanvas：墨水画布


        private void InkCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            App.Model.墨水同步器 = InkCanvas.InkPresenter.ActivateCustomDrying();
            App.Model.尺子 = new InkPresenterRuler(InkCanvas.InkPresenter);
            App.Model.量角器 = new InkPresenterProtractor(InkCanvas.InkPresenter);

            InkCanvas.InkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;  //笔画输入、笔画开始
            InkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected; //笔画收集

            InkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
        }


        //墨水主持人——笔画收集
        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);
 
            CanvasGeometry InkStrokeGeometry = CanvasGeometry.CreateInk(App.Model.VirtualControl, args.Strokes, Matrix3x2.CreateScale(new Vector2(App.Model.SX, App.Model.SY)), 1);

            using (CanvasDrawingSession ds =App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                ds.FillGeometry(InkStrokeGeometry, -(float)App.Model.X * App.Model.SX, -(float)App.Model.Y * App.Model.SY, App.Model.InkColor);
            }

            //Clear：清空画布
            var 等待干 = App.Model.墨水同步器.BeginDry();
            App.Model.墨水同步器.EndDry();

            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate();//画布刷新     

            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }
        //笔画输入——笔画开始
        private void StrokeInput_StrokeStarted(InkStrokeInput sender, PointerEventArgs args)
        {
        }

         
        #endregion

        #region Move.《移动事件》：铅笔Pencil


        private void Pencil_Move_Start(bool isSelf = true)//是否是自己的指针事件
        {
            if (isSelf == false)
            {
                InkCanvas.Visibility = Visibility.Collapsed;
            }
        }
        private void Pencil_Move_Delta(bool isSelf = true)//是否是自己的指针事件
        {
            App.Model.InkSet();//设置墨水
        }
        private void Pencil_Move_Complete(bool isSelf = true)//是否是自己的指针事件
        {
            if (isSelf == false)
            {
                InkCanvas.Visibility = Visibility.Visible;
            }
            App.Model.InkSet();//设置墨水
        }


        #endregion

        #region Pen：添加
 
        //Add：添加
        private void PenAdd_Start (Vector2 v)
        {
            if (PenIndex >= 0) v = App.Setting.PenVectorList[PenIndex].Vect;//磁吸点

            App.Setting.PenVectorList.Add(new Pen(v));//添加点
        }
        private void PenAdd_Delta(Vector2 v)
        {
            foreach (var Pen in App.Setting.PenVectorList)//磁吸所有点
            {
                if (Vector2.DistanceSquared(v, Pen.Vect )< 100 * App.Model.SX)
                {
                    v = Pen.Vect;
                }
            }
            App.Setting.PenVectorList[App.Setting.PenVectorList.Count - 1].Set(v); //移动最后一个点
        }
        private void PenAdd_Complete(Vector2 v)
        {
        }



        #endregion

        #region Pen：编辑


        //Edit：编辑
        Vector2 PenOld;
        private void PenEdit_Start(Vector2 v)
        {
            if (PenIndex >= 0)
            {
                if (pha == HorizontalAlignment.Center)
                {
                    PenOld = v;//旧

                    if (App.Setting.PenVectorList[PenIndex].isChecked == false)//点击时如果点的点不是选中点，就清空
                    {
                        foreach (var item in App.Setting.PenVectorList)
                        {
                            item.isChecked = false;
                        }
                    }
                    App.Setting.PenVectorList[PenIndex].isChecked = true;//选定
                }
               else if (pha == HorizontalAlignment.Left)
                {
                    App.Setting.PenVectorList[PenIndex].SetRightDistance();//辅助
                    App.Setting.PenVectorList[PenIndex].SetLeft(v); //设置
                }
                else if (pha == HorizontalAlignment.Right)
                {
                    App.Setting.PenVectorList[PenIndex].SetLeftDistance();//辅助
                    App.Setting.PenVectorList[PenIndex].SetRight(v);//设置
                }
            }
            else
            {
                PenStart = v;
                PenEnd = v;
            }
        }
        private void PenEdit_Delta(Vector2 v)
        {
            if (PenIndex >= 0)
            {
                if (pha == HorizontalAlignment.Center)
                {
                    Vector2 Changed = v - PenOld;//位置变化

                    foreach (var item in App.Setting.PenVectorList)
                    {
                        if (item.isChecked == true) item.Move(Changed);//移动所有选定点
                    }

                    App.Setting.PenVectorList[PenIndex].Set(v); //移动最后一个点

                    PenOld = v;//旧
                }
                else if (pha == HorizontalAlignment.Left) App.Setting.PenVectorList[PenIndex].SetLeft(v);
                else if (pha == HorizontalAlignment.Right) App.Setting.PenVectorList[PenIndex].SetRight(v);
            }
            else
            {
                PenEnd = v;//移动钢笔矩形
            }
        }
        private void PenEdit_Complete(Vector2 v)
        {
            if (PenIndex >= 0)
            {
                if (pha == HorizontalAlignment.Center)
                {
                }
                else if (pha == HorizontalAlignment.Left) App.Setting.PenVectorList[PenIndex].Left = v;
                else if (pha == HorizontalAlignment.Right) App.Setting.PenVectorList[PenIndex].Right = v;
            }
            else
            {
                foreach (var item in App.Setting.PenVectorList)//判断钢笔矩形内的点
                {
                    if (item.Vect.X > PenStart.X && item.Vect.Y > PenStart.Y && item.Vect.X < PenEnd.X && item.Vect.Y < PenEnd.Y)
                        item.isChecked = true;
                    else
                        item.isChecked = false;
                }
                PenEnd = v;//移动钢笔矩形
                 PenStart.X = PenStart.Y = PenEnd.X = PenEnd.Y = 0;  //关闭钢笔矩形
             }
        }




        #endregion

        #region Pen：矩形


        //Rect：矩形
        Vector2 PenStart = new Vector2();
        Vector2 PenEnd = new Vector2();
        private void PenRect_Start(Vector2 v)
        {
            PenStart = v;
            PenEnd = v;
        }
        private void PenRect_Delta(Vector2 v)
        {
            PenEnd = v;//移动钢笔矩形

            //判断钢笔矩形内的点
            var Left = Math.Min(PenStart.X, PenEnd.X);
            var Top = Math.Min(PenStart.Y, PenEnd.Y);
            var Right = Math.Max(PenStart.X, PenEnd.X);
            var Bottom = Math.Max(PenStart.Y, PenEnd.Y);
            foreach (var item in App.Setting.PenVectorList)
            {
                if (item.Vect.X > Left && item.Vect.Y >Top && item.Vect.X <Right && item.Vect.Y < Bottom)item.isChecked = true;
                else item.isChecked = false;
            }
        }
        private void PenRect_Complete(Vector2 v)
        {
            PenEnd = v;//移动钢笔矩形
                       
            //判断钢笔矩形内的点
            var Left = Math.Min(PenStart.X, PenEnd.X);
            var Top = Math.Min(PenStart.Y, PenEnd.Y);
            var Right = Math.Max(PenStart.X, PenEnd.X);
            var Bottom = Math.Max(PenStart.Y, PenEnd.Y);
            foreach (var item in App.Setting.PenVectorList)
            {
                if (item.Vect.X > Left && item.Vect.Y > Top && item.Vect.X < Right && item.Vect.Y < Bottom) item.isChecked = true;
                else item.isChecked = false;
            }

            //清空数据
            PenStart.X = PenStart.Y = PenEnd.X = PenEnd.Y = 0;
        }


        #endregion

        #region Event封装事件：钢笔Pen

       
        private int PenIndex = -1; //钢笔索引
        private HorizontalAlignment pha = HorizontalAlignment.Center;//钢笔左中右点
        private void Pen_Start(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            PenIndex = -1;
            pha = HorizontalAlignment.Center;
            for (int i = 0; i < App.Setting.PenVectorList.Count; i++)
            {
                if (App.Setting.PenVectorList[i].isChecked == true)
                {
                     if (Vector2.DistanceSquared(v, App.Setting.PenVectorList[i].Vect) < 100 * App.Model.SX)
                    {
                        PenIndex = i;
                        break;
                    }
                  else  if (Vector2.DistanceSquared(v, App.Setting.PenVectorList[i].Left) < 100 * App.Model.SX)
                    {
                        PenIndex = i;
                        pha = HorizontalAlignment.Left;
                        break;
                    }
                    else if (Vector2.DistanceSquared(v, App.Setting.PenVectorList[i].Right) < 100 * App.Model.SX)
                    {
                        PenIndex = i;
                        pha = HorizontalAlignment.Right;
                        break;
                    }
                }
                else if (Vector2.DistanceSquared(v, App.Setting.PenVectorList[i].Vect) < 100 * App.Model.SX)
                {
                    PenIndex = i;
                    break;
                }
            }

            //封装事件
            if (App.Setting.PenMode == 0) PenAdd_Start(v);
            else 
            {
                if (PenIndex >= 0) PenEdit_Start(v);
                else PenRect_Start(v);
            }

            App.Model.Refresh++;//画布刷新
        }
        private void Pen_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            //封装事件
            if (App.Setting.PenMode == 0) PenAdd_Delta(v);
            else
            {
                if (PenIndex >= 0) PenEdit_Delta(v);
                else PenRect_Delta(v);
            }

            App.Model.Refresh++;//画布刷新
        }
        private void Pen_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();

            //封装事件
            if (App.Setting.PenMode == 0) PenAdd_Complete(v);
            else
            {
                if (PenIndex >= 0) PenEdit_Complete(v);
                else PenRect_Complete(v);
            }

            App.Model.Refresh++;//画布刷新
        }

        #endregion

        #region Eraser：橡皮
         

        // 绘画橡皮 
         private void EraserDraw(Vector2 v, float R)
        {
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                 //绘画
                ds.DrawImage(new Transform2DEffect
                {
                    Source = App.Setting.Paint,
                    TransformMatrix = Matrix3x2.CreateScale(App.Model.PressureVector2) * Matrix3x2.CreateRotation(App.Model.Radius)
                }, v.X -R, v.Y - R, App.Setting.Paint.GetBounds(App.Model.VirtualControl), 1, CanvasImageInterpolation.HighQualityCubic, CanvasComposite.DestinationOut);//DestinationOut

                //圆环矩形
                DrawRectFollow(App.Model.CanvasToScreen(v.ToPoint()));// 跟随屏幕绘画矩形区域
                DrawEllipseFollow();// 跟随绘画圆环
                App.Model.isReRender = true;//重新渲染
                CanvasVirtualControl222.Invalidate(DrawRectVirtual(DrawRect));//画布局部刷新
            }
        }
        private void EraserDraw(Vector2 start, Vector2 end, float R)
        {
            //虚拟画布渲染矩形
            Rect VirtualRect = DrawRectVirtual
            (
                App.Model.CanvasToScreen
                (
                    new Rect
                    (
                       Math.Min(start.X, end.X) - R,
                       Math.Min(start.Y, end.Y) - R,
                       Math.Abs(start.X - end.X) + R + R,
                       Math.Abs(start.Y - end.Y) + R + R
                    )
                )
            );
            using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
            {
                Vector2 vector = start - end;//向量变换，用来计算旋转角度
                App.Model.Radius = 修图.Library.Method.Tanh(vector);//旋转角度

                //两向量的间距，获取插值点数量
                int count = (int)(Math.Abs(vector.Length()) / (App.Setting.PaintSpace * R));
                //首末差距的向量
                Vector2 spaceveector = new Vector2((end.X - start.X) / count, (end.Y - start.Y) / count);
                 
                //插值点，画直线
                for (int i = 0; i < count; i++)
                {
                    start += spaceveector;
                    ds.DrawImage(new Transform2DEffect
                    {
                        Source = App.Setting.Paint,
                        TransformMatrix = Matrix3x2.CreateScale(App.Model.PressureVector2) * Matrix3x2.CreateRotation(App.Model.Radius)
                    }, start.X - R, start.Y - R, App.Setting.Paint.GetBounds(App.Model.VirtualControl), 1, CanvasImageInterpolation.HighQualityCubic, CanvasComposite.DestinationOut);
                }
            }
            //圆环矩形
            DrawRectFollow(App.Model.CanvasToScreen(end.ToPoint()));// 跟随屏幕绘画矩形区域
            DrawEllipseFollow();// 跟随绘画圆环
            App.Model.isReRender = true;//重新渲染
            CanvasVirtualControl222.Invalidate(VirtualRect);//画布局部刷新

        }


        #endregion

        #region Event封装事件：橡皮Eraser


        Vector2 EraserOldVector;

        private void Eraser_Start(Point p)
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
            App.UndoAdd(undo);

            EraserOldVector = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.PaintWidth;

            EraserDraw(EraserOldVector, R); // 绘画

            //圆环&矩形
            DrawRectSet(p, R);  // 设置屏幕绘画矩形区域
            DrawEllipseSet(); // 设置绘画圆环 
        }
        private void Eraser_Delta(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
            float R = App.Setting.PaintWidth;

             if ((Math.Abs((v - EraserOldVector).Length()) > (App.Setting.PaintSpace * R)))
            {
                EraserDraw(EraserOldVector, v, R); // 绘画
                EraserOldVector = v;
             }
            else
            {
                DrawEllipseFollow(p);//跟随圆环
            }
        }
        private void Eraser_Complete(Point p)
        {
            Vector2 v = App.Model.ScreenToCanvas(p).ToVector2();
 
            DrawEllipseCollapsed(); //圆环矩形
            App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
        }  


        #endregion





        #region Event封装事件：暗室灯光Lighting


        Point lsps;//LightingStartPointStart：首点初始
        Point leps;//LightingEndPointStart：末点初始

        private void Lighting_Start(Point p)
        {
            lsps.X = Canvas.GetLeft(LightingThumbStart);
            lsps.Y = Canvas.GetTop(LightingThumbStart);

            leps.X = Canvas.GetLeft(LightingThumbEnd);
            leps.Y = Canvas.GetTop(LightingThumbEnd);
        }
        private void Lighting_Delta(Point p)
        {
            Canvas.SetLeft(LightingThumbStart, p.X - PointStart.X + lsps.X);
            Canvas.SetTop(LightingThumbStart, p.Y - PointStart.Y + lsps.Y);
            Canvas.SetLeft(LightingThumbEnd, p.X - PointStart.X + leps.X);
            Canvas.SetTop(LightingThumbEnd, p.Y - PointStart.Y + leps.Y);

            LightingScreenToCanvas();
            修图.BarPage.EffectPage1.Lighting.Render();
        }
        private void Lighting_Complete(Point p)
        {
            LightingScreenToCanvas();
            修图.BarPage.EffectPage1.Lighting.Render();
        }


        #endregion

        #region Move.《移动事件》：暗室灯光Lighting


        private void Lighting_Move_Start()
        {
            LightingLineWhite.Visibility = Visibility.Collapsed;
            LightingLineBlack.Visibility = Visibility.Collapsed;

            LightingThumbStart.Visibility = Visibility.Collapsed;
            LightingThumbEnd.Visibility = Visibility.Collapsed;
            LightingThumbFocus.Visibility = Visibility.Collapsed;
        }
        private void Lighting_Move_Delta()
        {
            LightingCanvasToScreen();
        }
        private void Lighting_Move_Complete()
        {
            LightingLineWhite.Visibility = Visibility.Visible;
            LightingLineBlack.Visibility = Visibility.Visible;

            LightingThumbStart.Visibility = Visibility.Visible;
            LightingThumbEnd.Visibility = Visibility.Visible;
            LightingThumbFocus.Visibility = Visibility.Visible;
        }


        #endregion

        #region Lighting：控件


        //Start：首点
        private void LightingThumbStart_DragStarted(object sender, DragStartedEventArgs e)
        {
            lsps.X = Canvas.GetLeft(LightingThumbStart);
            lsps.Y = Canvas.GetTop(LightingThumbStart);

            if (App.Setting.isGradientRadial == true)
            {
                leps.X = Canvas.GetLeft(LightingThumbEnd);
                leps.Y = Canvas.GetTop(LightingThumbEnd);
            }
        }
        private void LightingThumbStart_DragDelta(object sender, DragDeltaEventArgs e)
        {
            lsps.X += e.HorizontalChange;
            lsps.Y += e.VerticalChange;

            Canvas.SetLeft(LightingThumbStart, lsps.X);
            Canvas.SetTop(LightingThumbStart, lsps.Y);

            if (App.Setting.isGradientRadial == true)
            {
                leps.X += e.HorizontalChange;
                leps.Y += e.VerticalChange;

                Canvas.SetLeft(LightingThumbEnd, leps.X);
                Canvas.SetTop(LightingThumbEnd, leps.Y);
            }

            LightingScreenToCanvas(); 
            修图.BarPage.EffectPage1.Lighting.Render();
        }
        private void LightingThumbStart_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            LightingScreenToCanvas();
            修图.BarPage.EffectPage1.Lighting.Render();
        }



        //End：末点
        private void LightingThumbEnd_DragStarted(object sender, DragStartedEventArgs e)
        {
            leps.X = Canvas.GetLeft(LightingThumbEnd);
            leps.Y = Canvas.GetTop(LightingThumbEnd);
        }
        private void LightingThumbEnd_DragDelta(object sender, DragDeltaEventArgs e)
        {
            leps.X += e.HorizontalChange;
            leps.Y += e.VerticalChange;

            Canvas.SetLeft(LightingThumbEnd, leps.X);
            Canvas.SetTop(LightingThumbEnd, leps.Y);

            LightingScreenToCanvas(); 
            修图.BarPage.EffectPage1.Lighting.Render();
        }
        private void LightingThumbEnd_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            LightingScreenToCanvas();
            修图.BarPage.EffectPage1.Lighting.Render();
        }




        //Focus：焦点

        Point lf;//LightingFocus：焦点

        private void LightingThumbFocus_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            lf.X = Canvas.GetLeft(LightingThumbFocus);
            lf.Y = Canvas.GetTop(LightingThumbFocus);
         }
        private void LightingThumbFocus_DragDelta(object sender, DragDeltaEventArgs e)
        { 
            lf.X += e.HorizontalChange;
            lf.Y += e.VerticalChange;


            var fs = -Method.两点距(lf, lsps);
            var fe = -Method.两点距(lf, leps);

            var f = (float)(fs / (fs+fe));
            if (f > 1) f = 1;
            if (f < 0) f = 0;
            App.Setting.LightFocus = f;

            LightingCanvasToScreen();
            修图.BarPage.EffectPage1.Lighting.Render();
        }
        private void LightingThumbFocus_DragStarted(object sender, DragStartedEventArgs e)
        {
            LightingCanvasToScreen();
            修图.BarPage.EffectPage1.Lighting.Render();
        }


        

        #endregion

        #region Lighting：算法


        private void LightingCanvasToScreen(bool isFocus=false)
        {
            lsps = App.Model.CanvasToScreen(App.Setting.LightingStartPoint);
            leps = App.Model.CanvasToScreen(App.Setting.LightingEndPoint);

            Canvas.SetLeft(LightingThumbStart, lsps.X - 15);
            Canvas.SetTop(LightingThumbStart, lsps.Y - 15);

            Canvas.SetLeft(LightingThumbEnd, leps.X - 15);
            Canvas.SetTop(LightingThumbEnd, leps.Y - 15);
 
            LightingFolow();
        }
        private void LightingScreenToCanvas()
        {
            Point Start = new Point(Canvas.GetLeft(LightingThumbStart) + 15, Canvas.GetTop(LightingThumbStart) + 15);
            Point End = new Point(Canvas.GetLeft(LightingThumbEnd) + 15, Canvas.GetTop(LightingThumbEnd) + 15);

            App.Setting.LightingStartPoint = App.Model.ScreenToCanvas(Start);
            App.Setting.LightingEndPoint = App.Model.ScreenToCanvas(End);

            LightingFolow();
        }

        private void LightingFolow()//控件跟随数据
        {
            Point start = new Point(Canvas.GetLeft(LightingThumbStart) + 15, Canvas.GetTop(LightingThumbStart) + 15);
            Point end = new Point(Canvas.GetLeft(LightingThumbEnd) + 15, Canvas.GetTop(LightingThumbEnd) + 15);

            LightingLineWhite.X1 = LightingLineBlack.X1 = start.X;
            LightingLineWhite.Y1 = LightingLineBlack.Y1 = start.Y;

            LightingLineWhite.X2 = LightingLineBlack.X2 = end.X;
            LightingLineWhite.Y2 = LightingLineBlack.Y2 = end.Y;

            lf = Method.比在线上点(App.Setting.LightFocus, start, end);
            Canvas.SetLeft(LightingThumbFocus, lf.X -15);
            Canvas.SetTop(LightingThumbFocus, lf.Y -15);
        }

        private void LightingChange()//跟随选区变换
        {
            if (App.Model.isAnimated == true)
            {
                App.Judge();//判断选区，改变是否动画与选区矩形
                Rect r = App.Model.MaskRect;

                App.Setting.LightingStartPoint.X = r.X;
                App.Setting.LightingStartPoint.Y = r.Y;
                App.Setting.LightingEndPoint.X = r.Width + r.X;
                App.Setting.LightingEndPoint.Y = r.Height + r.Y;
            }
            else
            {
                App.Setting.LightingStartPoint.X = App.Model.Width / 3;
                App.Setting.LightingStartPoint.Y = App.Model.Height / 3;
                App.Setting.LightingEndPoint.X = App.Model.Width * 2 / 3;
                App.Setting.LightingEndPoint.Y = App.Model.Height * 2 / 3;
            }
        }


        #endregion




        #region Move.《移动事件》：膨胀收缩PinchPunch


        private void PinchPunch_Move_Start()
        {
            PinchPunchEllipse.Visibility = Visibility.Collapsed;
             PinchPunchThumb.Visibility = Visibility.Collapsed;
         }
        private void PinchPunch_Move_Delta()
        {
            PinchPunchCanvasToScreen();
        }
        private void PinchPunch_Move_Complete()
        {
            PinchPunchEllipse.Visibility = Visibility.Visible;
            PinchPunchThumb.Visibility = Visibility.Visible;
        }


        #endregion

        #region PinchPunch：控件

        double ppxs;

        private void PinchPunchThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            ppxs = Canvas.GetLeft(PinchPunchThumb);
        }
        private void PinchPunchThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ppxs += e.HorizontalChange;
            Canvas.SetLeft(PinchPunchThumb, ppxs);

            Tip(Canvas.GetLeft(PinchPunchThumb) + 15, Canvas.GetTop(PinchPunchThumb) + 15, App.Setting.PinchPunchRadius);//Tip：提示

            PinchPunchScreenToCanvas();
            PinchPunchCanvasToScreen(true);
            修图.BarPage.EffectPage3.PinchPunch.Render();
        }
        private void PinchPunchThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            TipGrid.Visibility = Visibility.Collapsed;//Tip：提示
        }


        #endregion

        #region PinchPunch：算法


        private void PinchPunchCanvasToScreen(bool isCon=false)
        {
            Vector2 center = App.Model.CanvasToScreen(new Vector2(App.Model.Width / 2, App.Model.Height / 2));
            double r = App.Setting.PinchPunchRadius * App.Model.XS;
            if (isCon==false)
            {
                Canvas.SetLeft(PinchPunchThumb, center.X - 15 + r);
                Canvas.SetTop(PinchPunchThumb, center.Y - 15);
            }  

            PinchPunchEllipse.Width = PinchPunchEllipse.Height = 2 * r;
            Canvas.SetLeft(PinchPunchEllipse, center.X - r);
            Canvas.SetTop(PinchPunchEllipse, center.Y - r);
        }
        private void PinchPunchScreenToCanvas()
        {
            var x= Canvas.GetLeft(PinchPunchThumb) + 15;
            var y= Canvas.GetTop(PinchPunchThumb) + 15;
            var p = App.Model.ScreenToCanvas(new Point(x, y));

            App.Setting.PinchPunchRadius =Math.Abs((float)p.X - App.Model.Width / 2);
        }

        private void PinchPunchChange()
        {
            App.Setting.PinchPunchRadius = App.Model.Width / 2;
            PinchPunchCanvasToScreen();
        }



        #endregion




        #region Event封装事件：裁切Crop

        //屏幕首点
        Point csps;//CropStartPointStart：首点初始
        Point ceps;//CropEndPointStart：末点初始

        private void Crop_Start(Point p)
        {
            csps = App.Model.CanvasToScreen(App.Setting.CropStartPoint);
            ceps = App.Model.CanvasToScreen(App.Setting.CropEndPoint);
         }
        private void Crop_Delta(Point p)
        {
            double hor = p.X - PointStart.X;
            double ver = p.Y - PointStart.Y;
             
            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(new Point(csps.X + hor, csps.Y + ver));
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(new Point(ceps.X + hor, ceps.Y + ver));
            CropCanvasToScreen();
         }
        private void Crop_Complete(Point p)
        {
            CropCanvasToScreen();

            App.Model.Refresh++;//刷新底栏数字选择器
        }


        #endregion

        #region Move.《移动事件》：裁切Crop


        private void Crop_Move_Start()
        {
            //Polyline：多边形
            PolylineWhite.Visibility = Visibility.Collapsed;
            PolylineBlack.Visibility = Visibility.Collapsed;

            CropRectPath.Visibility = Visibility.Collapsed;
 
            CropThumbLT.Visibility = Visibility.Collapsed;
            CropThumbRB.Visibility = Visibility.Collapsed;
             CropThumbLB.Visibility = Visibility.Collapsed;
            CropThumbRT.Visibility = Visibility.Collapsed;
             CropThumbLC.Visibility = Visibility.Collapsed;
            CropThumbRC.Visibility = Visibility.Collapsed;
             CropThumbTC.Visibility = Visibility.Collapsed;
            CropThumbBC.Visibility = Visibility.Collapsed;
        }
        private void Crop_Move_Delta()
        {
            CropCanvasToScreen();
        }
        private void Crop_Move_Complete()
        {
            //Polyline：多边形
            PolylineWhite.Visibility = Visibility.Visible;
            PolylineBlack.Visibility = Visibility.Visible;

            CropRectPath.Visibility = Visibility.Visible;

            CropThumbLT.Visibility = Visibility.Visible;
            CropThumbRB.Visibility = Visibility.Visible;
             CropThumbLB.Visibility = Visibility.Visible;
            CropThumbRT.Visibility = Visibility.Visible;
             CropThumbLC.Visibility = Visibility.Visible;
            CropThumbRC.Visibility = Visibility.Visible;
             CropThumbTC.Visibility = Visibility.Visible;
            CropThumbBC.Visibility = Visibility.Visible; 
        }


        #endregion

        #region Crop：控件


        private void CropThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            csps = App.Model.CanvasToScreen(App.Setting.CropStartPoint);
            ceps = App.Model.CanvasToScreen(App.Setting.CropEndPoint);

            isThumb = true;//拇指正在移动，移动事件不起作用
         }
        private void CropThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
            App.Model.Refresh++;//刷新底栏数字选择器

            isThumb = false;//拇指正在移动，移动事件不起作用
         }



        //Start：首点
        private void CropThumbLT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            csps.X += e.HorizontalChange;
            csps.Y += e.VerticalChange;


            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
        }
        //End：末点
        private void CropThumbRB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ceps.X += e.HorizontalChange;
            ceps.Y += e.VerticalChange;
 
            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
        } 

        //左下 
        private void CropThumbLB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            csps.X += e.HorizontalChange;
            ceps.Y += e.VerticalChange;

            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
        } 
        //右上 
        private void CropThumbRT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ceps.X += e.HorizontalChange;
            csps.Y += e.VerticalChange;

            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
        }

        //左中
        private void CropThumbLC_DragDelta(object sender, DragDeltaEventArgs e)
        {
             csps.X += e.HorizontalChange;

            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
        }
        //右中
        private void CropThumbRC_DragDelta(object sender, DragDeltaEventArgs e)
        {
             ceps.X += e.HorizontalChange;

            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
        }

        //上中
        private void CropThumbTC_DragDelta(object sender, DragDeltaEventArgs e)
        {
            csps.Y += e.VerticalChange;

            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
        }
        //下中
        private void CropThumbBC_DragDelta(object sender, DragDeltaEventArgs e)
        {
             ceps.Y+= e.VerticalChange;

            App.Setting.CropStartPoint = App.Model.ScreenToCanvas(csps);
            App.Setting.CropEndPoint = App.Model.ScreenToCanvas(ceps);
            CropCanvasToScreen();
        }


        #endregion

        #region Crop：算法
 

        private void CropCanvasToScreen()
        {
            Point Start = App.Model.CanvasToScreen(App.Setting.CropStartPoint);
            Point End = App.Model.CanvasToScreen(App.Setting.CropEndPoint);

            double CenterX = (Start.X + End.X) / 2;
            double CenterY = (Start.Y + End.Y) / 2;

            Canvas.SetLeft(CropThumbLT, Start.X-15); Canvas.SetTop(CropThumbLT, Start.Y - 15);
            Canvas.SetLeft(CropThumbRB, End.X - 15); Canvas.SetTop(CropThumbRB, End.Y - 15);

            Canvas.SetLeft(CropThumbLB, Start.X - 15); Canvas.SetTop(CropThumbLB, End.Y - 15);
            Canvas.SetLeft(CropThumbRT, End.X - 15); Canvas.SetTop(CropThumbRT, Start.Y - 15);

            Canvas.SetLeft(CropThumbLC, Start.X - 15); Canvas.SetTop(CropThumbLC, CenterY - 15);
            Canvas.SetLeft(CropThumbRC, End.X - 15); Canvas.SetTop(CropThumbRC, CenterY - 15);

            Canvas.SetLeft(CropThumbTC, CenterX - 15); Canvas.SetTop(CropThumbTC, Start.Y - 15);
            Canvas.SetLeft(CropThumbBC, CenterX - 15); Canvas.SetTop(CropThumbBC, End.Y - 15);



            double L = Math.Min(Start.X, End.X);
            double R = Math.Max(Start.X, End.X);
            double T= Math.Min(Start.Y, End.Y);
            double B = Math.Max(Start.Y, End.Y);

            Point LT = new Point(L, T);
            Point RT = new Point(R, T);
            Point LB = new Point(L, B);
            Point RB = new Point(R, B);

            //矩形
            CropRectOut.Rect = new Rect(0, 0, App.Model.GridWidth, App.Model.GridHeight);
            CropRectIn.Rect = new Rect(LT, RB);

            //Polyline：多边形
            PolylineWhite.Points = new PointCollection {LT , RT, RB, LB, LT };
            PolylineBlack.Points = new PointCollection { LT, RT, RB, LB, LT };
        }
        private void CropScreenToCanvas()
        { 
        }

         
        private void CropChange()//跟随选区变换
        {
            App.Judge();//判断选区，改变是否动画与选区矩形

            Rect r = App.Model.MaskRect;

            if (App.Model.isAnimated == true)
            {
                App.Setting.CropStartPoint.X = r.X;
                App.Setting.CropStartPoint.Y = r.Y;
                App.Setting.CropEndPoint.X = r.Width + r.X;
                App.Setting.CropEndPoint.Y = r.Height + r.Y;
            }
            else
            {
                App.Setting.CropStartPoint.X = 0;
                App.Setting.CropStartPoint.Y = 0;
                App.Setting.CropEndPoint.X = App.Model.Width;
                App.Setting.CropEndPoint.Y = App.Model.Height;
            }
        }
         


        #endregion

        
        #region Event封装事件：渐变Gradient


        Point gsps;//GradientStartPointStart：首点初始
        Point geps;//GradientEndPointStart：末点初始

        private void Gradient_Start(Point p)
        { 
             gsps = App.Model.CanvasToScreen(App.Setting.GradientStartPoint);
            geps = App.Model.CanvasToScreen(App.Setting.GradientEndPoint);
        }
        private void Gradient_Delta(Point p)
        { 
            double hor = p.X - PointStart.X;
            double ver = p.Y - PointStart.Y;

            App.Setting.GradientStartPoint = App.Model.ScreenToCanvas(new Point(gsps.X + hor, gsps.Y + ver));
            App.Setting.GradientEndPoint = App.Model.ScreenToCanvas(new Point(geps.X + hor, geps.Y + ver));

            GradientCanvasToScreen();
            修图.BarPage.OtherPage.Gradient.Render();
        }
        private void Gradient_Complete(Point p)
        {
            GradientCanvasToScreen();
            修图.BarPage.OtherPage.Gradient.Render();
        }


        #endregion

        #region Move.《移动事件》：渐变Gradient


        private void Gradient_Move_Start()
        {
            GradientLineWhite.Visibility = Visibility.Collapsed;
            GradientLineBlack.Visibility = Visibility.Collapsed;
            GradientThumbStart.Visibility = Visibility.Collapsed;
            GradientThumbEnd.Visibility = Visibility.Collapsed;
        }
        private void Gradient_Move_Delta()
        {
            GradientCanvasToScreen();
        }
        private void Gradient_Move_Complete()
        {
            GradientLineWhite.Visibility = Visibility.Visible;
            GradientLineBlack.Visibility = Visibility.Visible;
            GradientThumbStart.Visibility = Visibility.Visible;
            GradientThumbEnd.Visibility = Visibility.Visible;
        }


        #endregion

        #region Gradient：控件


        //Start：首点
        private void GradientThumbStart_DragStarted(object sender, DragStartedEventArgs e)
        {
            isThumb = true;//拇指正在移动，移动事件不起作用

            App.Model.TipVisibility = Visibility.Visible;//全局提示
        }
        private void GradientThumbStart_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double hor = e.HorizontalChange / App.Model.XS;
            double ver = e.VerticalChange / App.Model.YS;

            App.Setting.GradientStartPoint.X += hor;
            App.Setting.GradientStartPoint.Y += ver;
            if (App.Setting.isGradientRadial == true)
            {
                App.Setting.GradientEndPoint.X += hor;
                App.Setting.GradientEndPoint.Y += ver;
            }

            GradientCanvasToScreen();
            修图.BarPage.OtherPage.Gradient.Render();

            App.Model.Tip = "X:" + ((int)App.Setting.GradientStartPoint.X).ToString() + "  Y:" + ((int)App.Setting.GradientStartPoint.Y).ToString();//全局提示
        }
        private void GradientThumbStart_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false;//拇指正在移动，移动事件不起作用

            GradientScreenToCanvas();
            修图.BarPage.OtherPage.Gradient.Render();

             App.Model.TipVisibility = Visibility.Collapsed;//全局提示
        }



            //End：末点
            private void GradientThumbEnd_DragStarted(object sender, DragStartedEventArgs e)
        {
            isThumb = true;//拇指正在移动，移动事件不起作用 

            App.Model.TipVisibility = Visibility.Visible;//全局提示
        }
        private void GradientThumbEnd_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double hor = e.HorizontalChange / App.Model.XS;
            double ver = e.VerticalChange / App.Model.YS;

            App.Setting.GradientEndPoint.X += hor;
            App.Setting.GradientEndPoint.Y += ver;

            GradientCanvasToScreen();
            修图.BarPage.OtherPage.Gradient.Render();

            App.Model.Tip = "X:" + ((int)App.Setting.GradientEndPoint.X).ToString() + "  Y:" + ((int)App.Setting.GradientEndPoint.Y).ToString();
         }
        private void GradientThumbEnd_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false;//拇指正在移动，移动事件不起作用

            GradientScreenToCanvas();
            修图.BarPage.OtherPage.Gradient.Render();

             App.Model.TipVisibility = Visibility.Collapsed;//全局提示   
        }


            #endregion
          
        #region Gradient：算法


            private void GradientCanvasToScreen()
        {
            Point Start = App.Model.CanvasToScreen(App.Setting.GradientStartPoint);
            Point End = App.Model.CanvasToScreen(App.Setting.GradientEndPoint);

            //控件
            Canvas.SetLeft(GradientThumbStart, Start.X - 15);
            Canvas.SetTop(GradientThumbStart, Start.Y - 15);
            Canvas.SetLeft(GradientThumbEnd, End.X - 15);
            Canvas.SetTop(GradientThumbEnd, End.Y - 15);

            //线
            GradientLineWhite.X1 = GradientLineBlack.X1 = Start.X;
            GradientLineWhite.Y1 = GradientLineBlack.Y1 = Start.Y;
            GradientLineWhite.X2 = GradientLineBlack.X2 = End.X;
            GradientLineWhite.Y2 = GradientLineBlack.Y2 = End.Y;
        }
        private void GradientScreenToCanvas()
        {
        }
        
        private void GradientChange()//跟随选区变换
        {
            App.Judge();//判断选区，改变是否动画与选区矩形

            Rect r = App.Model.MaskRect;

            if (App.Model.isAnimated == true)
            {
                App.Setting.GradientStartPoint.X = r.X;
                App.Setting.GradientStartPoint.Y = r.Y;
                App.Setting.GradientEndPoint.X = r.Width + r.X;
                App.Setting.GradientEndPoint.Y = r.Height + r.Y;
            }
            else
            {
                App.Setting.GradientStartPoint.X = App.Model.Width / 3;
                App.Setting.GradientStartPoint.Y = App.Model.Height / 3;
                App.Setting.GradientEndPoint.X = App.Model.Width * 2 / 3;
                App.Setting.GradientEndPoint.Y = App.Model.Height * 2 / 3;
            }
        }



        #endregion 


        #region Event封装事件：渐隐Fade


        Point fsps;//FadeStartPointStart：首点初始
        Point feps;//FadeEndPointStart：末点初始
 
        private void Fade_Start(Point p)
        {
            fsps = App.Model.CanvasToScreen(App.Setting.FadeStartPoint);
            feps = App.Model.CanvasToScreen(App.Setting.FadeEndPoint); 
        }
        private void Fade_Delta(Point p)
        {
            double hor = p.X - PointStart.X;
            double ver = p.Y - PointStart.Y;

            App.Setting.FadeStartPoint = App.Model.ScreenToCanvas(new Point(fsps.X + hor, fsps.Y + ver));
            App.Setting.FadeEndPoint = App.Model.ScreenToCanvas(new Point(feps.X + hor, feps.Y + ver));

            FadeCanvasToScreen();
            修图.BarPage.OtherPage.Fade.Render();
        }
        private void Fade_Complete(Point p)
        {
            FadeCanvasToScreen();
            修图.BarPage.OtherPage.Fade.Render();
         }


        #endregion

        #region Move.《移动事件》：渐隐Fade


        private void Fade_Move_Start()
        {
            FadeLineWhite.Visibility = Visibility.Collapsed;
            FadeLineBlack.Visibility = Visibility.Collapsed;
            FadeThumbStart.Visibility = Visibility.Collapsed;
            FadeThumbEnd.Visibility = Visibility.Collapsed;
        }
        private void Fade_Move_Delta()
        {
            FadeCanvasToScreen();
        }
        private void Fade_Move_Complete()
        {
            FadeLineWhite.Visibility = Visibility.Visible;
            FadeLineBlack.Visibility = Visibility.Visible;
            FadeThumbStart.Visibility = Visibility.Visible;
            FadeThumbEnd.Visibility = Visibility.Visible;
        }


        #endregion

        #region Fade：控件


        //Start：首点
        private void FadeThumbStart_DragStarted(object sender, DragStartedEventArgs e)
        {
            isThumb = true;//拇指正在移动，移动事件不起作用

            App.Model.TipVisibility = Visibility.Visible;//全局提示
        }
        private void FadeThumbStart_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double hor = e.HorizontalChange / App.Model.XS;
            double ver = e.VerticalChange / App.Model.YS;

            App.Setting.FadeStartPoint.X += hor;
            App.Setting.FadeStartPoint.Y += ver;
            if (App.Setting.isFadeRadial == true)
            {
                App.Setting.FadeEndPoint.X += hor;
                App.Setting.FadeEndPoint.Y += ver;
            }
            FadeCanvasToScreen();
            修图.BarPage.OtherPage.Fade.Render();

            App.Model.Tip = "X:" + ((int)App.Setting.FadeStartPoint.X).ToString() + "  Y:" + ((int)App.Setting.FadeStartPoint.Y).ToString();//全局提示
        }
        private void FadeThumbStart_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false;//拇指正在移动，移动事件不起作用

            FadeScreenToCanvas();
            修图.BarPage.OtherPage.Fade.Render();

            App.Model.TipVisibility = Visibility.Collapsed;//全局提示
        }




        //End：末点
        private void FadeThumbEnd_DragStarted(object sender, DragStartedEventArgs e)
        {
            isThumb = true;//拇指正在移动，移动事件不起作用 
        }
        private void FadeThumbEnd_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double hor = e.HorizontalChange / App.Model.XS;
            double ver = e.VerticalChange / App.Model.YS;

            App.Setting.FadeEndPoint.X += hor;
            App.Setting.FadeEndPoint.Y += ver;

            FadeCanvasToScreen();
            修图.BarPage.OtherPage.Fade.Render();

            var s = "X:" + ((int)App.Setting.FadeEndPoint.X).ToString() + "  Y:" + ((int)App.Setting.FadeEndPoint.Y).ToString();
            App.Tip(s);//全局提示
        }
        private void FadeThumbEnd_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false;//拇指正在移动，移动事件不起作用

            FadeScreenToCanvas();
            修图.BarPage.OtherPage.Fade.Render();

            TipGrid.Visibility = Visibility.Collapsed;//Tip：提示
        }


        #endregion

        #region Fade：算法


        private void FadeCanvasToScreen()
        {
            Point Start = App.Model.CanvasToScreen(App.Setting.FadeStartPoint);
            Point End = App.Model.CanvasToScreen(App.Setting.FadeEndPoint);

            //控件
            Canvas.SetLeft(FadeThumbStart, Start.X - 15);
            Canvas.SetTop(FadeThumbStart, Start.Y - 15);
            Canvas.SetLeft(FadeThumbEnd, End.X - 15);
            Canvas.SetTop(FadeThumbEnd, End.Y - 15);

            //线
            FadeLineWhite.X1 = FadeLineBlack.X1 = Start.X;
            FadeLineWhite.Y1 = FadeLineBlack.Y1 = Start.Y;
            FadeLineWhite.X2 = FadeLineBlack.X2 = End.X;
            FadeLineWhite.Y2 = FadeLineBlack.Y2 = End.Y;
        }
        private void FadeScreenToCanvas()
        {
        }

        private void FadeChange()//跟随选区变换
        {
            App.Judge();//判断选区，改变是否动画与选区矩形

            Rect r = App.Model.MaskRect;

            if (App.Model.isAnimated == true)
            {
                App.Setting.FadeStartPoint.X = r.X;
                App.Setting.FadeStartPoint.Y = r.Y;
                App.Setting.FadeEndPoint.X = r.Width + r.X;
                App.Setting.FadeEndPoint.Y = r.Height + r.Y;
            }
            else
            {
                App.Setting.FadeStartPoint.X = App.Model.Width / 3;
                App.Setting.FadeStartPoint.Y = App.Model.Height / 3;
                App.Setting.FadeEndPoint.X = App.Model.Width * 2 / 3;
                App.Setting.FadeEndPoint.Y = App.Model.Height * 2 / 3;
            }
        }

        
        #endregion
        

        #region Event封装事件：文字Text


        Point tsps;//TextStartPointStart：首点初始
        Point teps;//TextEndPointStart：末点初始

        private void Text_Start(Point p)
        {
            tsps.X = Canvas.GetLeft(TextThumbStart);
            tsps.Y = Canvas.GetTop(TextThumbStart);

            teps.X = Canvas.GetLeft(TextThumbEnd);
            teps.Y = Canvas.GetTop(TextThumbEnd);

            App.Model.TipVisibility = Visibility.Visible;//全局提示
        }
        private void Text_Delta(Point p)
        {
            Canvas.SetLeft(TextThumbStart, p.X - PointStart.X + tsps.X);
            Canvas.SetTop(TextThumbStart, p.Y - PointStart.Y + tsps.Y);
            Canvas.SetLeft(TextThumbEnd, p.X - PointStart.X + teps.X);
            Canvas.SetTop(TextThumbEnd, p.Y - PointStart.Y + teps.Y);

            TextScreenToCanvas();
            修图.BarPage.OtherPage.Text.Render();

            App.Model.Tip = "X:" + ((int)App.Setting.TextStartPoint.X).ToString() + "  Y:" + ((int)App.Setting.TextStartPoint.Y).ToString(); //全局提示
        }
        private void Text_Complete(Point p)
        {
             TextScreenToCanvas();
            修图.BarPage.OtherPage.Text.Render();

            App.Model.TipVisibility = Visibility.Collapsed;//全局提示
         }


        #endregion

        #region Move.《移动事件》：文字Text


        private void Text_Move_Start()
        { 
            TextRectBlack.Visibility = Visibility.Collapsed;
            TextRectWhite.Visibility = Visibility.Collapsed;
            TextThumbStart.Visibility = Visibility.Collapsed;
            TextThumbEnd.Visibility = Visibility.Collapsed;
        }
        private void Text_Move_Delta()
        {
            TextCanvasToScreen();
        }
        private void Text_Move_Complete()
        {
            TextRectBlack.Visibility = Visibility.Visible;
            TextRectWhite.Visibility = Visibility.Visible;
             TextThumbStart.Visibility = Visibility.Visible;
            TextThumbEnd.Visibility = Visibility.Visible;
        }


        #endregion

        #region Text：控件

        

        //Start：首点
        private void TextThumbStart_DragStarted(object sender, DragStartedEventArgs e)
        {
             isThumb = true;//拇指正在移动，移动事件不起作用

            tsps.X = Canvas.GetLeft(TextThumbStart);
            tsps.Y = Canvas.GetTop(TextThumbStart);

            App.Model.TipVisibility = Visibility.Visible;//全局提示
        }
        private void TextThumbStart_DragDelta(object sender, DragDeltaEventArgs e)
        {
            tsps.X += e.HorizontalChange;
            tsps.Y += e.VerticalChange;

            Canvas.SetLeft(TextThumbStart, tsps.X);
            Canvas.SetTop(TextThumbStart, tsps.Y);

            TextScreenToCanvas();
            修图.BarPage.OtherPage.Text.Render();

            App.Model.Tip = "W:" + ((int)App.Setting.TextW).ToString() + " H:" + ((int)App.Setting.TextH).ToString();
         }
        private void TextThumbStart_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false;//拇指正在移动，移动事件不起作用
 
            TextScreenToCanvas();
            修图.BarPage.OtherPage.Text.Render();

            App.Model.TipVisibility = Visibility.Collapsed;
        }



        //End：末点
        private void TextThumbEnd_DragStarted(object sender, DragStartedEventArgs e)
        {
            isThumb = true;//拇指正在移动，移动事件不起作用

            teps.X = Canvas.GetLeft(TextThumbEnd);
            teps.Y = Canvas.GetTop(TextThumbEnd);

             App.Model.TipVisibility = Visibility.Visible;
        }
        private void TextThumbEnd_DragDelta(object sender, DragDeltaEventArgs e)
        {
            teps.X += e.HorizontalChange;
            teps.Y += e.VerticalChange;

            Canvas.SetLeft(TextThumbEnd, teps.X);
            Canvas.SetTop(TextThumbEnd, teps.Y);

            TextScreenToCanvas();
            修图.BarPage.OtherPage.Text.Render();

            App.Model.Tip = "W:" + ((int)App.Setting.TextW).ToString() + " H:" + ((int)App.Setting.TextH).ToString();//全局提示
         }
        private void TextThumbEnd_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false;//拇指正在移动，移动事件不起作用
 
            TextScreenToCanvas();
            修图.BarPage.OtherPage.Text.Render();

            App.Model.TipVisibility = Visibility.Collapsed;//全局提示
        }


        #endregion

        #region Text：算法


        private void TextCanvasToScreen()
        {
            Point Start = App.Model.CanvasToScreen(App.Setting.TextStartPoint);
            Point End = App.Model.CanvasToScreen(App.Setting.TextEndPoint);

            Canvas.SetLeft(TextThumbStart, Start.X - 15);
            Canvas.SetTop(TextThumbStart, Start.Y - 15);

            Canvas.SetLeft(TextThumbEnd, End.X - 15);
            Canvas.SetTop(TextThumbEnd, End.Y - 15);

            TextFolow();
        }
        private void TextScreenToCanvas()
        {
            Point Start = new Point(Canvas.GetLeft(TextThumbStart) + 15, Canvas.GetTop(TextThumbStart) + 15);
            Point End = new Point(Canvas.GetLeft(TextThumbEnd) + 15, Canvas.GetTop(TextThumbEnd) + 15);

            App.Setting.TextStartPoint = App.Model.ScreenToCanvas(Start);
            App.Setting.TextEndPoint = App.Model.ScreenToCanvas(End);

            TextFolow();
        }



        private void TextFolow()//控件跟随数据
        {
            //首点末点
            double x1 = Canvas.GetLeft(TextThumbStart);
            double y1 = Canvas.GetTop(TextThumbStart);
            double x2 = Canvas.GetLeft(TextThumbEnd);
            double y2 = Canvas.GetTop(TextThumbEnd);

            //横纵宽高
            double x = Math.Min(x1, x2);
            double y = Math.Min(y1, y2);
            double w = Math.Abs(x1 - x2);
            double h = Math.Abs(y1 - y2);

            //XY
            Canvas.SetLeft(TextRectBlack, x + 15);
            Canvas.SetTop(TextRectBlack, y + 15);
            Canvas.SetLeft(TextRectWhite, x + 15);
            Canvas.SetTop(TextRectWhite, y + 15);
            //WH
            TextRectBlack.Width = TextRectWhite.Width =  w;
            TextRectBlack.Height =  TextRectWhite.Height = h;
        }

        private void TextChange()//跟随选区变换
        {
            App.Judge();//判断选区，改变是否动画与选区矩形

            Rect r = App.Model.MaskRect;

            if (App.Model.isAnimated==true)
            {
                 App.Setting.TextStartPoint.X = r.X;
                App.Setting.TextStartPoint.Y = r.Y;
                App.Setting.TextEndPoint.X = r.Width+ r.X;
                App.Setting.TextEndPoint.Y = r.Height + r.Y;
            }
            else
            {
                App.Setting.TextStartPoint.X = App.Model.Width/3;
                App.Setting.TextStartPoint.Y = App.Model.Height/3;
                App.Setting.TextEndPoint.X = App.Model.Width*2/3;
                App.Setting.TextEndPoint.Y = App.Model.Height*2/3;
            }
        }


        #endregion


        #region Event封装事件：变换Transform


        float mx;//TransformStartPointStart：首点初始
        float my;//TransformEndPointStart：末点初始

        //Alignment：对齐
        float TransformAlignmentDistance;   //对齐所需的距离
        //水平对齐
        float TransformHorizontalLeft;
        float TransformHorizontalCenter;
        float TransformHorizontalRight;
        //垂直对齐     
        float TransformVerticalTop;
        float TransformVerticalCenter;
        float TransformVerticalBottom;
        
        private void Transform_Start(Point p)
        {
             mx = App.Setting.TransformX;
            my = App.Setting.TransformY;

            TransformAlignmentDistance = 10 * App.Model.SX;  //Alignment：对齐

            //Snapping：吸附 
            if (App.Setting.TransformSnapping == true)
            {
                //水平对齐
                TransformHorizontalLeft = 修图.BarPage.OtherPage.Transform.GetHorizontal(HorizontalAlignment.Left);
                TransformHorizontalCenter = 修图.BarPage.OtherPage.Transform.GetHorizontal(HorizontalAlignment.Center);
                TransformHorizontalRight = 修图.BarPage.OtherPage.Transform.GetHorizontal(HorizontalAlignment.Right);
                //垂直对齐     
                TransformVerticalTop = 修图.BarPage.OtherPage.Transform.GetVertical(VerticalAlignment.Top);
                TransformVerticalCenter = 修图.BarPage.OtherPage.Transform.GetVertical(VerticalAlignment.Center);
                TransformVerticalBottom = 修图.BarPage.OtherPage.Transform.GetVertical(VerticalAlignment.Bottom);
            }
            App.Model.TipVisibility = Visibility.Collapsed;//全局提示

        }
        private void Transform_Delta(Point p)
        {
            //根据旋转与变换进行换算XY
            App.Setting.TransformX = mx + (float)((p.X - PointStart.X) / App.Model.XS);
            App.Setting.TransformY = my + (float)((p.Y - PointStart.Y) / App.Model.YS);

            //Snapping：吸附
            if (App.Setting.TransformSnapping == true)
            {
                //水平对齐    
                if (Math.Abs(App.Setting.TransformX - TransformHorizontalLeft) < TransformAlignmentDistance) App.Setting.TransformX = TransformHorizontalLeft;
                else if (Math.Abs(App.Setting.TransformX - TransformHorizontalCenter) < TransformAlignmentDistance) App.Setting.TransformX = TransformHorizontalCenter;
                else if (Math.Abs(App.Setting.TransformX - TransformHorizontalRight) < TransformAlignmentDistance) App.Setting.TransformX = TransformHorizontalRight;
                //垂直对齐   
                if (Math.Abs(App.Setting.TransformY - TransformVerticalTop) < TransformAlignmentDistance) App.Setting.TransformY = TransformVerticalTop;
                else if (Math.Abs(App.Setting.TransformY - TransformVerticalCenter) < TransformAlignmentDistance) App.Setting.TransformY = TransformVerticalCenter;
                else if (Math.Abs(App.Setting.TransformY - TransformVerticalBottom) < TransformAlignmentDistance) App.Setting.TransformY = TransformVerticalBottom;
            }

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();

            App.Model.Tip =  "X:" + ((int)App.Setting.TransformX).ToString() + "  Y:" + ((int)App.Setting.TransformY).ToString();//全局提示
         }
        private void Transform_Complete(Point p)
        {
            TipGrid.Visibility = Visibility.Collapsed;//Tip：提示

            TransformScreenToCanvas();
            修图.BarPage.OtherPage.Transform.Render();

            App.Model.Refresh++;//刷新底栏数字选择器

            App.Model.TipVisibility = Visibility.Collapsed;//全局提示

        }


        #endregion

        #region Move.《移动事件》：变换Transform


        private void Transform_Move_Start()
        {
            TransformThumbLT.Visibility = Visibility.Collapsed;
            TransformThumbRB.Visibility = Visibility.Collapsed;
            TransformThumbLB.Visibility = Visibility.Collapsed;
            TransformThumbRT.Visibility = Visibility.Collapsed;
            TransformThumbLC.Visibility = Visibility.Collapsed;
            TransformThumbRC.Visibility = Visibility.Collapsed;
            TransformThumbTC.Visibility = Visibility.Collapsed;
            TransformThumbBC.Visibility = Visibility.Collapsed;

            TransformThumbXSkew.Visibility = Visibility.Collapsed;
            TransformThumbYSkew.Visibility = Visibility.Collapsed;
            TransformThumbRotation.Visibility = Visibility.Collapsed;

            //Polyline：多边形
            PolylineWhite.Visibility = Visibility.Collapsed;
            PolylineBlack.Visibility = Visibility.Collapsed;
        }
        private void Transform_Move_Delta()
        {
            TransformCanvasToScreen();
        }
        private void Transform_Move_Complete()
        {
            TransformThumbLT.Visibility = Visibility.Visible;
            TransformThumbRB.Visibility = Visibility.Visible;
            TransformThumbLB.Visibility = Visibility.Visible;
            TransformThumbRT.Visibility = Visibility.Visible;
            TransformThumbLC.Visibility = Visibility.Visible;
            TransformThumbRC.Visibility = Visibility.Visible;
            TransformThumbTC.Visibility = Visibility.Visible;
            TransformThumbBC.Visibility = Visibility.Visible;

            TransformThumbXSkew.Visibility = Visibility.Visible;
            TransformThumbYSkew.Visibility = Visibility.Visible;
            TransformThumbRotation.Visibility = Visibility.Visible;

            //Polyline：多边形
            PolylineWhite.Visibility = Visibility.Visible;
            PolylineBlack.Visibility = Visibility.Visible;
        }


        #endregion

        #region Transform：控件


        //备份数据
        public float TransformX = 0;
        public float TransformY = 0;
        public float TransformW = 1000;
        public float TransformH = 1000;
        double LR;//左右距离
        double TB;//上下距离
        double LTRB;//左上与右下距离
        double LBRT;//左下与右上距离



        //中心点
        Point CC;

        //左上右下
        Point LT, RT, RB, LB;//四角点
        Point LC, TC, RC, BC;//四边点
        Point newLT, newRT, newRB, newLB;//四角点位置（临时计算点位置）
        Point newLC, newTC, newRC, newBC;//四边点位置（临时计算点位置）

        //旋转偏移
        Point Rotation, XSkew, YSkew;
        Point newRotation, newXSkew, newYSkew;//辅助点（临时计算点位置）



        //左上右下：改变XY使之对齐（造成自由变换的假象）
        private Vector2 TransformAlign()//左上&右下
        {
            //根据角度对齐XY
            var X = (App.Setting.TransformH - TransformH) / 2;
            var Y = (App.Setting.TransformW - TransformW) / 2;

            var VectX = (float)(App.Setting.TransformSin * Y + App.Setting.TransformCos * X);
            var VectY = (float)(App.Setting.TransformCos * Y - App.Setting.TransformSin * X);

            return new Vector2(VectX, VectY);
        }
        private Vector2 TransformReverseAlign()//右上&左下
        {
            //根据角度对齐XY
            var X = (App.Setting.TransformH - TransformH) / 2;
            var Y = (App.Setting.TransformW - TransformW) / 2;

            var VectX = (float)(-App.Setting.TransformSin * Y + App.Setting.TransformCos * X);
            var VectY = (float)(App.Setting.TransformCos * Y + App.Setting.TransformSin * X);

            return new Vector2(VectX, VectY);
        }

        private Vector2 TransformVerticalAlign()//上&下
        {
            //根据角度对齐X
            var X = (App.Setting.TransformH - TransformH) / 2;
 
            var VectX = (float)(App.Setting.TransformCos * X);
            var VectY = -(float)(  App.Setting.TransformSin * X);

            return new Vector2(VectX, VectY);
        }
        private Vector2 TransformHorizontalAlign()//左&右
        {
            //根据角度对齐Y
            var Y = (App.Setting.TransformW - TransformW) / 2;

            var VectX = (float)(App.Setting.TransformSin * Y );
            var VectY = (float)(App.Setting.TransformCos * Y );

            return new Vector2(VectX, VectY);
        }
   

        #endregion

        #region Transform：左上右下


        //左上右下：Started与Completed
        private void TransformThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            isThumb = true;//拇指正在移动，移动事件不起作用

            newLT = LT;
            newRT = RT;
            newRB = RB;
            newLB = LB;
            newLC = LC;
            newTC = TC;
            newRC = RC;
            newBC = BC;
            newXSkew = XSkew;
            newYSkew = YSkew;

            LR = Method.两点距(LC, RC);
            TB = Method.两点距(TC, BC);
            LTRB = Method.两点距(LT, RB);
            LBRT = Method.两点距(LB, RT);
             
            TransformX = App.Setting.TransformX;
            TransformY = App.Setting.TransformY;
            TransformW = App.Setting.TransformW;
            TransformH = App.Setting.TransformH;

            //改变中心点
            CC.X = (LT.X + RB.X) / 2;
            CC.Y = (LT.Y + RB.Y) / 2;
        }

     

        private void TransformThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false;//拇指正在移动，移动事件不起作用

            TransformScreenToCanvas();
            修图.BarPage.OtherPage.Transform.Render();

            App.Model.Refresh++;//刷新底栏数字选择器
        }





        //左
        private void TransformThumbLC_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newLC.X += e.HorizontalChange;
            newLC.Y += e.VerticalChange;

            var s = (float)(Method.两点距(newLC, RC) / LR);
            App.Setting.TransformW = TransformW * s;//改变W宽

            //Ratio：等比例
            if (App.Setting.TransformRatio == true) App.Setting.TransformH = TransformH * s;//改变H高

            //对齐XY
            Vector2 Align = TransformHorizontalAlign();  
            App.Setting.TransformY = TransformY - Align.X;
            App.Setting.TransformX = TransformX - Align.Y;

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();
        }
        //上
        private void TransformThumbTC_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newTC.X += e.HorizontalChange;
            newTC.Y += e.VerticalChange;

            var s = (float)(Method.两点距(newTC, BC) / TB);
            App.Setting.TransformH = TransformH * s;//改变H高

            //Ratio：等比例
            if (App.Setting.TransformRatio == true) App.Setting.TransformW = TransformW * s;//改变W宽

            //对齐XY
            Vector2 Align = TransformVerticalAlign();
            App.Setting.TransformY = TransformY - Align.X;
            App.Setting.TransformX = TransformX - Align.Y;

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();
        }
         

        //右
        private void TransformThumbRC_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newRC.X += e.HorizontalChange;
            newRC.Y += e.VerticalChange;

            var s = (float)(Method.两点距(newRC, LC) / LR);
            App.Setting.TransformW = TransformW * s;//改变W宽

            //Ratio：等比例
            if (App.Setting.TransformRatio == true) App.Setting.TransformH = TransformH * s;//改变H高

            //对齐XY
            Vector2 Align = TransformHorizontalAlign();
            App.Setting.TransformY = TransformY + Align.X;
            App.Setting.TransformX = TransformX + Align.Y;

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();
        }
        //下
        private void TransformThumbBC_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newBC.X += e.HorizontalChange;
            newBC.Y += e.VerticalChange;

            var s = (float)(Method.两点距(newBC, TC) / TB);
            App.Setting.TransformH = TransformH * s;//改变H高

            //Ratio：等比例
            if (App.Setting.TransformRatio == true) App.Setting.TransformW = TransformW * s;//改变W宽

            //对齐XY
            Vector2 Align = TransformVerticalAlign();
            App.Setting.TransformY = TransformY + Align.X;
            App.Setting.TransformX = TransformX + Align.Y;

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();
        }



        //左上
        private void TransformThumbLT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newLT.X += e.HorizontalChange;
            newLT.Y += e.VerticalChange;

            if (App.Setting.TransformRatio == false)//Ratio：等比例
            {
                App.Setting.TransformW = TransformW * (float)(Method.两点距(newLT, RT) / LR);
                App.Setting.TransformH = TransformH * (float)(Method.两点距(newLT, LB) / TB);
            }
            else
            {
                var s = (float)(Method.两点距(newLT, RB) / LTRB);
                App.Setting.TransformW = TransformW * s;
                App.Setting.TransformH = TransformH * s;
            }

            //对齐XY
            Vector2 Align = TransformAlign();
            App.Setting.TransformY = TransformY - Align.X;
            App.Setting.TransformX = TransformX - Align.Y;

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();
        }
        //右上 
        private void TransformThumbRT_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newRT.X += e.HorizontalChange;
            newRT.Y += e.VerticalChange;

            if (App.Setting.TransformRatio == false)//Ratio：等比例
            {
                App.Setting.TransformW = TransformW * (float)(Method.两点距(newRT, LT) / LR);
                App.Setting.TransformH = TransformH * (float)(Method.两点距(newRT, RB) / TB);
            }
            else
            {
                var s = (float)(Method.两点距(newRT, LB) / LBRT);
                App.Setting.TransformW = TransformW * s;
                App.Setting.TransformH = TransformH * s;
            }

            //对齐XY
            Vector2 Align = TransformReverseAlign();
            App.Setting.TransformY = TransformY - Align.X;
            App.Setting.TransformX = TransformX + Align.Y;

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();
        }
        //右下 
        private void TransformThumbRB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newRB.X += e.HorizontalChange;
            newRB.Y += e.VerticalChange;

            if (App.Setting.TransformRatio==false)//Ratio：等比例
            {
                App.Setting.TransformW = TransformW * (float)(Method.两点距(newRB, LB) / LR);
                App.Setting.TransformH = TransformH * (float)(Method.两点距(newRB, RT) / TB);
            }
            else
            {
                var s = (float)(Method.两点距(newRB, LT) / LTRB);
                App.Setting.TransformW = TransformW * s;
                App.Setting.TransformH = TransformH * s;
            }

            //对齐XY
            Vector2 Align = TransformAlign();
            App.Setting.TransformY = TransformY + Align.X;
            App.Setting.TransformX = TransformX + Align.Y;

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();
        }
        //左下 
        private void TransformThumbLB_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newLB.X += e.HorizontalChange;
            newLB.Y += e.VerticalChange;

            if (App.Setting.TransformRatio == false)//Ratio：等比例
            {
                App.Setting.TransformW = TransformW * (float)(Method.两点距(newLB, RB) / LR);
                App.Setting.TransformH = TransformH * (float)(Method.两点距(newLB, LT) / TB);
            }
            else
            {
                var s = (float)(Method.两点距(newLB, RT) / LBRT);
                App.Setting.TransformW = TransformW * s;
                App.Setting.TransformH = TransformH * s;
            }

            //对齐XY
            Vector2 Align = TransformReverseAlign();
            App.Setting.TransformY = TransformY + Align.X;
            App.Setting.TransformX = TransformX - Align.Y;

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();
        }


        #endregion

        #region Transform：旋转偏移


        //旋转偏移：Started与Completed
        private void TransformThumbRotation_DragStarted(object sender, DragStartedEventArgs e)
        {
            isThumb = true;//拇指正在移动，移动事件不起作用

            newRotation = Rotation;
            newXSkew = XSkew;
            newYSkew = YSkew;

            //改变中心点
            CC.X = (LT.X + RB.X) / 2;
            CC.Y = (LT.Y + RB.Y) / 2;

             App.Model.TipVisibility = Visibility.Visible;//全局提示
        }
        private void TransformThumbRotation_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isThumb = false;//拇指正在移动，移动事件不起作用
            TipGrid.Visibility = Visibility.Collapsed;//Tip：提示

            Transform_Move_Delta();
            修图.BarPage.OtherPage.Transform.Render();

            App.Model.TipVisibility = Visibility.Collapsed;//全局提示
        }






        //旋转
        private void TransformThumbRotation_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newRotation.X += e.HorizontalChange;
            newRotation.Y += e.VerticalChange;

            double angle = 90- Method.点角度(CC, newRotation);//Angle（0~360）
            if (App.Setting.TransformH < 0) angle = (angle - 180) % 180;//如果上下翻转，旋转会相反

            App.Setting.TransformAngle = (float)(angle / 180d * Math.PI);

            TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();

            App.Model.Tip = ((float)angle).ToString()+"%";//全局提示
        }
        //X偏移（下方）
        private void TransformThumbXSkew_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newXSkew.X += e.HorizontalChange;
            newXSkew.Y += e.VerticalChange;

            double angle =90 + Method.点角度(CC, newXSkew);//Angle（0~360）
            if (App.Setting.TransformH < 0) angle = -angle;//如果上下翻转，偏移会相反

            App.Setting.TransformXSkew = App.Setting.TransformAngle+(float)(angle / 180d * Math.PI);

             TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();

            App.Model.Tip = ((float)angle).ToString() + "%";//全局提示
        }
        //Y偏移（右方）
        private void TransformThumbYSkew_DragDelta(object sender, DragDeltaEventArgs e)
        {
            newYSkew.X += e.HorizontalChange;
            newYSkew.Y += e.VerticalChange;

            double angle = - Method.点角度(CC, newYSkew);//Angle（0~360）
            if (App.Setting.TransformW < 0) angle = -angle;//如果左右翻转，偏移会相反

            App.Setting.TransformYSkew = -App.Setting.TransformAngle + (float)(angle / 180d * Math.PI);

             TransformCanvasToScreen();
            修图.BarPage.OtherPage.Transform.Render();

            App.Model.Tip = ((float)angle).ToString() + "%";//全局提示
        }


        #endregion

        #region Transform：算法


        public  void TransformCanvasToScreen()
        { 
            //Canvas
            Matrix3x2 m = App.Setting.TransformMatrix;

            float L = (float)App.Setting.TransformRect.Left;
            float R = (float)App.Setting.TransformRect.Right;
            float T = (float)App.Setting.TransformRect.Top;
            float B = (float)App.Setting.TransformRect.Bottom;


            //Screen
            LT = App.Model.CanvasToScreen(Vector2.Transform(new Vector2(L, T), m).ToPoint());
            RT = App.Model.CanvasToScreen(Vector2.Transform(new Vector2(R, T), m).ToPoint());
            RB = App.Model.CanvasToScreen(Vector2.Transform(new Vector2(R, B), m).ToPoint());
            LB = App.Model.CanvasToScreen(Vector2.Transform(new Vector2(L, B), m).ToPoint());

            LC = new Point((LT.X + LB.X) / 2, (LT.Y + LB.Y) / 2);
            TC = new Point((LT.X + RT.X) / 2, (LT.Y + RT.Y) / 2);
            RC = new Point((RT.X + RB.X) / 2, (RT.Y + RB.Y) / 2);
            BC = new Point((RB.X + LB.X) / 2, (RB.Y + LB.Y) / 2);

            XSkew = Method.距在线上点(32, BC, TC);
            YSkew = Method.距在线上点(32, RC, LC);
            Rotation = Method.距在线上点(32, TC, BC);


            //Thumb
            Canvas.SetLeft(TransformThumbLT, LT.X - 15);  Canvas.SetTop(TransformThumbLT, LT.Y - 15);
            Canvas.SetLeft(TransformThumbRT, RT.X - 15);  Canvas.SetTop(TransformThumbRT, RT.Y - 15);
            Canvas.SetLeft(TransformThumbRB, RB.X - 15);  Canvas.SetTop(TransformThumbRB, RB.Y - 15);
            Canvas.SetLeft(TransformThumbLB, LB.X - 15);  Canvas.SetTop(TransformThumbLB, LB.Y - 15);

            Canvas.SetLeft(TransformThumbLC,  LC.X- 15);  Canvas.SetTop(TransformThumbLC, LC.Y - 15);
            Canvas.SetLeft(TransformThumbTC,  TC.X- 15);  Canvas.SetTop(TransformThumbTC, TC.Y - 15);
            Canvas.SetLeft(TransformThumbRC, RC.X- 15);  Canvas.SetTop(TransformThumbRC, RC.Y- 15);
            Canvas.SetLeft(TransformThumbBC,BC.X  - 15);  Canvas.SetTop(TransformThumbBC,BC.Y - 15);

            Canvas.SetLeft(TransformThumbXSkew, XSkew.X - 15); Canvas.SetTop(TransformThumbXSkew, XSkew.Y - 15);
            Canvas.SetLeft(TransformThumbYSkew, YSkew.X - 15); Canvas.SetTop(TransformThumbYSkew, YSkew.Y - 15);
            Canvas.SetLeft(TransformThumbRotation, Rotation.X - 15); Canvas.SetTop(TransformThumbRotation, Rotation.Y - 15);

            //Polyline：多边形
            PolylineWhite.Points = new PointCollection{Rotation,TC,RT,RB,LB,LT,TC};
             PolylineBlack.Points = new PointCollection { Rotation, TC, RT, RB, LB, LT, TC };
        }
        private void TransformScreenToCanvas()
        {
        }

        

        private void TransformChange()//跟随选区变换
        {
            int t = App.Model.Tool;
            //Layer：图片
            if (t == 400) App.Setting.TransformRect = App.Model.SecondCanvasBitmap.Bounds;
            //Mask：粘贴
            else if (t == 102)
            {
                 if (App.Model.isAnimated == true)
                {
                    App.Judge();//判断选区是否存在并改变选区边界
                    App.Setting.TransformRect = App.Model.MaskRect;
                }
                else App.Setting.TransformRect = App.GetBounds(App.Model.Clipboard.GetPixelColors(), App.Model.Clipboard.SizeInPixels);
             }

            //Transform：变换      //Transform：变换选区
            else if (t == 308 || t == 111)
            {
                if (App.Model.isAnimated == true)
                {
                    App.Judge();//判断选区是否存在并改变选区边界
                    App.Setting.TransformRect = App.Model.MaskRect;
                }
                else App.Setting.TransformRect = App.GetCurrentBounds();
            }
             
            App.Setting.TransformX = 0;
            App.Setting.TransformY = 0;
            App.Setting.TransformW = (float)App.Setting.TransformRect.Width;
            App.Setting.TransformH = (float)App.Setting.TransformRect.Height;
            App.Setting.TransformXSkew = 0;
            App.Setting.TransformYSkew = 0;
            App.Setting.TransformAngle = 0;

            //Mask：粘贴  //Layer：图片
            if (t == 102&&t == 400) 
            {
                 //居中
                修图.BarPage.OtherPage.Transform.Horizontal(HorizontalAlignment.Center);
                修图.BarPage.OtherPage.Transform.Vertical(VerticalAlignment.Center);
             }
      }



        #endregion

          

    }
}
