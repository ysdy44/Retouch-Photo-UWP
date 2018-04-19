using Windows.UI;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
 using static 修图.Model.Model;

namespace 修图.Model
{
  public  class Undo
    {
        public UndeoType undeoType = new UndeoType();


        int Index;


        #region Global：全局



        public void Perform()   //执行
        {
            try
            {
                switch (undeoType)
                {
                    case UndeoType.Targe: TargePerform(); break;
                    case UndeoType.Mask: MaskPerform(); break;

                    case UndeoType.Index: IndexPerform(); break;
                    case UndeoType.Collection: CollectionPerform(); break;
                    case UndeoType.Add: AddPerform(); break;
                    case UndeoType.Remove: RemovePerform(); break;

                    case UndeoType.MergeDown: MergeDownPerform(); break;

                    case UndeoType.Visual: VisualPerform(); break;
                    case UndeoType.Opacity: OpacityPerform(); break;
                    case UndeoType.Blend: BlendPerform(); break;

                    case UndeoType.Tool: ToolPerform(); break;


                    default: break;
                }
            }
            catch (System.Exception)
            {
            }
        }


        public void Dispose()//释放
        {
            switch (undeoType)
            {
                case UndeoType.Targe :  TargeDispose(); break;
                case UndeoType.Mask :  MaskDispose(); break;

                case UndeoType.Index :  IndexDispose(); break;
                case UndeoType.Collection :  CollectionDispose(); break;
                case UndeoType.Add: AddDispose(); break;
                case UndeoType.Remove: RemoveDispose(); break;

                case UndeoType.MergeDown: MergeDownDispose(); break;

                case UndeoType.Visual :  VisualDispose(); break;
                case UndeoType.Opacity :  OpacityDispose(); break;
                case UndeoType.Blend :  BlendDispose(); break;

                case UndeoType.Tool :  ToolDispose(); break;

                default:  break;
            }
        }

        #endregion


        #region Targe：目标


      public  CanvasRenderTarget Targe;
        public Layer Targelayer;
        public void TargeInstantiation(int Index, ICanvasImage ci)
        {
            try
            {
                undeoType = UndeoType.Targe;

                Targe = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
                using (var ds = Targe.CreateDrawingSession())
                {
                    ds.DrawImage(ci);
                }

                this.Index = Index;
                this.Targelayer = App.Model.Layers[Index];
            }
            catch (System.Exception)
            { 
            }
        }


        public void TargePerform()
        {
            using (var ds = Targelayer.CanvasRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(Targe);
            }
            App.Model.Layers[Index].SetWriteableBitmap(App.Model.VirtualControl);//缩略图
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
        public void TargeDispose()
        {
            Targe.Dispose();
        }

        #endregion

        #region Mask：选区


        CanvasRenderTarget MaskVirtual;
        CanvasRenderTarget MaskAnimated;

        public void MaskInstantiation(ICanvasImage vi, ICanvasImage an)
        {
            undeoType = UndeoType.Mask ;

            MaskVirtual = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (var ds = MaskVirtual.CreateDrawingSession())
            {
                ds.DrawImage(vi);
            }

            MaskAnimated = new CanvasRenderTarget(App.Model.AnimatedControl, App.Model.Width, App.Model.Height);
            using (var ds = MaskAnimated.CreateDrawingSession())
            {
                ds.DrawImage(an);
            }
        }
        public void MaskPerform()
        {
            using (var ds = App.Model.MaskRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(MaskVirtual);
            }

            using (var ds = App.Model.MaskAnimatedTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(MaskAnimated);
            }

            //画蓝色半透明选区
            if (App.Model.isMask == true) App.Model.isReRender = true;//重新渲染
            App.Model.isReStroke = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
        public void MaskDispose()
        {
            MaskVirtual.Dispose();
            MaskAnimated.Dispose();
        }

        #endregion



        #region Index：图层索引


        public void IndexInstantiation(int i)
        {
            undeoType = UndeoType.Index;

            this.Index = i;
        }
        public void IndexPerform()
        {
            App.Model.isCanUndo = false;//关闭撤销

            App.Model.Index=this.Index;

            App.Model.isCanUndo = true;//打开撤销
        }
        public void IndexDispose()
        {
        }


        #endregion

        #region Collection：集合改变


        public List<Layer> Layers;

        public void CollectionInstantiation(int i, IList<Layer> l)
        {
            undeoType = UndeoType.Collection;

            this.Index = i;

            this.Layers = new List<Layer> { };
            foreach (var item in l)
            {
                Layers.Add(item);
            }
        }

        public void CollectionPerform()
        {
            if (this.Layers.Count > 0)//避免这个类为空
            {
                App.Model.isCanUndo = false;//关闭撤销

                //清空全局层并根据这个撤销类遍历添加
                App.Model.Layers.Clear();
                for (int i = 0; i < this.Layers.Count; i++)
                {
                    App.Model.Layers.Add(this.Layers[i]);
                }
                App.Model.Index=this.Index;

                App.Model.isCanUndo = true;//打开撤销

                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }

        public void CollectionDispose()
        {
            Layers.Clear();
        }


        #endregion

        #region Add：添加图层


        public void AddInstantiation(int i)
        {
            undeoType = UndeoType.Add;

            this.Index = i;
        }
        public void AddPerform()
        {
            App.Model.isCanUndo = false;//关闭撤销

            App.Model.Layers.RemoveAt(this.Index);
            App.Model.Index = this.Index;

            App.Model.isCanUndo = true;//打开撤销      

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
        public void AddDispose()
        {
        }


        #endregion

        #region Remove：移除图层

        Layer layer;

        public void RemoveInstantiation(int i,Layer l)
        {
            undeoType = UndeoType.Remove;

            this.Index = i;
            this.layer = l;
        }
        public void RemovePerform()
        {
            App.Model.isCanUndo = false;//关闭撤销

            App.Model.Layers.Insert(this.Index,this.layer);
            App.Model.Index = this.Index;

            App.Model.isCanUndo = true;//打开撤销
 
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
        public void RemoveDispose()
        {
        }


        #endregion


        #region MergeDown：合并图层

        Layer layerA;
        Layer layerB;

        public void MergeDownInstantiation(int i, Layer A,Layer B)
        {
            undeoType = UndeoType.MergeDown;

            this.Index = i;
            this.layerA = A;
            this.layerB = B;
        }
        public void MergeDownPerform()
        {
            App.Model.isCanUndo = false;//关闭撤销
 
            App.Model.Layers.RemoveAt(this.Index);
            App.Model.Layers.Insert(this.Index, this.layerB);
            App.Model.Layers.Insert(this.Index, this.layerA);
            App.Model.Index = this.Index;

            App.Model.isCanUndo = true;//打开撤销

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
        public void MergeDownDispose()
        {
        }


        #endregion



        #region Visual：可视


        bool Visual;

        public void VisualInstantiation(int i, bool v)//可视改变
        {
            undeoType = UndeoType.Visual;

            this.Index = i;
            this.Visual = v;
        }
        public void VisualPerform()
        {
            App.Model.Layers[Index].SetVisual(this.Visual);//不产生撤销类的方法，适用于撤销的操作

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
        public void VisualDispose()
        {

        }

        #endregion

        #region Opacity：透明度


        double Opacity;

        public void OpacityInstantiation(int i, double o)//透明度改变
        {
            undeoType = UndeoType.Opacity;

            this.Index = i;
            this.Opacity = o;
        }
        protected void OpacityPerform()
        {
            App.Model.Layers[Index].Opacity = this.Opacity;//不产生撤销类的方法，适用于撤销的操作

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }
        protected void OpacityDispose()
        {

        }


        #endregion

        #region Blend：混合


        int Blend;

        public void BlendInstantiation(int i, int b)//透混合改变
        {
            undeoType = UndeoType.Blend;

            this.Index = i;
            this.Blend = b;
        }
        protected void BlendPerform()
        {
            if (Index>=0&& Index<App.Model.Undos.Count)
            {
            App.Model.Layers[Index].SetBlendIndex(this.Blend);//不产生撤销类的方法，适用于撤销的操作

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
             }
        }
        protected void BlendDispose()
        {
        }

        #endregion



        #region Tool：工具改变


        int Tool;

        public void ToolInstantiation(int t)//工具改变
        {
            undeoType = UndeoType.Tool;

            this.Tool = t;
        }
        protected void ToolPerform()
        {
            App.Model.Tool = this.Tool;
        }
        protected void ToolDispose()
        {

        }


        #endregion



        #region XXXXX：啊啊啊啊啊




        public void LayerAddInstantiation(int Index)//图层增加改变
        {
            undeoType = UndeoType.Add ;

            this.Index = Index;
        }
        public void LayerAddUndo()
        {
            if (Index >= 0 && Index < App.Model.Undos.Count)
            App.Model.Layers.RemoveAt(Index);
        }


         public void LayerRemoveInstantiation(int Index,Layer layer)//图层移除改变
        {
            undeoType = UndeoType.Remove ;

            this.Index = Index;
            this.layer = layer;
        }
         public void LayerRemoveUndo()
        {
            if (Index >= 0 && Index < App.Model.Undos.Count)
 
                App.Model.Layers.Insert(Index, layer);
        }



        #endregion
         

    }
}
