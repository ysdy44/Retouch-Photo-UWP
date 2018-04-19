using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;

using System.Numerics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using 修图.Model;
using Windows.System;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;
using Windows.ApplicationModel.Resources;

namespace 修图
{
    sealed partial class App : Application
    { 
        //明暗主题：实例化
        public static Model.Model Model = new Model.Model();
        public static Model.Setting Setting = new Model.Setting();
      
        //Undo：撤销
        public static void UndoAdd(Undo undo)//撤销添加
        {
            ////如果当前撤销索引小于总撤销数目，即并不在最后一步，就移除以后的项
            int Count = App.Model.Undos.Count - 1;
            if (App.Model.UndoIndex < Count)
            {
                for (int i = Count; i > 0; i--)
                {
                    App.Model.Undos[i].Dispose();//释放资源
                    App.Model.Undos.RemoveAt(i);//移除项
                }
            }
            App.Model.Undos.Add(undo);
            App.Model.UndoIndex = App.Model.Undos.Count-1;//更新撤销索引    

            App.Model.isUndo = true;//撤销可用
            App.Model.isRedo = false;//重做可用
        }

        #region Formatting&Initialize：格式化与初始化


        //格式化（先设定宽高）
        public static void Formatting(Layer l,bool isAnimated=true)
        {
            //画布位置居中，并与左右边界保持1/8的边距
            App.Model.CanvasWidth = App.Model.GridWidth / 8 * 7;
            App.Model.CanvasHeight = App.Model.CanvasWidth / App.Model.Width * App.Model.Height;
            App.Model.X = (App.Model.GridWidth - App.Model.CanvasWidth) / 2;
            App.Model.Y = (App.Model.GridHeight - App.Model.CanvasHeight) / 2;

            //清空
            foreach (var Layer in App.Model.Layers)
            {
                Layer.CanvasRenderTarget.Dispose();
            }
            App.Model.Layers.Clear();
            App.Model.Layers.Add(l);

              //初始化
            App.Initialize(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            if (isAnimated) App.Model.MaskAnimatedTarget = new CanvasRenderTarget(App.Model.AnimatedControl, App.Model.Width, App.Model.Height);

            App.Model.isReRender = true;//重新渲染
            App.Model.isReStroke = true;//重新描边
            App.Model.Refresh++;//画布刷新 
            
            //更新类库
            //var Model = new 修图.Model.Model();       
            App.Model.Undos.Clear();
            App.Model.UndoIndex = -1;
            App.Model.isUndo = false;
            App.Model.isRedo = false;
            App.Model.Tool = 0;
            App.Model.Index = 0;
            App.Model.Refresh = 0;
            App.Setting = new 修图.Model.Setting();
        }
        public static void Formatting( bool isAnimated = true)
        {
            //画布位置居中，并与左右边界保持1/8的边距
            App.Model.CanvasWidth = App.Model.GridWidth / 8 * 7;
            App.Model.CanvasHeight = App.Model.CanvasWidth / App.Model.Width * App.Model.Height;
            App.Model.X = (App.Model.GridWidth - App.Model.CanvasWidth) / 2;
            App.Model.Y = (App.Model.GridHeight - App.Model.CanvasHeight) / 2;

            //清空
            foreach (var Layer in App.Model.Layers)
            {
                Layer.CanvasRenderTarget.Dispose();
            }
            App.Model.Layers.Clear();

            //初始化
            App.Initialize(App.Model.VirtualControl,App.Model .Width, App.Model.Height);
            if (isAnimated) App.Model.MaskAnimatedTarget = new CanvasRenderTarget(App.Model.AnimatedControl, App.Model.Width, App.Model.Height);

            App.Model.isReRender = true;//重新渲染
            App.Model.isReStroke = true;//重新描边
            App.Model.Refresh++;//画布刷新

            //更新类库
            App.Model.Undos.Clear();
            App.Model.UndoIndex = -1;
            App.Model.isUndo = false;
            App.Model.isRedo = false;
            App.Model.Tool = 0;
            App.Model.Index = 0;
            App.Model.Refresh = 0;
            App.Setting = new 修图.Model.Setting();
        }


        //初始化
         public static void Initialize(CanvasVirtualControl sender, int width, int height)
        {
            App.GrayWhiteGrid = App.GrayWhite(sender, width, height);//灰白网格
            //主渲染目标
            App.Model.MainRenderTarget = new CanvasRenderTarget(sender, width, height, 96);
            //选区渲染目标
            App.Model.MaskRenderTarget = new CanvasRenderTarget(sender, width, height);
            
            //空白渲染目标
            App.Model.NullRenderTarget = new CanvasRenderTarget(sender, width, height);

            //第二页渲染目标（上下）
            App.Model.SecondSourceRenderTarget = new CanvasRenderTarget(sender, width, height);
            App.Model.SecondTopRenderTarget = new CanvasRenderTarget(sender, width, height);
            App.Model.SecondBottomRenderTarget = new CanvasRenderTarget(sender, width, height);
            
            App.Model.isAnimated = false;
        }
        //初始化选区渲染目标
        public static void InitializeMask()
        {
            using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(App.Model.MainRenderTarget);
            }
        }
        //初始化特效渲染目标
        public static void InitializeEffect()
        {
            //特效渲染目标 （上中下）（如果选区存在：选区选中的图像 ；否则：全部图像）
            using (CanvasDrawingSession ds = App.Model.SecondSourceRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                //获得当前图层的渲染目标
                ds.DrawImage(App.Model.Layers[App.Model.Index].CanvasRenderTarget);

                //扣取选区的区域：如果有选区
                if (App.Model.isAnimated==true)
               ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.HighQualityCubic, CanvasComposite.DestinationIn);
            }



            //下特效渲染目标 （从灰白网格到当前图层的下一层）
            using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(App.GrayWhiteGrid);

                ICanvasImage ci = App.Model.NullRenderTarget;//由下向上渲染的图片接口

                for (int i = App.Model.Layers.Count - 1; i > App.Model.Index; i--)//自下而上渲染
                {
                    ci = App.Render(App.Model.Layers[i], ci);//渲染
                }
                ds.DrawImage(ci);



                //获得当前图层的扣掉图形的渲染目标
                if (App.Model.isAnimated == true)
                {
                    //复制源图 
                    CanvasCommandList ccl = new CanvasCommandList(App.Model.VirtualControl);
                    //渲染目标的画布布尔运算
                    using (CanvasDrawingSession dds = ccl.CreateDrawingSession())
                    {
                        dds.DrawImage(App.Model.CurrentRenderTarget);

                        dds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationOut);
                    }
                    ds.DrawImage(ccl);
                }
            }



            //上特效渲染目标 （从当前图层的的上一层到顶层）
            using (CanvasDrawingSession ds = App.Model.SecondTopRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                //当前图层索引不在0，即并不在第一层
                ICanvasImage ci = App.Model.NullRenderTarget;

                //当前图层索引不在1，即并不在第二层
                if (App.Model.Index >0)
                {
                    for (int i = App.Model.Index - 1; i >= 0; i--)//自下而上渲染
                    {
                        ci = App.Render(App.Model.Layers[i], ci);//渲染
                    }
                }
                ds.DrawImage(ci);
            }
         }
        //初始化杂项渲染目标
        public static void InitializeOther()
        {
            //下杂项渲染目标 （从灰白网格到当前图层）
            using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            {
                ICanvasImage ci = App.GrayWhiteGrid;//由下向上渲染的图片接口

                for (int i = App.Model.Layers.Count - 1; i >= App.Model.Index; i--)//自下而上渲染
                {
                    ci = App.Render(App.Model.Layers[i], ci);//渲染
                }
                ds.DrawImage(ci);
            }

            //上杂项渲染目标 （从当前图层的的上一层到顶层）
            using (CanvasDrawingSession ds = App.Model.SecondTopRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                ICanvasImage ci = App.Model.NullRenderTarget;

                //当前图层索引不在0，即并不在第一层
                if (App.Model.Index > 0)
                {
                    for (int i = 0; i < App.Model.Index; i++)//自上而下渲染
                    {
                        ci = App.Render(App.Model.Layers[i], ci);//渲染
                    }
                    ds.DrawImage(ci);
                }
            }
        }
        //初始化裁切渲染目标
        public static void InitializeCrop()
        {
            using (CanvasDrawingSession ds = App.Model.SecondTopRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                ICanvasImage ci = App.Model.NullRenderTarget;
                for (int i = App.Model.Layers.Count - 1; i >= 0; i--)//自下而上渲染
                {
                    ci = App.Render(App.Model.Layers[i], ci);//渲染
                }
                ds.DrawImage(ci);
            }
            修图.BarPage.OtherPage.Crop.Render();
         }
        //初始化液化渲染目标
        public static void InitializeLiquify()
        {
            //特效渲染目标 
            using (CanvasDrawingSession ds = App.Model.SecondSourceRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                //获得当前图层的渲染目标
                ds.DrawImage(App.Model.CurrentRenderTarget);
            }

             //下层清空为蓝色
            using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            {
                ds.Clear(App.Setting.LiquifyColor);
              }
        }


        #endregion

        #region Render：渲染


        //灰白网格底图
        public static ICanvasImage GrayWhiteGrid;
     
        //创建灰白网格底图
        public static ICanvasImage GrayWhite(ICanvasResourceCreator crc, int width, int height)
        {
            return new CropEffect//裁切成画布大小
            {
                Source = new DpiCompensationEffect//根据DPI适配
                {
                    Source = new ScaleEffect//缩放
                    {
                        Source = new BorderEffect//无限延伸图片
                        {
                             Source = CanvasBitmap.CreateFromColors(crc, new
                            Color[]  //从数组创建2x2图片
                            {
                                Color.FromArgb(255, 233, 233, 233),Colors.White,
                                Colors.White,Color.FromArgb(255, 233, 233, 233),
                            }, 2, 2),
                            ExtendX = CanvasEdgeBehavior.Wrap,
                            ExtendY = CanvasEdgeBehavior.Wrap
                        },
                        Scale = new Vector2(8, 8),
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor
                    }
                },
                 SourceRectangle = new Rect(0, 0, App.Model.Width, App.Model.Height),
            };
         }


        //渲染：输入上层、下底，输出新的下层
        public static ICanvasImage Render(Layer l, ICanvasImage ci)
        {
            if (l.Visual == true )//图层可视且透明度大于零
            {
                if (l.Opacity == 100)//无透明度渲染
                {
                    //图层无混合模式
                    if (l.BlendIndex == 0) ci = new CompositeEffect
                    {
                        Sources = { ci, l.CanvasRenderTarget, }
                    };//结合接口
                    //图层有混合模式
                    else ci = new BlendEffect() { Background = l.CanvasRenderTarget, Foreground = ci, Mode = l.BlendMode, };//混合接口
                }
                else if (l.Opacity == 0)//透明度渲染
                {
                    return ci;
                }
                else// if (l.Opacity < 100)//有透明度渲染
                {
                    ICanvasImage oci = new OpacityEffect { Source = l.CanvasRenderTarget,  Opacity = (float)(l.Opacity / 100) };//透明度接口

                    //图层无混合模式
                    if (l.BlendIndex == 0) ci = new CompositeEffect { Sources = { ci, oci } };//结合接口
                                                                                              //图层有混合模式
                    else ci = new BlendEffect() { Background = oci, Foreground = ci, Mode = l.BlendMode, };//混合接口
                }
            }
            return ci;
        }
        public static ICanvasImage RenderTransform(Layer l, ICanvasImage ci)
        {
            if (l.Visual == true && l.Opacity != 0)//图层可视且透明度大于零
            {
                if (l.Opacity == 100)//无透明度渲染
                {
                    //图层无混合模式
                    if (l.BlendIndex == 0) ci = new CompositeEffect
                    {
                        Sources = { ci, GetTransform(l.CanvasRenderTarget, l.Vect) }
                    };//结合接口
                    //图层有混合模式
                    else ci = new BlendEffect() { Background = GetTransform(l.CanvasRenderTarget, l.Vect), Foreground = ci, Mode = l.BlendMode, };//混合接口
                }
                else if (l.Opacity == 0)//透明度渲染
                {
                    return ci;
                }
                else// if (l.Opacity < 100)//有透明度渲染
                {
                    ICanvasImage oci = new OpacityEffect { Source = GetTransform(l.CanvasRenderTarget, l.Vect), Opacity = (float)(l.Opacity / 100) };//透明度接口

                    //图层无混合模式
                    if (l.BlendIndex == 0) ci = new CompositeEffect { Sources = { ci, oci } };//结合接口
                                                                                              //图层有混合模式
                    else ci = new BlendEffect() { Background = oci, Foreground = ci, Mode = l.BlendMode, };//混合接口
                }
            }
            return ci;
        }
     
        //渲染：输入上层、上层的渲染目标、下底，输出新的下层
        public static ICanvasImage Render(Layer l, ICanvasImage CanvasImage, ICanvasImage ci)
        {
            if (l.Visual == true && l.Opacity != 0)//图层可视且透明度大于零
            {
                if (l.Opacity == 100)//无透明度渲染
                {
                    //图层无混合模式
                    if (l.BlendIndex == 0) ci = new CompositeEffect { Sources = { ci, CanvasImage } };//结合接口
                    //图层有混合模式
                    else ci = new BlendEffect() { Background = CanvasImage, Foreground = ci, Mode = l.BlendMode, };//混合接口
                }
                else if (l.Opacity == 0)//透明度渲染
                {
                    return ci;
                }
                else //if (l.Opacity < 100)//有透明度渲染
                {
                    ICanvasImage oci = new OpacityEffect { Source = CanvasImage, Opacity = (float)(l.Opacity / 100) };//透明度接口
                    //图层无混合模式
                    if (l.BlendIndex == 0) ci = new CompositeEffect { Sources = { ci, oci } };//结合接口
                    //图层有混合模式
                    else ci = new BlendEffect() { Background = oci, Foreground = ci, Mode = l.BlendMode, };//混合接口
                }
            }
            return ci;
        }
         public static ICanvasImage GetTransform(ICanvasImage CanvasImage, Vector2 m)
        {
            return new Transform2DEffect
            {
                Source = CanvasImage,
                TransformMatrix =Matrix3x2.CreateTranslation(m),
                InterpolationMode= CanvasImageInterpolation.NearestNeighbor
            };
        }
      
        
        #endregion

        #region Mask&Bound：选区与边界


        //输入最新的画布几何、动画几何、选区模式，改变主选区与套索几何（当All,All,3时为不超过画布边界）
        public static void Mask(CanvasGeometry CanvasGeometry, CanvasGeometry AnimatedGeometry, int MaskMode=0,bool isClear=false)
        {
            CanvasComposite compositeMode = CanvasComposite.SourceOver;
            switch (MaskMode)
            {
                //主模式：SourceOver
                case 0: compositeMode = CanvasComposite.SourceOver; break;
                //加模式：SourceOver
                case 1: compositeMode = CanvasComposite.SourceOver; break;
                //减模式：DestinationOut
                case 2: compositeMode = CanvasComposite.DestinationOut; break;
                //差模式：DestinationIn
                case 3: compositeMode = CanvasComposite.DestinationIn; break;
                //负模式：DestinationIn
                case 4: compositeMode = CanvasComposite.Xor; break;
                default: break;
            }



            //画布
            CanvasCommandList cl = new CanvasCommandList(App.Model.VirtualControl);//新图片（列表命令）
            using (CanvasDrawingSession ds = cl.CreateDrawingSession())//新图片上画几何
            {
                 ds.FillGeometry(CanvasGeometry, App.Setting.MaskColor);
            }
            using (CanvasDrawingSession ds = App.Model.MaskRenderTarget.CreateDrawingSession())//旧图片上画旧图片（并布尔运算）
            {
                if (isClear == true|| MaskMode==0) ds.Clear(Colors.Transparent);
                ds.DrawImage(cl, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
            }



            //动画
            CanvasCommandList acl = new CanvasCommandList(App.Model.AnimatedControl);//新图片（列表命令）
            using (CanvasDrawingSession ds = acl.CreateDrawingSession())//新图片上画几何
            {
                ds.FillGeometry(AnimatedGeometry, App.Setting.MaskColor);
            }    
            using (CanvasDrawingSession ds = App.Model.MaskAnimatedTarget.CreateDrawingSession())//旧图片上画旧图片（并布尔运算）
            {
                if (isClear == true || MaskMode == 0) ds.Clear(Colors.Transparent);
                ds.DrawImage(acl, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
            }



            if (App.Model.isMask == true)//蓝色选区
            {
                App.Model.isReRender = true;
                App.Model.Refresh++;
            }
        }
        public static void Mask(ICanvasImage VirtualImage, ICanvasImage AnimatedImage, int MaskMode = 0, bool isClear = false)
        {
            CanvasComposite compositeMode = CanvasComposite.SourceOver;
            switch (MaskMode)
            {
                //主模式：SourceOver
                case 0: compositeMode = CanvasComposite.SourceOver; break;
                //加模式：SourceOver
                case 1: compositeMode = CanvasComposite.SourceOver; break;
                //减模式：DestinationOut
                case 2: compositeMode = CanvasComposite.DestinationOut; break;
                //差模式：DestinationIn
                case 3: compositeMode = CanvasComposite.DestinationIn; break;
                //负模式：DestinationIn
                case 4: compositeMode = CanvasComposite.Xor; break;
                default: break;
            }



            //画布
            CanvasCommandList cl = new CanvasCommandList(App.Model.VirtualControl);//新图片（列表命令）
            using (CanvasDrawingSession ds = cl.CreateDrawingSession())//新图片上画几何
            {
                ds.Clear(App.Setting.MaskColor);
                 ds.DrawImage(VirtualImage, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationIn);
             }
            using (CanvasDrawingSession ds = App.Model.MaskRenderTarget.CreateDrawingSession())//旧图片上画旧图片（并布尔运算）
            {
                if (isClear == true || MaskMode == 0) ds.Clear(Colors.Transparent);
                ds.DrawImage(cl, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
            }



            //动画
            CanvasCommandList acl = new CanvasCommandList(App.Model.AnimatedControl);//新图片（列表命令）
            using (CanvasDrawingSession ds = acl.CreateDrawingSession())//新图片上画几何
            {
                     ds.Clear(App.Setting.MaskColor);
                    ds.DrawImage(AnimatedImage, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, CanvasComposite.DestinationIn);
             }
            using (CanvasDrawingSession ds = App.Model.MaskAnimatedTarget.CreateDrawingSession())//旧图片上画旧图片（并布尔运算）
            {
                if (isClear == true || MaskMode == 0) ds.Clear(Colors.Transparent);
                ds.DrawImage(acl, 0, 0, App.Model.MaskAnimatedTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
            }



            if (App.Model.isMask == true)//蓝色选区
            {
                App.Model.isReRender = true;
                App.Model.Refresh++;
            }
        }

        public static void MaskClear(Color color)
        {
            using (CanvasDrawingSession ds = App.Model.MaskRenderTarget.CreateDrawingSession())//旧图片上画旧图片（并布尔运算）
            {
                ds.Clear(color);
            }

            using (CanvasDrawingSession ds = App.Model.MaskAnimatedTarget.CreateDrawingSession())//旧图片上画旧图片（并布尔运算）
            {
                ds.Clear(color);
             }
        }
        public static void MaskClear()
        {
            using (CanvasDrawingSession ds = App.Model.MaskRenderTarget.CreateDrawingSession())//旧图片上画旧图片（并布尔运算）
            {
                ds.Clear(Colors.Transparent);
            } 

            using (CanvasDrawingSession ds = App.Model.MaskAnimatedTarget.CreateDrawingSession())//旧图片上画旧图片（并布尔运算）
            {
                ds.Clear(Colors.Transparent);
            }
        }

        
        //得到选区的边界 
        public static Rect GetBounds(Color[] co, int W, int H)
        { 
            //左上右下
            int left = W;
            int top = H;
            int right = 0;
            int bottom = 0;

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    if (co[y * W + x].A !=0)
                    {
                        if (left > x) left = x;
                        if (top > y) top = y;

                        if (right < x) right = x;
                        if (bottom < y) bottom = y;
                    }
                }
            }

            if (right > left && bottom > top) return new Rect(left, top, right - left, bottom - top);
            else return new Rect(0, 0, W, H);
        }
        public static Rect GetBounds(Color[] co, BitmapSize size)
        {
            int W = (int)size.Width;
            int H = (int)size.Height;

            //左上右下
            int left = W;
            int top = H;
            int right = 0;
            int bottom = 0;

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    if (co[y * W + x].A != 0)
                    {
                        if (left > x) left = x;
                        if (top > y) top = y;

                        if (right < x) right = x;
                        if (bottom < y) bottom = y;
                    }
                }
            }

            if (right > left && bottom > top) return new Rect(left, top, right - left, bottom - top);
            else return new Rect(0, 0, W, H);
        }

        //得到当前图层的边界 
        public static Rect GetCurrentBounds()
        {
            var co = App.Model.CurrentRenderTarget.GetPixelColors();
            return GetBounds(co, App.Model.Width, App.Model.Height);
        }
     
        //判断选区是否存在并改变选区边界
        public static void Judge()
        {
            var co = App.Model.MaskRenderTarget.GetPixelColors();

            //左上右下
            int left = App.Model.Width;
            int top = App.Model.Height;
            int right = 0;
            int bottom = 0;

            for (int y = 0; y < App.Model.Height; y++)
            {
                for (int x = 0; x < App.Model.Width; x++)
                {
                    if (co[y * App.Model.Width + x].A > 10)
                    {
                        if (left > x) left = x;
                        if (top > y) top = y;

                        if (right < x) right = x;
                        if (bottom < y) bottom = y;
                    }
                }
            }

            if (right > left && bottom > top)
            {
                App.Model.MaskRect = new Rect(left, top, right - left, bottom - top);
                App.Model.isAnimated = true;
            }
            else
            {
                App.Model.MaskRect = new Rect(0, 0, App.Model.Width, App.Model.Height);
                App.Model.isAnimated = false;
            }
        }
         public static void Judge(Rect rect)
        {
            if (rect.X + rect.Width >0&&
                rect.Y + rect.Height > 0 &&
                rect.X   < App.Model.Height &&
                rect.Y  < App.Model.Width )
            {
                App.Model.MaskRect = rect;
                App.Model.isAnimated = true;
            }
            else
            {
                App.Model.MaskRect = new Rect(0, 0, App.Model.Width, App.Model.Height);
                App.Model.isAnimated = false;
            }
        }
        public static void Judge(double X, double Y, double W, double H)
        {
            if (X + W> 0 &&
                Y + W> 0 &&
                X < App.Model.Height &&
                Y < App.Model.Width)
            {
                App.Model.MaskRect = new Rect(X,Y,W,H);
                App.Model.isAnimated = true;
            }
            else
            {
                App.Model.MaskRect = new Rect(0, 0, App.Model.Width, App.Model.Height);
                App.Model.isAnimated = false;
            }
        }


        #endregion

        #region Main：全局


        //数据容器
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        //Resource：资源钩子
        public static ResourceLoader resourceLoader = new ResourceLoader();

        //Tip：输入文字，弹出提示框
        public async static void Tip(string s = "👁‍🗨当前图层不可视")
        {
            App.Model.Tip = s;
            App.Model.TipVisibility = Visibility.Visible;
            await Task.Delay(1000);
            App.Model.TipVisibility = Visibility.Collapsed;
        }

        //File：文件关联
    public static   IStorageItem StorageItem;
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            StorageItem = args.Files.First();

            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                frame = new Frame();
                Window.Current.Content = frame;
            }

          //  if (App.Model.Layers.Count > 0)// frame.Navigate(typeof(DrawPage) );//IStorageItem
          //  {
         //       Pictures((StorageFile)StorageItem);
         //   }else
             frame.Navigate(typeof(MainPage));

            Window.Current.Activate();
        }



        private async void Pictures(StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                App.Model.Name = file.DisplayName;//Name：名称

                //图片
                App.Model.SecondCanvasBitmap = await CanvasVirtualBitmap.LoadAsync(App.Model.VirtualControl, stream);
                //宽高
                App.Model.Width = (int)App.Model.SecondCanvasBitmap.SizeInPixels.Width;
                App.Model.Height = (int)App.Model.SecondCanvasBitmap.SizeInPixels.Height;
                if (App.Model.Width > 16384) App.Model.Width = 16384;
                if (App.Model.Height > 16384) App.Model.Height = 16384;

                MainCenter(App.Model.SecondCanvasBitmap);
                App.Model.StartVisibility = Visibility.Collapsed;
            }
        }

        private async void MainCenter(CanvasVirtualBitmap cb)//Main：Center为图片
        {
            //新建层
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (var ds = crt.CreateDrawingSession())
            {
                ds.DrawImage(cb);
            }
            Layer l = new Layer { CanvasRenderTarget = crt };
            if (App.Model.isLowView) l.LowView();
            else l.SquareView();
            l.SetWriteableBitmap(App.Model.VirtualControl);

            //格式化 
            App.Formatting(l);
        }
        #endregion


        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

        }
         
        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>ss
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
