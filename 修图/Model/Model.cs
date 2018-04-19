using System.Numerics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Geometry;

namespace 修图.Model
{
   public class Model : INotifyPropertyChanged
    {

        #region Pressure & DPI：压力 & 分辨率


        public float Radius;//画笔的旋转角度

        public float pressure = 1;
        public float Pressure//画笔的压力
        {
            get => pressure;
            set
            {
                if (value>0.01)
                {
                    pressure = value;
                    PressureVector2 = new Vector2(value);
                }
            }
        }
        public Vector2 PressureVector2;//画笔的压力


        public float DPIReady;//真正的DPI（240）
        public float DPI = 96;//
        public float DPIScale {get => DPIReady /DPI; }//DPI的比例（240/96）


        #endregion


        //Main：用来绑定显示主页面
        private Visibility startVisibility = Visibility.Visible;
        public Visibility StartVisibility
        {
            get { return startVisibility; }
            set
            {
                startVisibility = value;
                this.OnPropertyChanged("StartVisibility");
            }
        }


        //Undo：撤销          
        public bool isCanUndo = true;//是否开启撤销
         public enum UndeoType
        {
            Null,//空

            Targe,//渲染目标改变
            Mask,//选区改变

            Index,//图层索引改变
            Collection,//图层顺序改变
            Add,//图层增加改变
            Remove,//图层移除改变

            MergeDown,//图层合并改变


            Visual,//可视改变
            Opacity,//透明度改变
            Blend,//混合改变

            Tool, //工具改变

            Cursor,//光标改变
            Pen,//钢笔改变
        }
        public List<Undo> Undos = new List<Undo> { };//撤销列
        public int UndoIndex = 0;//当前的撤销步骤的索引



        //墨水控件
        public InkCanvas InkCanvas = new InkCanvas();
        //画布控件
        public CanvasVirtualControl VirtualControl = new CanvasVirtualControl();
        //动画控件
        public CanvasAnimatedControl AnimatedControl = new CanvasAnimatedControl();
        //主图层
        public  ObservableCollection<Layer> Layers = new ObservableCollection<Layer>();
        public CanvasRenderTarget CurrentRenderTarget//返回当前图层的渲染目标
        {
            get => Layers[Index].CanvasRenderTarget;
            set => Layers[Index].CanvasRenderTarget = value;
        }
        public bool CurrentVisual//返回当前图层的可视
        {
            get => Layers[Index].Visual;
            set => Layers[Index].Visual = value;
        }
        //Undo：撤销
        public int LayersCount; //Layers的数量，用来判断集合改变时是否是List.Move触发的



        #region Global：全局


        //图层索引
        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                if (value >= 0 && value < Layers.Count)
                {
                    index = value;
                    this.OnPropertyChanged("Index");

                    if (isCanUndo == true)
                    {
                        //Undo：撤销
                        Undo undo = new Undo();
                        undo.IndexInstantiation(OldIndex);
                        App.UndoAdd(undo);

                        OldIndex = index;
                    }
                }
            }
        }

        int OldIndex;
        private void SetIndex(int i)//不产生撤销类的改变索引的方法，适用于撤销的操作
        {
            isCanUndo = false;//关闭撤销
            Index = i;
            isCanUndo = true;//开启撤销
        }


        //保存名称
        private string name = "Photo";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.OnPropertyChanged("Name");

            }
        }


        //选择工具（决定底栏跳转什么页面）
        private int tool;
        public int Tool
        {
            get { return tool; }
            set
            {
                tool = value;
                this.OnPropertyChanged("Tool");
            }
        }


        //Key：键盘
        private bool isctrl;
        public bool isCtrl
        {
            get { return isctrl; }
            set
            {
                isctrl = value;
                this.OnPropertyChanged("isCtrl");
            }
        }

        private bool isshift;
        public bool isShift
        {
            get { return isshift; }
            set
            {
                isshift = value;
                this.OnPropertyChanged("isShift");
            }
        }
 

        #endregion

        #region Binding：绑定


        //底栏可视
        private Visibility bottomVisibility = Visibility.Collapsed;
        public Visibility BottomVisibility
        {
            get { return bottomVisibility; }
            set
            {
                bottomVisibility = value;
                this.OnPropertyChanged("BottomVisibility");
            }
        }

        //Main：按钮可用
        private bool ismain;
        public bool isMain
        {
            get { return ismain; }
            set
            {
                ismain = value;
                this.OnPropertyChanged("isMain");
            }
        }


        //Cursor：光标可用
        private bool iscursor;
        public bool isCursor
        {
            get { return iscursor; }
            set
            {
                iscursor = value;
                 this.OnPropertyChanged("isCursor");
            }
        }

        //Line：网格线
        private bool isline;
        public bool isLine
        {
            get { return isline; }
            set
            {
                isline = value;
                this.Refresh++;//画布刷新
                this.OnPropertyChanged("isLine");
            }
        }
        //Mask：蓝色选区
        private bool ismask;
        public bool isMask
        {
            get { return ismask; }
            set
            {
                ismask = value;
                this.isReRender=true;//重新渲染
                this.Refresh++;//画布刷新
                this.OnPropertyChanged("isMask");
            }
        }
        //Ruler：标尺
        private bool isruler;
        public bool isRuler
        {
            get { return isruler; }
            set
            {
                isruler = value;
                this.isReRender = true;//重新渲染
                this.Refresh++;//画布刷新
                this.OnPropertyChanged("isRuler");
            }
        }


        //Undo：撤销
        private bool isundo;
        public bool isUndo
        {
            get { return isundo; }
            set
            {
                isundo = value;
                  this.OnPropertyChanged("isUndo");
            }
        }
        //Redo：重做
        private bool isredo;
        public bool isRedo
        {
            get { return isredo; }
            set
            {
                isredo = value; 
                this.OnPropertyChanged("isRedo");
            }
        }


        //Straw：临时取色
        private Visibility strawVisibility = Visibility.Visible;
        public Visibility StrawVisibility
        {
            get { return strawVisibility; }
            set
            {
                strawVisibility = value;
                this.OnPropertyChanged("StrawVisibility");
            }
        }

        //Second：标题栏文字
        private string text = string.Empty;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                this.OnPropertyChanged("Text");
            }
        }


        // 图层扁列表视图
        public bool isLowView = true;


        //应用屏幕宽度（用来判断手机平板电脑）
        public double ScreenWidth;
        public double ScreenHeight;


        //画布网格宽度
        public double GridWidth=1024;
        public double GridHeight = 1024;

        //提示
        private string tip;
        public string Tip
        {
            get { return tip; }
            set
            {
                tip = value;
                this.OnPropertyChanged("Tip");
            }
        }

        private Visibility tipVisibility = Visibility.Collapsed;
        public Visibility TipVisibility
        {
            get { return tipVisibility; }
            set
            {
                tipVisibility = value;
                this.OnPropertyChanged("TipVisibility");
            }
        }


        #endregion



        #region Virtual & Animated：虚拟 & 动画


        //画布控件
        public CanvasRenderTarget MainRenderTarget;//主要渲染目标

        //选区CanvasVirtualControl
        public CanvasRenderTarget MaskRenderTarget;  //套索渲染目标
        public Rect MaskRect;//存储选区的边界：App.Judge()

        //剪切板
        private bool isclipboard;
        public bool isClipboard//是否存在粘贴板
        {
            get { return isclipboard; }
            set
            {
                isclipboard = value;
                this.OnPropertyChanged("isClipboard");
            }
        }
        public CanvasRenderTarget Clipboard;//剪切板渲染目标

        //空画布
        public CanvasRenderTarget NullRenderTarget;//空白渲染目标

        //是否重新把图层层叠渲染
        public bool isReRender = false;
        //画布刷新（使用方法：Refresh++）
        private int refresh;
        public int Refresh
        {
            get { return refresh; }
            set
            {
                if (value > 1024) refresh = 0;
                else refresh = value;
                this.OnPropertyChanged("Refresh");
            }
        }




        public CanvasRenderTarget MaskAnimatedTarget;  //套索渲染目标

        //选区模式 ：主模式：0、加模式：1、减模式：2 、差模式：3
        private int maskMode;
        public int MaskMode
        {
            get
            {

                return maskMode;
            }
            set
            {
                maskMode = value;
                this.OnPropertyChanged("MaskMode");
            }
        }

        private bool isanimated;
        public bool isAnimated//是否有套索动画
        {
            get { return isanimated; }
            set
            {
                isanimated = value; 
                this.OnPropertyChanged("isAnimated");
            }
        }
        public bool isReStroke = false;//是否重新设置套索描边
        public bool isUpdate = true;//是否套索动画更新


        #endregion
 
        #region Second ：第二界面


        public CanvasBitmap PunchDisplacement;
        public CanvasBitmap PinchDisplacement;

        public ICanvasImage SecondCanvasImage;  //图片
        public CanvasVirtualBitmap SecondCanvasBitmap;  //位图
        public CanvasGeometry SecondCanvasGeometry;//杂项几何
        public CanvasRenderTarget SecondSourceRenderTarget;  //源图渲染目标
        public CanvasRenderTarget SecondTopRenderTarget;  //上渲染目标 
        public CanvasRenderTarget SecondBottomRenderTarget;  //下渲染目标
 

         #endregion



        #region  Concrol：控件宽高


        //控件宽高
        public int Width=1024;
        public int Height = 1024;

        //画布位移
        private double x;
        public double X
        {
            get { return x; }
            set
            {
               x = value;
                //  this.OnPropertyChanged("X");
            }
        }
        private double y;
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
              //  this.OnPropertyChanged("Y");
            }
        }

        //画布宽高
        private double canvasWidth = 1024;
        public double CanvasWidth
        {
            get { return canvasWidth; }
            set
            {
                canvasWidth = value;
                this.OnPropertyChanged("CanvasWidth");
            }
        }
        private double canvasHeight = 1024;
        public double CanvasHeight
        {
            get { return canvasHeight; }
            set
            {
                canvasHeight = value;
                this.OnPropertyChanged("CanvasHeight");
            }
        }


        //画布比例
        public double XS
        {
            get => CanvasWidth / Width;
        }
        public double YS
        {
            get => CanvasHeight / Height;
        }
        public float SX
        {
            get => Width / (float)CanvasWidth;
        }
        public float SY
        {
            get => Height / (float)CanvasHeight;
        }


        //屏幕层与源图层
        public Point ScreenToCanvas(Point p)
        {
            return new Point((p.X - X) / XS, (p.Y - Y) / YS);
        }
        public Vector2 ScreenToCanvas(Vector2 p)
        {
            return new Vector2((float)((p.X - X) / XS),(float)( (p.Y - Y) / YS));
        }
        public Rect ScreenToCanvas(Rect r)
        {
            return new Rect(
              (r.X - X) / XS,
               (r.Y - Y) / YS,
                r.Width / XS,
                r.Height / YS
                );
        }

        public Point CanvasToScreen(Point p)
        {
            return new Point(p.X * XS + X, p.Y * YS + Y);
        }
        public Vector2 CanvasToScreen(Vector2 p)
        {
            return new Vector2((float)(p.X * XS + X), (float)(p.Y * YS + Y));
        }
        public Rect CanvasToScreen(Rect r)
        {
            return new Rect(
                r.X * XS + X,
                r.Y * YS + Y, 
                r.Width * XS, 
                r.Height * YS
                );
        } 

        //屏幕层与渲染层
        public Point ScreenToImage(Point p)
        {
            return new Point((p.X - X), (p.Y - Y));
        }
        public Point ImageToScreen(Point p)
        {
            return new Point(p.X + X, p.Y * +Y);
        }

        //对于画布的变换
        public Matrix3x2 Matrix 
        {
            get => new Matrix3x2((float)XS, 0, 0, (float)YS, (float)X, (float)Y);
        }

        #endregion

        #region InkCanvas：墨水画布


        public float InkWidth = 12;
        public float InkHeight = 12;
        public float InkAngle = 0;

        public InkSynchronizer 墨水同步器;
        public InkPresenterRuler 尺子;
        public InkPresenterProtractor 量角器;

        public void InkSet()
        {
            InkDrawingAttributes attribute = InkCanvas.InkPresenter.CopyDefaultDrawingAttributes();

            attribute.FitToCurve = true;//贝塞尔
            attribute.Color = this.Inkcolor;
            attribute.PenTip = PenTipShape.Circle;//笔尖类型设置
            attribute.PenTipTransform = Matrix3x2.CreateRotation(this.InkAngle);////笔尖形状矩阵
            attribute.Size = new Size(this.InkWidth * this.XS, this.InkHeight * this.YS);//画笔粗细

            this.InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(attribute);
        }


        //墨水描边 
        public float InkStrokeWidth = 5;
        public CanvasStrokeStyle InkStrokeStyle = new CanvasStrokeStyle();


        #endregion



        #region Color：颜色


        //颜色
        private Color color = Colors.Black;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                this.OnPropertyChanged("Color");
            }
        }


        //Pen：钢笔
        private Color Pencolor = Color.FromArgb(255, 54, 135, 230);
        public Color PenColor
        {
            get { return Pencolor; }
            set
            {
                Pencolor = value;
                this.OnPropertyChanged("PenColor");
            }
        }

        //Ink：墨水
        private Color Inkcolor = Color.FromArgb(255, 54, 135, 230);
        public Color InkColor
        {
            get { return Inkcolor; }
            set
            {
                Inkcolor = value;
                this.OnPropertyChanged("InkColor");
            }
        }


        //Effecr：特效
        //阴影
        private Color Shadowcolor = Colors.Black;
        public Color ShadowColor
        {
            get { return Shadowcolor; }
            set
            {
                Shadowcolor = value;
                this.OnPropertyChanged("ShadowColor");
            }
        }
        //消除颜色
        private Color ChromaKeycolor = Colors.Gray;
        public Color ChromaKeyColor
        {
            get { return ChromaKeycolor; }
            set
            {
                ChromaKeycolor = value;
                this.OnPropertyChanged("ChromaKeyColor");
            }
        }


        //色彩
        private Color Tintcolor = Color.FromArgb(255, 0, 165, 255);
        public Color TintColor
        {
            get { return Tintcolor; }
            set
            {
                Tintcolor = value;
                this.OnPropertyChanged("TintColor");
            }
        }
 

        //装饰图案
        private Color Vignettecolor = Color.FromArgb(0, 0, 0, 0);
        public Color VignetteColor
        {
            get { return Vignettecolor; }
            set
            {
                Vignettecolor = value;
                this.OnPropertyChanged("VignetteColor");
            }
        }

        //Other：其他
        //Text：文字
        private Color Textcolor  = Colors.Black;
        public Color TextColor
        {
            get { return Textcolor; }
            set
            {
                Textcolor = value;
                this.OnPropertyChanged("TextColor");
            }
        }

        //Grids：网格线
        private Color Gridscolor = Colors.Black;
        public Color GridsColor
        {
            get { return Gridscolor; }
            set
            {
                Gridscolor = value;
                this.OnPropertyChanged("GridsColor");
            }
        }


        //Fill：填充颜色
        private Color Fillcolor = Colors.Black;
        public Color FillColor
        {
            get { return Fillcolor; }
            set
            {
                Fillcolor = value;
                this.OnPropertyChanged("FillColor");
            }
        }


        //StrokeColor：描边颜色
        private Color Geometrycolor = Color.FromArgb(255, 66, 141, 255);//浅亮蓝色
        public Color GeometryColor
        {
            get { return Geometrycolor; }
            set
            {
                Geometrycolor = value;
                this.OnPropertyChanged("GeometryColor");
            }
        }
        private Color Strokecolor = Color.FromArgb(255, 0, 182, 255);//浅亮蓝色
        public Color StrokeColor
        {
            get { return Strokecolor; }
            set
            {
                Strokecolor = value;
                this.OnPropertyChanged("StrokeColor");
            }
        }



        #endregion

        #region  Theme：主题


        //亚克力
        private ElementTheme theme;
        public ElementTheme Theme
        {
            get { return theme; }
            set
            {
                theme = value;
                this.OnPropertyChanged("Theme");
            }
        }
        //亚克力
        private AcrylicBrush acrylicBrush = new AcrylicBrush();
        public AcrylicBrush AcrylicBrush
        {
            get { return acrylicBrush; }
            set
            {
                acrylicBrush = value;
                this.OnPropertyChanged("AcrylicBrush");
            }
        }
        //透明颜色
        private SolidColorBrush opacityBrush = new SolidColorBrush();
        public SolidColorBrush OpacityBrush
        {
            get { return opacityBrush; }
            set
            {
                opacityBrush = value;
                this.OnPropertyChanged("OpacityBrush");
            }
        }




        //标题颜色
        private SolidColorBrush titleColor = new SolidColorBrush();
        public SolidColorBrush TitleColor
        {
            get { return titleColor; }
            set
            {
                titleColor = value;
                this.OnPropertyChanged("TitleColor");
            }
        }
        //顶栏颜色
        private SolidColorBrush buttonColor = new SolidColorBrush();
        public SolidColorBrush ButtonColor
        {
            get { return buttonColor; }
            set
            {
                buttonColor = value;
                this.OnPropertyChanged("ButtonColor");
            }
        }
        



        //背景颜色
        private SolidColorBrush background = new SolidColorBrush();
        public SolidColorBrush Background
        {
            get { return background; }
            set
            {
                background = value;
                this.OnPropertyChanged("Background");
            }
        }
        //面板颜色
        private SolidColorBrush panelColor = new SolidColorBrush();
        public SolidColorBrush PanelColor
        {
            get { return panelColor; }
            set
            {
                panelColor = value;
                this.OnPropertyChanged("PanelColor");
            }
        }
        



        //浅线颜色
        private SolidColorBrush lineColor = new SolidColorBrush();
        public SolidColorBrush LineColor
        {
            get { return lineColor; }
            set
            {
                lineColor = value;
                this.OnPropertyChanged("LineColor");
            }
        }
        //文本背景
        private SolidColorBrush textBackColor = new SolidColorBrush();
        public SolidColorBrush TextBackColor
        {
            get { return textBackColor; }
            set
            {
                textBackColor = value;
                this.OnPropertyChanged("TextBackColor");
            }
        }
        



        //字体颜色
        private SolidColorBrush foreground = new SolidColorBrush();
        public SolidColorBrush Foreground
        {
            get { return foreground; }
            set
            {
                foreground = value;
                this.OnPropertyChanged("Foreground");
            }
        }
        //高亮字体颜色
        private SolidColorBrush signForeground = new SolidColorBrush();
        public SolidColorBrush SignForeground
        {
            get { return signForeground; }
            set
            {
                signForeground = value;
                this.OnPropertyChanged("SignForeground");
            }
        }


        #endregion


        public Model()  { }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
