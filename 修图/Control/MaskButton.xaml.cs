using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls; 
using Windows.UI.Xaml.Input; 
using Microsoft.Graphics.Canvas; 
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI;
using 修图.Library;
using 修图.Model;

namespace 修图.Control
{
    public sealed partial class MaskButton : UserControl
    {
        //Delegate
        public delegate void MaskHandler(int Index);
        public event MaskHandler Mask;
      
        public MaskButton()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
         }

        #region Global：全局

        private void flyout_Opened(object sender, object e)
        {
            //初始化选区渲染目标
            App.InitializeMask();

            App.Judge();//判断选区，改变是否动画与选区矩形 
        }

        #endregion


        #region Edit：编辑


        //Cut：剪切
        private void EditCutButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Cut();
            flyout.Hide();
        }
        public static void Cut()
        {
            //如果图层不可视或透明
            if (App.Model.CurrentVisual == false) App.Tip();
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                if (App.Model.isAnimated == true)//选区存在
                {
                    App.Model.Clipboard = new CanvasRenderTarget(App.Model.VirtualControl, (float)App.Model.MaskRect.Width, (float)App.Model.MaskRect.Height);
                    using (CanvasDrawingSession ds = App.Model.Clipboard.CreateDrawingSession())
                    {
                        ds.DrawImage(App.Model.CurrentRenderTarget, (float)-App.Model.MaskRect.X, (float)-App.Model.MaskRect.Y);
                        //扣外留内
                        CanvasComposite compositeMode = CanvasComposite.DestinationIn;
                        ds.DrawImage(App.Model.MaskRenderTarget, (float)-App.Model.MaskRect.X, (float)-App.Model.MaskRect.Y, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
                    }
                    using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                    {
                        //扣内留外
                        CanvasComposite compositeMode = CanvasComposite.DestinationOut;
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
                    }
                }
                else if (App.Model.isAnimated == false)//选区不存在
                {
                    var rect = App.GetCurrentBounds();
                    App.Model.Clipboard = new CanvasRenderTarget(App.Model.VirtualControl, (float)rect.Width, (float)rect.Height);
                    using (CanvasDrawingSession ds = App.Model.Clipboard.CreateDrawingSession())
                    {
                        ds.DrawImage(App.Model.CurrentRenderTarget, (float)-rect.X, (float)-rect.Y);
                    }
                    using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                    }
                }

                App.Model.isClipboard = true;//存在剪切板
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新   
                App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图 
            }
        }




        //Copy：复制
        private void EditCopyButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Copy();
            flyout.Hide();
        }
        public static void Copy()
        {
            if (App.Model.isAnimated == true)//选区存在
            {
                App.Model.Clipboard = new CanvasRenderTarget(App.Model.VirtualControl, (float)App.Model.MaskRect.Width, (float)App.Model.MaskRect.Height);
                using (CanvasDrawingSession ds = App.Model.Clipboard.CreateDrawingSession())
                {
                    ds.DrawImage(App.Model.CurrentRenderTarget, (float)-App.Model.MaskRect.X, (float)-App.Model.MaskRect.Y);
                    //扣外留内
                    CanvasComposite compositeMode = CanvasComposite.DestinationIn;
                    ds.DrawImage(App.Model.MaskRenderTarget, (float)-App.Model.MaskRect.X, (float)-App.Model.MaskRect.Y, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
                }
            }
            else if (App.Model.isAnimated == false)//选区不存在
            {
                var rect = App.GetCurrentBounds();
                App.Model.Clipboard = new CanvasRenderTarget(App.Model.VirtualControl, (float)rect.Width, (float)rect.Height);
                using (CanvasDrawingSession ds = App.Model.Clipboard.CreateDrawingSession())
                {
                    ds.DrawImage(App.Model.CurrentRenderTarget, (float)-rect.X, (float)-rect.Y);
                }
            }
            App.Model.isClipboard = true;//存在剪切板
        }




        //Paste：粘贴
        private void EditPasteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.Model.isClipboard == true)
            {
                App.InitializeOther();

                flyout.Hide();
                Mask(102);
            }
        }



        //Clear：清除
        private void EditClearButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Clear();
            flyout.Hide();
        }
        public static void Clear()
        {
            //如果图层不可视或透明
            if (App.Model.CurrentVisual == false) App.Tip();
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                if (App.Model.isAnimated == true)//选区存在
                {
                    using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                    {
                        //扣内留外
                        CanvasComposite compositeMode = CanvasComposite.DestinationOut;
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
                    }
                }
                else if (App.Model.isAnimated == false)//选区不存在
                {
                    using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                    }
                }


                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新    
                App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
            }
        }




        //Extract：提取
        private void EditExtractButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Extract();
            flyout.Hide();
        }
        public static void Extract()
        {
            //如果图层不可视或透明
            if (App.Model.CurrentVisual == false) App.Tip();
            else
            {
                //Undo：撤销
                Undo undo = new Undo();
                undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                App.UndoAdd(undo);

                if (App.Model.isAnimated == true)//选区存在
                {
                    using (CanvasDrawingSession ds = App.Model.CurrentRenderTarget.CreateDrawingSession())
                    {
                        //扣外留内
                        CanvasComposite compositeMode = CanvasComposite.DestinationIn;
                        ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
                    }
                }

                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新    
                App.Model.Layers[App.Model.Index].SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
            }
        }


        //Merge：复制合并
        private void EditMergeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Merge();
            flyout.Hide();
        }
        public static void Merge()
        {
            //使Source为合成所有图层
            App.InitializeCrop();

            if (App.Model.isAnimated == true)//选区存在
            {
                App.Model.Clipboard = new CanvasRenderTarget(App.Model.VirtualControl, (float)App.Model.MaskRect.Width, (float)App.Model.MaskRect.Height);
                using (CanvasDrawingSession ds = App.Model.Clipboard.CreateDrawingSession())
                {
                    ds.DrawImage(App.Model.SecondTopRenderTarget, (float)-App.Model.MaskRect.X, (float)-App.Model.MaskRect.Y);
                    //扣外留内
                    CanvasComposite compositeMode = CanvasComposite.DestinationIn;
                    ds.DrawImage(App.Model.MaskRenderTarget, (float)-App.Model.MaskRect.X, (float)-App.Model.MaskRect.Y, App.Model.MaskRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
                }
            }
            else if (App.Model.isAnimated == false)//选区不存在
            {
                App.Model.Clipboard = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width,App.Model.Height);
                using (CanvasDrawingSession ds = App.Model.Clipboard.CreateDrawingSession())
                {
                    ds.DrawImage(App.Model.SecondTopRenderTarget);
                }
            }
            App.Model.isClipboard = true;//存在剪切板
        }


        #endregion


        #region  Mask：选区


        //All：全选
        private void EditAllButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            All();
            flyout.Hide();
        }
        public static void All()
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            App.MaskClear(App.Setting.MaskColor);

            //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = true;

            if (App.Model.isMask==true)
            {
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }



        //Deselect：取消选择
        private void EditDeselectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Deselect();
            flyout.Hide();
        }
        public static void Deselect()
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget,App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            App.MaskClear();

            //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = true;

            if (App.Model.isMask == true)
            {
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }



        //Pixel：像素选择
        private void EditPixelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Pixel();
            flyout.Hide();
        }
        public static void Pixel()
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            //复制
            CanvasRenderTarget crt =new CanvasRenderTarget(App.Model.AnimatedControl,App.Model.Width,App.Model.Height);
            crt.SetPixelBytes(App.Model.Layers[App.Model.Index].CanvasRenderTarget.GetPixelBytes());

            App.Mask(App.Model.Layers[App.Model.Index].CanvasRenderTarget, crt);

            //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = true;

            if (App.Model.isMask == true)
            {
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }

        //Invert：反选
        private void EditInvertButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Invert();
            flyout.Hide();
        }
        public static void Invert()
        {
            //Undo：撤销
            Undo undo = new Undo();
            undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
            App.UndoAdd(undo);

            CanvasCommandList vcl = new CanvasCommandList(App.Model.VirtualControl);
            using (var ds = vcl.CreateDrawingSession())
            {
                ds.Clear(Colors.White);
            }

            CanvasCommandList acl = new CanvasCommandList(App.Model.AnimatedControl);
            using (var ds = acl.CreateDrawingSession())
            {
                ds.Clear(Colors.White);
            }
             App.Mask(vcl, acl,4);//反选


             //重新设置描边
            App.Model.isReStroke = true;
            App.Model.isAnimated = true;

            if (App.Model.isMask == true)
            {
                App.Model.isReRender = true;//重新渲染
                App.Model.Refresh++;//画布刷新
            }
        }


        //Feather：羽化
        private void EditFeatherButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeCrop();//初始化

            Mask(110);
            flyout.Hide();
        }

        //Transform：变换选区
        private void EditTransformButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.InitializeCrop();//初始化

            Mask(111);
            flyout.Hide();
        }


        #endregion


    }
}
