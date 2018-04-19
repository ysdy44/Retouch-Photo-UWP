using System.Numerics;
using System.ComponentModel;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace 修图.Model 
{
    public class Layer :INotifyPropertyChanged
    {
        //图层名称
        private string name = App.resourceLoader.GetString("/Layer/Name_");
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.OnPropertyChanged("Name");
            }
        }



        //图层可视/不可
        private bool visual=true;
        public bool Visual
        {
            get { return visual; }
            set
            {
                visual = value;
                this.OnPropertyChanged("Visual");

                if (App.Model.isCanUndo == true)
                {
                    //Undo：撤销
                    Undo undo = new Undo();
                    int Index = App.Model.Layers.IndexOf(this);
                    undo.VisualInstantiation(Index,OldVisual);
                     App.UndoAdd(undo);

                    OldVisual = Visual;
                }
            }
        }

        //Undo：撤销
        bool OldVisual;
        public void SetVisual(bool v)//不产生撤销类的改变可视的方法，适用于撤销的操作
        {
            App.Model.isCanUndo = false;//关闭撤销
            this.Visual = v;
            App.Model.isCanUndo = true;//开启撤销
        }



        //图层不透明度（0~100）
        private double opacity=100;
        public double Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;
                this.OnPropertyChanged("Opacity");
            }
        }

        public double OldOpacity=100;  //Undo：撤销
        public void UndoOpacity(int i)//撤销的方法
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.OpacityInstantiation(i, OldOpacity);
            App.UndoAdd(undo);

            OldOpacity = this.Opacity;
        }


        //图层位图
        public CanvasRenderTarget CanvasRenderTarget {  get; set;  }
   
        
        #region Blend：混合


        //图层混合
        private int blendIndex;
        public int BlendIndex
        {
            get { return blendIndex; }
            set
            {
                blendIndex = value;
                this.OnPropertyChanged("BlendIndex");

                if (App.Model.isCanUndo == true)
                {
                    //Undo：撤销
                    Undo undo = new Undo();
                    int Index = App.Model.Layers.IndexOf(this);
                    undo.BlendInstantiation(Index, OldBlendIndex);
                    App.UndoAdd(undo);

                    OldBlendIndex = BlendIndex;
                }
            }
        }

        //Undo：撤销
        int OldBlendIndex;
        public void SetBlendIndex(int b)//不产生撤销类的改变可视的方法，适用于撤销的操作
        {
            App.Model.isCanUndo = false;//关闭撤销

            this.BlendIndex = b;

            App.Model.isCanUndo = true;//开启撤销
        }

        public BlendEffectMode BlendMode
        {
            get
            {
                if (this.BlendIndex == 1) return BlendEffectMode.Multiply;
                else if (this.BlendIndex == 2) return BlendEffectMode.Screen;
                else if (this.blendIndex == 3) return BlendEffectMode.Dissolve;

                else if (this.blendIndex == 4) return BlendEffectMode.Darken;
                else if (this.blendIndex == 5) return BlendEffectMode.Lighten;
                else if (this.blendIndex == 6) return BlendEffectMode.DarkerColor;
                else if (this.blendIndex == 7) return BlendEffectMode.LighterColor;

                else if (this.blendIndex == 8) return BlendEffectMode.ColorBurn;
                else if (this.blendIndex == 9) return BlendEffectMode.ColorDodge;
                else if (this.blendIndex == 10) return BlendEffectMode.LinearBurn;
                else if (this.blendIndex == 11) return BlendEffectMode.LinearDodge;

                else if (this.blendIndex == 12) return BlendEffectMode.Overlay;
                else if (this.blendIndex == 13) return BlendEffectMode.SoftLight;
                else if (this.blendIndex == 14) return BlendEffectMode.HardLight;
                else if (this.blendIndex == 15) return BlendEffectMode.VividLight;
                else if (this.blendIndex == 16) return BlendEffectMode.LinearLight;
                else if (this.blendIndex ==  17) return BlendEffectMode.PinLight;

                else if (this.blendIndex == 18) return BlendEffectMode.HardMix;
                else if (this.blendIndex == 19) return BlendEffectMode.Difference;
                else if (this.blendIndex == 20) return BlendEffectMode.Exclusion;

                else if (blendIndex == 21) return BlendEffectMode.Hue;
                else if (blendIndex == 22) return BlendEffectMode.Saturation;
                else if (blendIndex == 23) return BlendEffectMode.Color;

                else if (blendIndex == 24) return BlendEffectMode.Luminosity;
                else if (blendIndex == 25) return BlendEffectMode.Subtract;
                else// if (blendIndex ==  26)
                    return BlendEffectMode.Division;
            }
        }

        
        #endregion

        

        #region Matrix：矩阵变换


        public bool isSelected = false;//光标中是否被选中

        //位移
        public Vector2 Vect;
        public Rect CanvasRect = new Rect();
        public Rect VectRect//现在的位置矩形
        {
            get => new Rect(CanvasRect.X + Vect.X, CanvasRect.Y + Vect.Y, CanvasRect.Width, CanvasRect.Height);
        }
      

        //栅格化
        public void Rasterize()
        {
            if (Vect.X!=0||Vect.Y!=0)
            {
                //绘画
                using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
                {
                    ds.Clear(Colors.Transparent);
                    ds.DrawImage(App.GetTransform(this.CanvasRenderTarget, Vect));
                }
                using (CanvasDrawingSession ds = this.CanvasRenderTarget.CreateDrawingSession())
                {
                    ds.Clear(Colors.Transparent);
                    ds.DrawImage(App.Model.SecondBottomRenderTarget);
                }

                //缩略图
                SetWriteableBitmap(App.Model.VirtualControl);


                //栅格化
                CanvasRect = App.GetBounds(this.CanvasRenderTarget.GetPixelColors(), App.Model.Width, App.Model.Height);
                Vect = new Vector2(0, 0);
            }
        }


        #endregion





        #region Thumbnail：缩略图


        //图层缩略图
        private WriteableBitmap writeableBitmap;
        public WriteableBitmap WriteableBitmap
        {
            get
            {
                return writeableBitmap;
             }
            set
            {
                writeableBitmap = value;
                this.OnPropertyChanged("WriteableBitmap");
            }
        }

        //设置图层缩略图
        public void SetWriteableBitmap(ICanvasResourceCreatorWithDpi rc)
        {
            try
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(rc, (int)(this.width * App.Model.DPIScale), (int)(this.height * App.Model.DPIScale));
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(new ScaleEffect
                    {
                        Source = this.CanvasRenderTarget,
                        Scale = new Vector2((float)(width * App.Model.DPIScale / this.CanvasRenderTarget.SizeInPixels.Width), (float)(this.height * App.Model.DPIScale / this.CanvasRenderTarget.SizeInPixels.Height))
                    });
                };
                WriteableBitmap = Library.Image.BuffertoBitmap(crt);

                CanvasRect = App.GetBounds(this.CanvasRenderTarget.GetPixelColors(), App.Model.Width, App.Model.Height);

            }
            catch (System.Exception)
            {

             }
         } 






        //列表视图宽度/高度
        private double width = 160;
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                this.OnPropertyChanged("Width");
            }
        }
        private double height = 160;
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                this.OnPropertyChanged("Height");
            }
        }

        public void LowView()
        {
            this.Height = 50;
            this.Width = App.Model.Width* Height / App.Model.Height ;
        }
        public void SquareView()
        {
            this.Width = 160;
            this.Height = App.Model.Height* Width / App.Model.Width ;
        }



        #endregion



        public Layer()
        {
            SquareView();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
}
