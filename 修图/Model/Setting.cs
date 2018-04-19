using System;
using System.Numerics;
using System.Collections.Generic;
using Windows.UI;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;

namespace 修图.Model
{
    public class Setting
    {

        #region Tool：工具


        //Ruler：标尺线
        public bool isRulerHorizontal = false;//是否水平移动标尺线
        public bool isRulerVertical = false;//是否垂直移动标尺线
        public int RulerHorizontalIndex = -1;//水平选中点索引
        public int RulerVerticalIndex = -1;//垂直选中点索引
        public List<float> RulerHorizontalList = new List<float>();
        public List<float> RulerVerticalList = new List<float>();


        //Liquify：液化
        public Color LiquifyColor = Color.FromArgb(255, 128, 128, 255);//蓝紫色

        public int LiquifyMode=1;//模式
        public CanvasCommandList Liquify;//画笔
        public float LiquifySize = 100;
        public float LiquifyAmount = 10;
              

        public void LiquifyFill(ICanvasResourceCreator rc)//填充蓝色渐变圆形
        {
            Liquify = new CanvasCommandList(rc);

            using (var ds = Liquify.CreateDrawingSession())
            {             
                //画渐变圆形
                CanvasRadialGradientBrush brush = PaintBrush(rc,new Vector2(LiquifySize), LiquifySize, 0, LiquifyAmount/100, LiquifyColor);
                ds.FillCircle(LiquifySize , LiquifySize, LiquifySize , brush);
            }
        }
        public void LiquifyFill(ICanvasResourceCreator rc, CanvasBitmap cb)//填充一个位图
        {
            Liquify = new CanvasCommandList(rc);

            using (var ds = Liquify.CreateDrawingSession())
            {
                //画渐变圆形
                CanvasRadialGradientBrush brush = PaintBrush(rc, new Vector2(LiquifySize), LiquifySize, 0, LiquifyAmount/100, LiquifyColor);
                ds.FillCircle(LiquifySize, LiquifySize, LiquifySize, brush);

                //画位图
                ScaleEffect se = new ScaleEffect
                {
                    Source = cb,
                    Scale = new Vector2(LiquifySize * 2 / (float)cb.Size.Width)
                };
                ds.DrawImage(se, 0, 0, new Rect(0, 0, LiquifySize*2, LiquifySize*2), 1, CanvasImageInterpolation.Linear, CanvasComposite.SourceIn);
            }
        }
        public void LiquifyDraw(ICanvasResourceCreator rc, Vector2 end, Vector2 start)//描边一个位图，用于涂抹改变方向时改变颜色的R与B分量
        {
            float R = 128 + (end.X - start.X) / 10 * 128;
            float G = 128 + (end.Y - start.Y) / 10 * 128;

            Liquify = new CanvasCommandList(rc);
            using (var ds = Liquify.CreateDrawingSession())
            {
                //画圆环
                var co = Color.FromArgb((byte)(255 * LiquifyAmount / 100), (byte)R, (byte)G, 255);
                ds.FillCircle(LiquifySize, LiquifySize, LiquifySize, co);
            }
        }


        //Cursor：光标

        //所有图层的选中组矩形
        public Point CursorGroupStart;
        public Point CursorGroupEnd;
        public void CursorGroup()//更改选中组的矩形点
        {
            Rect Rect = App.Model.Layers[App.Model.Index].VectRect;
            CursorGroupStart.X = Rect.Left;
            CursorGroupStart.Y = Rect.Top;
            CursorGroupEnd.X = Rect.Right;
            CursorGroupEnd.Y = Rect.Bottom;

            foreach (var layer in App.Model.Layers)
            {
                if ( layer.isSelected == true)
                {
                    if (  CursorGroupStart.X > layer.VectRect.Left)   CursorGroupStart.X = layer.VectRect.Left;
                    if (  CursorGroupStart.Y > layer.VectRect.Top)   CursorGroupStart.Y = layer.VectRect.Top;
                    if (  CursorGroupEnd.X < layer.VectRect.Right)   CursorGroupEnd.X = layer.VectRect.Right;
                    if (  CursorGroupEnd.Y < layer.VectRect.Bottom)   CursorGroupEnd.Y = layer.VectRect.Bottom;
                }
            }
        }

        //Straw：吸管
        public bool isStrawCurrent;


        //Magic：魔棒
        public float MagicTolerance = 12;


        //Rectangle：矩形
        public float Radius;
        public bool isCenter = false;
        public bool isSquare = false;


        //Mixer：混色     
        private Vector4 MixerVector = new Vector4();
        public Color MixerColor = Colors.White;
        public List<Color> MixerColors = new List<Color> { Colors.Transparent };
        public int MixerDry = 3;//干湿：用来判断涂抹颜色集合的最大数量
        public void MixerBlend()//混色
        {
            //颜色累加
            MixerVector = new Vector4();
            foreach (var co in MixerColors)
            {
                MixerVector.X += co.A;
                MixerVector.Y+= co.R;
                MixerVector.Z += co.G;
                MixerVector.W += co.B;
            }

            //颜色混合
            byte A = (byte)(MixerVector.X / MixerColors.Count);
            byte R = (byte)(MixerVector.Y / MixerColors.Count);
            byte G = (byte)(MixerVector.Z / MixerColors.Count);
            byte B = (byte)(MixerVector.W / MixerColors.Count);
            MixerColor = Color.FromArgb(A, R, G, B);

            //清空
            if (MixerColors.Count > MixerDry) MixerColors.RemoveAt(0);
        }



        //Paint：画笔

        //画笔   
        public CanvasCommandList Paint;
        public CanvasCommandList PaintShow;
        public float PaintWidth = 12;
        public float PaintHard = 1;
        public float PaintOpacity = 1;
        public float PaintSpace = 0.25f;

        public bool isPaintBitmap;
        public CanvasBitmap PaintBitmap;

        public CanvasRadialGradientBrush PaintBrush(ICanvasResourceCreator rc, float R, float Hard, float Opacity, Color Color)
        {
            CanvasGradientStop[] stops = new CanvasGradientStop[3]
            {
             new CanvasGradientStop
             {
                 Position=0,
               Color=   Color.FromArgb((byte)(Color.A * Opacity), Color.R, Color.G, Color.B)
             },
            new CanvasGradientStop
            {
                Position = 0.8f,
                Color = Color.FromArgb((byte)(Color.A * Opacity*Hard), Color.R, Color.G, Color.B)
            },
            new CanvasGradientStop
            {
                Position = 1,
                Color =  Colors.Transparent
            }
        };
            CanvasRadialGradientBrush PaintBrush = new CanvasRadialGradientBrush(rc, stops);



            PaintBrush.Center = new Vector2(0, 0);
            PaintBrush.RadiusX = PaintBrush.RadiusY = R;
            return PaintBrush;
        }
        public CanvasRadialGradientBrush PaintBrush(ICanvasResourceCreator rc,Vector2 v, float R, float Hard, float Opacity, Color Color)
        {
            CanvasRadialGradientBrush PaintBrush =

                new CanvasRadialGradientBrush(rc,
                Color.FromArgb((byte)(Color.A * Opacity), Color.R, Color.G, Color.B),
                Color.FromArgb((byte)(Color.A * Opacity * Hard), Color.R, Color.G, Color.B));

            PaintBrush.Center = v;
            PaintBrush.RadiusX = PaintBrush.RadiusY = R;
            return PaintBrush;
        }


        // 设置画笔（R：Radius）
        public void PaintSet(ICanvasResourceCreator rc, float R,Color co)
        {       
            //Paint：绘画
              Paint = new CanvasCommandList(rc);

            using (CanvasDrawingSession ds =   Paint.CreateDrawingSession())
            {
                ds.FillEllipse(0, 0, R, R,   PaintBrush(rc,R, PaintHard,PaintOpacity, co));
            }


            //Show：展示
            var radius = (float)Math.Sqrt(R);
              PaintShow = new CanvasCommandList(rc);

            using (CanvasDrawingSession ds =   PaintShow.CreateDrawingSession())
            {
                ds.FillEllipse(0, 0, radius, radius,   PaintBrush(rc, radius, PaintHard,PaintOpacity, Colors.White));
            } 
        }
        public void PaintSet(ICanvasResourceCreator rc,CanvasBitmap cb, float R, Color co)
        {
            //Paint：绘画
              Paint = new CanvasCommandList(rc);

            using (CanvasDrawingSession ds =   Paint.CreateDrawingSession())
            {
                ds.FillEllipse(0, 0, R, R, PaintBrush(rc, R, PaintHard, PaintOpacity,co));

                ICanvasImage ci = new ScaleEffect
                {
                    Source = cb,
                    Scale = new Vector2(2 * R / cb.SizeInPixels.Width)
                };
                ds.DrawImage(ci, -R, -R, ci.GetBounds(rc), 1, CanvasImageInterpolation.HighQualityCubic, CanvasComposite.DestinationIn);
            }



            //Show：展示
            var radius = (float)Math.Sqrt(R);
              PaintShow = new CanvasCommandList(rc);

            using (CanvasDrawingSession ds =   PaintShow.CreateDrawingSession())
            {
                ds.FillEllipse(0, 0, radius, radius, PaintBrush(rc, radius,PaintHard,PaintOpacity ,Colors.White));
                ICanvasImage ci = new ScaleEffect
                {
                    Source = cb,
                    Scale = new Vector2(2 * radius / cb.SizeInPixels.Width)
                };
                ds.DrawImage(ci, -radius, -radius, ci.GetBounds(rc), 1, CanvasImageInterpolation.HighQualityCubic, CanvasComposite.DestinationIn);
             }
        }
       

        //Pen：钢笔
        public int PenMode;//添加模式&节点模式&模式模式
        public List<Pen> PenVectorList = new List<Pen> { };
        public CanvasStrokeStyle PenStrokeStyle = new CanvasStrokeStyle();
        public int PenDash = 1;//贝塞尔曲线控制点的模式（0，1，2，3）

        public float PenWidth = 4;//描边宽度


        //Mask：选区
         public Color MaskColor = Color.FromArgb(255, 0, 141, 255);//蓝色

        #endregion


        #region Adjust：调整


        //调整的透明度
        public float AdjustOpacity = 0;

        //原本的调整数组
        public float[] AdjustOriginArray = new float[] 
        {
            1.00f, 0.00f, 0.00f, 0.00f, 0.00f,

            1.00f, 0.00f, 0.00f, 0.00f,

            0.00f, 0.00f, 0.00f, 0.00f
        };

        //实际的调整数组
        public float[] AdjustArray = new float[] 
        {
            1.00f, 0.00f, 0.00f, 0.00f, 0.00f,

            1.00f, 0.00f, 0.00f,0.00f,

            0.00f, 0.00f, 0.00f, 0.00f
        };


        #endregion


        #region Geometry：几何


         //Geometry：几何
        public int GeometryMode; //几何是否描边
        public CanvasStrokeStyle GeometryStrokeStyle = new CanvasStrokeStyle();
        public float GeometryWidth = 4;//描边宽度


        //RoundRect：圆角矩形
        public float RoundRectRadius = 20;

        //Image：图像
        public CanvasBitmap Image;
        public float ImageWidth;
        public float ImageHeight;
        public bool isImageRatio = true;

        //Word：字词
        public String WordFontFamily = "微软雅黑";
        public String WordText = "Text";
        public FontStyle WordFontStyle = FontStyle.Normal;

        //Pentagon：多边形
        public int PentagonCount = 5;//多边形的边数

        //Star：星星
        public int StarCount = 5; //星星的边数
        public float StarInner = 0.5f;//内半径

        //Pie：饼图
        public float PieInner = 0;//内半径
        public float PieSweep = (float)Math.PI / 2 * 3;//扫描角度

        //Cog：齿轮
        public int CogCount = 8;
        public float CogInner = 0.7f;//内半径
        public float CogTooth = 0.3f;//牙齿百分比
        public float CogNotch = 0.6f;//切口百分比

        //Arrow：箭头
        public float ArrowHead = 30;//头部长度

        //Capsule：胶囊
        public float CapsuleRadius = 20;//半径





        #endregion


        #region Mask：选区


        public float MaskOpacity = 0.5f;

        //羽化（0~100）
        public bool isBorder;
        public float Feather = 0;
        public int Border = 0;


        #endregion


        #region Effect：特效


        //曝光（-2~2）
        public float Exposure = 0;

        //亮度 （0~2）（Black：0~1/5）（White：4/5~1） 
        public float Brightness = 1f;
        public float BrightnessBlackX = 0f;
        public float BrightnessBlackY = 0f;
        public float BrightnessWhiteX = 1f;
        public float BrightnessWhiteY = 1f;

        //饱和度（0~2）
        public float Saturation = 1;

        //色相（0~6）
        public float HueAngle = 0;

        //对比度（-1~1）
        public float Contrast = 0;

        //冷暖（-1~1）（-1~1）
        public float Temperature = 0;
        public float TemperatureTint = 0;

        //高光/阴影（-1~1）（-1~1）（-1~1）(0~10)
        public float Shadows = 0;
        public float Highlights = 0;
        public float Clarity = 0;
        public float MaskBlurAmount = 1.25f;
        public bool SourceIsLinearGamma = false;//是否使用伽马


        #endregion


        #region Effect1：特效1


        //高斯模糊（0~100）（0~2*Pi）
        public float BlurAmount = 10;
        public float BlurAngle = 0;
        public EffectOptimization Optimization = EffectOptimization.Speed;//质量：低中高
        public EffectBorderMode BorderMode = EffectBorderMode.Soft;//边界：软硬


        //锐化（0~10）
        public float Sharpen = 10;

        //阴影（0~10）
        public float ShadowAmount = 50;
        public float ShadowX = 10;
        public float ShadowY = 10;
        public float ShadowOpacity = 1;

        //消除颜色（0~1）
        public float Tolerance = 0.1f;
        public bool ChromaKeyFeather = false;//是否羽化
        public bool InvertAlpha = false;//是否反转阿尔法


        //边缘检测（0~1）（0~10）
        public float EdgeAmount = 0.5f;
        public float EdgeBlurAmount;
        public EdgeDetectionEffectMode EdgeMode;
        public CanvasAlphaMode EdgeAlphaMode;
        public bool OverlayEdges = false;


        //边界
        public CanvasEdgeBehavior EdgeBehaviorX = CanvasEdgeBehavior.Wrap;
        public CanvasEdgeBehavior EdgeBehaviorY = CanvasEdgeBehavior.Wrap;


        //浮雕（0~10）（0~2*Pi）
        public float EmbossAmount = 1;
        public float EmbossAngle = 0;


        //暗示灯光 
        public CanvasHorizontalAlignment LightHorizontal = CanvasHorizontalAlignment.Left;
        public Point LightingStartPoint = new Point();
        public Point LightingEndPoint = new Point(100, 100);
        public float Light0 = 1;
        public float Light1 = 10;
        public float Light2 = 1;
        public float Light3 = 1;
        public float Light4 = 1;
        public Color LightColor = Colors.White;
        public float LightFocus = 1;
        public float LightAngle = (float)Math.PI/2;




        #endregion


        #region Effect2：特效2


        //离散转让（0~1）
        public bool RedDisable, GreenDisable, BlueDisable;
        public bool isTable;
        public float[] RedTable = new float[] { 0, 0.5f, 1 };
        public float[] GreenTable = new float[] { 0, 0.5f, 1 };
        public float[] BlueTable = new float[] { 0, 0.5f, 1 };


        //装饰图案
        public float VignetteAmountr;


        //色彩
        public float Tint = 1;


        //伽马转让（0~1）
        public int GammaTransferMode;

        public float AlphaAmplitude = 1;
        public float AlphaExponent = 1;
        public float AlphaOffset;

        public float RedAmplitude = 1;
        public float RedExponent = 1;
        public float RedOffset;

        public float GreenAmplitude = 1;
        public float GreenExponent = 1;
        public float GreenOffset;

        public float BlueAmplitude = 1;
        public float BlueExponent = 1;
        public float BlueOffset;



        #endregion


        #region Effect3：特效3


        //Glass：玻璃
        public float GlassAmount = 200;

        //PinchPunch：收缩膨胀
        public float PinchPunchAmount = 100;
        public float PinchPunchRadius= 100;


        //Morphology：形态学
        public float MorphologyAmount = 200;
         



        #endregion

         

        #region Other：杂项


        //Crop：裁切
        public Point CropStartPoint = new Point();
        public Point CropEndPoint = new Point(100, 100);
        public float CropAngle;

        public float CropX
        {
            set
            {
                var X = value - (float)(Math.Min(CropStartPoint.X, CropEndPoint.X));
                CropStartPoint.X += X; CropEndPoint.X += X;
            }
            get => (float)(Math.Min(CropStartPoint.X, CropEndPoint.X));
        }
        public float CropY
        {
            set
            {
                var Y = value - (float)(Math.Min(CropStartPoint.Y, CropEndPoint.Y));
                CropStartPoint.Y += Y; CropEndPoint.Y += Y;
            }
            get => (float)(Math.Min(CropStartPoint.Y, CropEndPoint.Y));
        }
        public float CropW
        {
            set
            {
                if (CropStartPoint.X > CropEndPoint.X) CropStartPoint.X = CropEndPoint.X + value;
                else CropEndPoint.X = CropStartPoint.X + value;
            }
            get => (float)(Math.Abs(CropStartPoint.X - CropEndPoint.X));
        }
        public float CropH
        {
            set
            {
                if (CropStartPoint.Y > CropEndPoint.Y) CropStartPoint.Y = CropEndPoint.Y + value;
                else CropEndPoint.Y = CropStartPoint.Y + value;
            }
            get => (float)(Math.Abs(CropStartPoint.Y - CropEndPoint.Y));
        }


        //Fill：填充
        public int FillMode = 0;
        public CanvasStrokeStyle StrokeStyle = new CanvasStrokeStyle();

        
        //Gradient：渐变
        public bool isGradientRadial;
        public Point GradientStartPoint = new Point();
        public Point GradientEndPoint = new Point(100, 100);
        public CanvasEdgeBehavior GradientEdgeBehavior;

        public int GradientCurrent;//当前 用数字表示选中索引，-1未未选中
        public List<CanvasGradientStop> GradientStops = new List<CanvasGradientStop>
        {
            new CanvasGradientStop
            {
                Position = 0f,
                Color = Color.FromArgb(0, 0, 0, 0)
            }, 
            new CanvasGradientStop
            {
                Position = 1f,
                Color = Colors.Black
            },
        };
  
        public ICanvasBrush GradientBrush//得到笔刷
        {
            get
            {
                Vector2 sv = GradientStartPoint.ToVector2();
                Vector2 ev = GradientEndPoint.ToVector2();

                if (isGradientRadial == false)
                {
                    CanvasLinearGradientBrush lgb = new CanvasLinearGradientBrush(
                        App.Model.VirtualControl,
                        GradientStops.ToArray(),
                        GradientEdgeBehavior,
                        CanvasAlphaMode.Premultiplied);
                    lgb.StartPoint = sv;
                    lgb.EndPoint = ev;
                    return lgb;
                }
                else
                {
                    CanvasRadialGradientBrush rgb = new CanvasRadialGradientBrush(
                        App.Model.VirtualControl,
                        GradientStops.ToArray(),
                        GradientEdgeBehavior,
                        CanvasAlphaMode.Premultiplied);
                    rgb.Center = sv;
                    rgb.RadiusX = ev.X - sv.X;
                    rgb.RadiusY = ev.Y - sv.Y;
                    return rgb;
                }
            }
        }
        public Color GradientGetCurrent//得到当前
        {
            get => GradientStops[GradientCurrent].Color;
        }
        public void GradientSetCurrent(Color co)//设置颜色
        {
            GradientStops[GradientCurrent] = new CanvasGradientStop
            {
                Position =   GradientStops[GradientCurrent].Position,
                Color = co
            };
        }
        public void GradientSetCurrent(float Position)//设置位置
        {
            GradientStops[GradientCurrent] = new CanvasGradientStop
            {
                Position = Position,
                Color =   GradientStops[GradientCurrent].Color,
            };
        }




        //Fade：渐隐
        public bool isFadeRadial;
        public Point FadeStartPoint = new Point();
        public Point FadeEndPoint = new Point(100, 100);
        public CanvasEdgeBehavior FadeEdgeBehavior;

        public int FadeCurrent;//当前 用数字表示选中索引，-1未未选中
        public List<CanvasGradientStop> FadeStops = new List<CanvasGradientStop>
        {
            new CanvasGradientStop
            {
                Position = 0f,
                Color = Color.FromArgb(255,  0, 141, 255)
            },
            new CanvasGradientStop
            {
                Position = 1f,
                Color = Color.FromArgb(0, 0, 141, 255)
            },
        };

        public ICanvasBrush FadeBrush//得到笔刷
        {
            get
            {
                Vector2 sv = FadeStartPoint.ToVector2();
                Vector2 ev = FadeEndPoint.ToVector2();

                if (isFadeRadial == false)
                {
                    CanvasLinearGradientBrush lgb = new CanvasLinearGradientBrush(
                        App.Model.VirtualControl,
                        FadeStops.ToArray(),
                        FadeEdgeBehavior,
                        CanvasAlphaMode.Premultiplied);
                    lgb.StartPoint = sv;
                    lgb.EndPoint = ev;
                    return lgb;
                }
                else
                {
                    CanvasRadialGradientBrush rgb = new CanvasRadialGradientBrush(
                        App.Model.VirtualControl,
                        FadeStops.ToArray(),
                        FadeEdgeBehavior,
                        CanvasAlphaMode.Premultiplied);
                    rgb.Center = sv;
                    rgb.RadiusX = ev.X - sv.X;
                    rgb.RadiusY = ev.Y - sv.Y;
                    return rgb;
                }
            }
        }
        public int FadeGetCurrent//得到当前
        {
            get => FadeStops[FadeCurrent].Color.A;
        }
        public void FadeSetCurrent(int opaity)//设置颜色
        {
            FadeStops[FadeCurrent] = new CanvasGradientStop
            {
                Position =   FadeStops[FadeCurrent].Position,
                Color =Color.FromArgb((byte)opaity,0, 141, 255)
            }; 
        }
        public void FadeSetCurrent(float Position)//设置位置
        {
            FadeStops[FadeCurrent] = new CanvasGradientStop
            {
                Position = Position,
                Color =   FadeStops[FadeCurrent].Color,
            };
        }
         


        //Text：文字
        public CanvasTextLayout TextLayout;
        public bool isUnderline=false;
        public CanvasTextFormat TextFormat = new CanvasTextFormat()
        {
            FontSize = 100,
            FontFamily = "微软雅黑",
        };
        public String Text = "Text";
        public Point TextStartPoint = new Point();
        public Point TextEndPoint = new Point(1000, 1000);
        
        public float TextX { get => (float)(Math.Min(TextStartPoint.X, TextEndPoint.X)); }
        public float TextY { get => (float)(Math.Min(TextStartPoint.Y, TextEndPoint.Y)); }
        public float TextW { get => (float)(Math.Abs(TextStartPoint.X - TextEndPoint.X)); }
        public float TextH { get => (float)(Math.Abs(TextStartPoint.Y - TextEndPoint.Y)); }


        //Grids：网格线
        public float GridsSpace = 10;


        //Transform：变换
        public Rect TransformRect;//变换源矩形
        public bool TransformSnapping = true;//Snapping：吸附
        public bool TransformRatio = true;//Ratio：等比例


        public float TransformX = 0;
        public float TransformY = 0;
        public float TransformW = 1000;
        public float TransformH = 1000;

        public float TransformXSkew = 0;
        public float TransformYSkew = 0;

        private float transformAngle = 0;
        public float TransformAngle
        {
            set
            {
                TransformSin = Math.Sin(TransformAngle);
                TransformCos = Math.Cos(TransformAngle);
                transformAngle = value;
            }
            get => transformAngle;
        }
        public double TransformSin = 0;
        public double TransformCos = 1;
        public Point TransformCenterPoint = new Point();//变换中心点

        public Matrix3x2 TransformMatrix
        {
            get
            { 
                var xs = (float)(TransformW / TransformRect.Width);
                var ys = (float)(TransformH / TransformRect.Height);


                return
                   //位移正常，旋转不正常（神奇代码！勿动）
                   Matrix3x2.CreateTranslation(-(float)(TransformRect.X + TransformRect.Width / 2), -(float)(TransformRect.Y + TransformRect.Height / 2)) //中心点移到（0，0）

                    * Matrix3x2.CreateSkew(TransformXSkew,   TransformYSkew)//XY偏移
                    * Matrix3x2.CreateScale(xs, ys)//WH

                    * Matrix3x2.CreateRotation(TransformAngle)

                 * Matrix3x2.CreateTranslation((float)(TransformRect.X + TransformRect.Width / 2), (float)(TransformRect.Y + TransformRect.Height / 2)) //中心点移到（0，0）
                  * Matrix3x2.CreateTranslation(TransformX, TransformY)
                    ;
            }
        }



        //Transform3D：变换3D
        public Rect Transform3DRect;//变换源矩形
        public HorizontalAlignment Transform3DMode;//Move、Scale、Rotate

        public Vector3 Transform3DMove;
        public Vector3 Transform3DScale = new Vector3(1, 1, 1);
        public Vector3 Transform3DRotate;

        public Matrix4x4 Transform3DMatrix
        {
            get
            {
                Vector3 CenterVect = new Vector3((float)(Transform3DRect.X + Transform3DRect.Width / 2), (float)(Transform3DRect.Y + Transform3DRect.Height / 2), 0);

                 return
                   //位移正常，旋转不正常（神奇代码！勿动）
                   Matrix4x4.CreateTranslation(- CenterVect) //中心点移到（0，0）
              
                   *Matrix4x4.CreateTranslation(Transform3DMove)
                   *Matrix4x4.CreateScale(Transform3DScale)
                   * Matrix4x4.CreateRotationX(Transform3DRotate.X)* Matrix4x4.CreateRotationY(Transform3DRotate.Y)* Matrix4x4.CreateRotationZ(Transform3DRotate.Z)

                   * Matrix4x4.CreateTranslation(CenterVect) //中心点移到（0，0）
                     ;
            }
        }


        #endregion


    }
}
