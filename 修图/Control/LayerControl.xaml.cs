using System; 
using System.Collections.Specialized;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
 using Windows.UI.Xaml.Input; 
using Windows.Storage.Streams;
 using Microsoft.Graphics.Canvas; 
using Windows.Storage.Pickers;
using Windows.Storage; 
using 修图.Model;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.Generic;
using System.Linq;

namespace 修图.Control
{
    public sealed partial class LayerControl : UserControl
    {
        //Undo：撤销
        Undo undo;//旧的撤销类

        //Delegate
        public delegate void LayerHandler();
        public event LayerHandler Layers;

        public LayerControl()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;

            App.Model.Layers.CollectionChanged += Bitmaps_CollectionChanged;//挂集合变化事件
            ListView.ItemsSource = App.Model.Layers;//绑定集合

            //Undo：撤销
            undo = new Undo();
            undo.CollectionInstantiation (App.Model.Index, App.Model.Layers);
        }

        #region Global：全局


        bool isCurrent = false;
 
        //选择索引改变
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             isCurrent = false;
        }
        //顺序改变
        private void Bitmaps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Undo：撤销
            //当在界面上拖动ListView改变顺序时，会因为List.Move方法而触发CollectionChanged集合改变事件
            //可以根据集合改变事件new一个撤销类，再把上次的图层的顺序打进去。
            //问题在于，无论List.Add还是List.Move都会触发集合改变事件，
            //于是记录List.Count存储到App.Model.LayersCount 里
            //当新的List.Count和旧的记录数App.Model.LayersCount相等时，就知道这次的集合改变事件是List.Move触发的，打入撤销类
            //如果不是就知道这是List,Add或List.Remove或别的什么方法触发的集合改变事件，就不撤销
            if (undo != null)
            {
                if (undo.Layers != null)
                {
                    if (App.Model.LayersCount == App.Model.Layers.Count)
                    {
                        if (App.Model.isCanUndo == true)
                        {
                            App.Model.Undos.Add(undo);//打入旧的撤销类

                            //更新撤销类
                            undo = new Undo();
                            undo.CollectionInstantiation(App.Model.Index, App.Model.Layers);
                        }
                    }
                }
            }

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }



        #endregion

        

        #region ListView：列表视图


        //图层可视
        private void LayerVisualToggleButton_Click(object sender, RoutedEventArgs e)
        {
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        //图层透明度
        private void LayerOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        //图层混合
        private void LayerBlendToggle_Click(object sender, RoutedEventArgs e)
        {
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }

        //图层混合模式
        private void LayerBlendCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
        }




        private void LayerFlyout_Closed(object sender, object e)
        {
            Layer I = App.Model.Layers[App.Model.Index];

            //Undo：撤销
            if (I.Opacity!=I.OldOpacity) I.UndoOpacity(App.Model.Index);
        }

        //图片点击
        private void LayerImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (isCurrent == true)
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            isCurrent = true;

            Jugde();//判断
        }

        //图片右击   
        private void LayerImage_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

            Jugde();//判断
        }


        #endregion



        #region Add：新建


        //Blank：空白图层
        private void LayerBlankButton_Click(object sender, RoutedEventArgs e)
        {
            Blank();
            Jugde();//判断
        }
        public static void Blank()
        {
            App.Model.isCanUndo = false;//关闭撤销
            int index = App.Model.Index;

            //新建渲染目标=>层
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            Layer l = new Layer { Visual = true, Opacity = 100, CanvasRenderTarget = crt, };
            if (App.Model.isLowView) l.LowView();
            else l.SquareView();
 
            //Undo：撤销
            Undo undo = new Undo();
            undo.AddInstantiation(App.Model.Index);
            App.UndoAdd(undo);

             App.Model.Layers.Insert(App.Model.Index, l);
            l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
            App.Model.Index = index + 1;

           App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新

            App.Model.LayersCount = App.Model.Layers.Count;//更新图层数量以供判断集合撤销
            App.Model.isCanUndo = true;//打开撤销      
         }

        //复制图层
        private async void LayerDuplicateButton_Click(object sender, RoutedEventArgs e) { Duplicate(); }
        public static void Duplicate()
        {
            App.Model.isCanUndo = false;//关闭撤销

            Layer l = App.Model.Layers[App.Model.Index];

            //复制渲染目标
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (var ds = crt.CreateDrawingSession())
            {
                ds.DrawImage(l.CanvasRenderTarget);
            }
            Layer ll = new Layer { Visual = true, Opacity = l.Opacity, CanvasRenderTarget = crt, BlendIndex = l.BlendIndex, };
            if (App.Model.isLowView) ll.LowView();
            else ll.SquareView();

            //Undo：撤销
            Undo undo = new Undo();
            undo.AddInstantiation(App.Model.Index);
            App.UndoAdd(undo);

            App.Model.Layers.Insert(App.Model.Index, ll);
            ll.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图

 
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新

            App.Model.LayersCount = App.Model.Layers.Count;//更新图层数量以供判断集合撤销
            App.Model.isCanUndo = true;//打开撤销      
        }

        //图片图层
        private async void LayerImageButton_Click(object sender, RoutedEventArgs e) 
        {
            //文件选择器
            FileOpenPicker openPicker = new FileOpenPicker();
            //选择视图模式
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            //初始位置
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //添加文件类型
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            StorageFile file = await openPicker.PickSingleFileAsync();//打开选择器

            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    //图片
                    App.Model.SecondCanvasBitmap = await CanvasVirtualBitmap.LoadAsync(App.Model.VirtualControl, stream);
                    App.InitializeOther();
                    Layers();

                    App.Model.isReRender = true;//重新渲染
                    App.Model.Refresh++;//画布刷新
                    App.Model.LayersCount = App.Model.Layers.Count;//Undo：撤销
                }
            }
        }


        //选区图层
        private void LayerSelectionButton_Click(object sender, RoutedEventArgs e) { Selection(); }
        private void Selection()
        {
            App.Model.isCanUndo = false;//关闭撤销

            Layer l = App.Model.Layers[App.Model.Index];

            //画布：输入源图、形状，输出带形状的源图
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (CanvasDrawingSession ds = crt.CreateDrawingSession())
            {
                ds.DrawImage(l.CanvasRenderTarget);
                CanvasComposite compositeMode = CanvasComposite.DestinationIn;//差模式
                ds.DrawImage(App.Model.MaskRenderTarget, 0, 0, l.CanvasRenderTarget.Bounds, 1, CanvasImageInterpolation.Linear, compositeMode);
            }

            //新建层
            Layer ll = new Layer { Visual = true, Opacity = l.Opacity, CanvasRenderTarget = crt, BlendIndex = l.BlendIndex };
            if (App.Model.isLowView) ll.LowView();
            else ll.SquareView();

            //Undo：撤销
            Undo undo = new Undo();
            undo.AddInstantiation(App.Model.Index);
            App.UndoAdd(undo);

             App.Model.Layers.Insert(App.Model.Index, ll);
            ll.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
            
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
            App.Model.LayersCount = App.Model.Layers.Count;//Undo：撤销

            App.Model.isCanUndo = true;//打开撤销
        }


        private void AddFlyout_Opened(object sender, object e)
        {

        }


        #endregion



        #region Merge：合并


        //Merge Down：向下合并
        private void LayerMergeDownButton_Click(object sender, RoutedEventArgs e)
        {
            App.Model.isCanUndo = false;//关闭撤销

            var index = App.Model.Index;
            if (index < App.Model.Layers.Count - 1 && App.Model.Layers.Count > 1)
            {
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
                Layer BLayer = App.Model.Layers[index + 1];//下层
                Layer ALayer = App.Model.Layers[index];//此层

                //Undo：撤销
                Undo undo = new Undo();
                undo.MergeDownInstantiation(index, ALayer,BLayer);
                App.UndoAdd(undo);

                //合并
                using (CanvasDrawingSession ds = crt.CreateDrawingSession())
                {
                    ds.DrawImage(BLayer.CanvasRenderTarget, 0, 0, BLayer.CanvasRenderTarget.Bounds, (float)(BLayer.Opacity / 100));
                    ds.DrawImage(ALayer.CanvasRenderTarget, 0, 0, ALayer.CanvasRenderTarget.Bounds, (float)(ALayer.Opacity / 100));
                }
                Layer Merge = new Layer { Name = App.resourceLoader.GetString("/Layer/NameMerge_"), Visual = true, Opacity = 100, CanvasRenderTarget = crt, };
                Merge.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
                if (App.Model.isLowView) Merge.LowView();
                else Merge.SquareView();
                App.Model.Layers.Insert(index, Merge); //插入

                App.Model.Layers.Remove(ALayer);
                App.Model.Layers.Remove(BLayer);
                 App.Model.Index=index;

                Jugde();//判断
            }

            App.Model.isCanUndo = true;//开启撤销

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
            App.Model.LayersCount = App.Model.Layers.Count;//Undo：撤销
        }

        //Merge Visual：合并可见
        private void LayerMergeVisualButton_Click(object sender, RoutedEventArgs e)
        {
            App.Model.isCanUndo = false;//关闭撤销

            //更新撤销类
            Undo undo = new Undo();
            undo.CollectionInstantiation(App.Model.Index, App.Model.Layers);
            App.UndoAdd(undo);


            //新建渲染目标=>层
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (CanvasDrawingSession ds = crt.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                ICanvasImage ci = App.Model.NullRenderTarget;
                for (int i = App.Model.Layers.Count - 1; i >= 0; i--)//自下而上渲染
                {
                    ci = App.Render(App.Model.Layers[i], ci);//渲染
                }
                ds.DrawImage(ci);
            }

            //删掉所有可视图层
            for (int i = App.Model.Layers.Count - 1; i >= 0; i--)//自下而上
            {
                Layer L = App.Model.Layers[i];
                if (L.Visual == true) App.Model.Layers.Remove(L);
            }

            //插入与索引
            Layer l = new Layer { Name = App.resourceLoader.GetString("/Layer/NameMerge_"), Visual = true, Opacity = 100, CanvasRenderTarget = crt, };
            if (App.Model.isLowView) l.LowView();
            else l.SquareView();

            l.SetWriteableBitmap(App.Model.VirtualControl);//刷新缩略图
            App.Model.Layers.Insert(0, l);
            App.Model.Index=0;

            App.Model.isCanUndo = true;//打开撤销
            App.Model.LayersCount = App.Model.Layers.Count;//Undo：撤销

            Jugde();//判断
         }

        //Merge All：全部合并
        private void LayerMergeAllButton_Click(object sender, RoutedEventArgs e)
        {
            App.Model.isCanUndo = false;//关闭撤销

            //更新撤销类
            Undo undo = new Undo();
            undo.CollectionInstantiation(App.Model.Index, App.Model.Layers);
            App.UndoAdd(undo);

            //新建渲染目标层
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (CanvasDrawingSession ds = crt.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                //全部合并
                ICanvasImage ci = App.Model.NullRenderTarget;
                for (int i = App.Model.Layers.Count - 1; i >= 0; i--)//自下而上渲染
                {
                    ci = App.Render(App.Model.Layers[i], ci);//渲染
                }
                ds.DrawImage(ci);
            }

            //删掉所有图层
            App.Model.Layers.Clear();
            //插入层        
            Layer l = new Layer { Name = App.resourceLoader.GetString("/Layer/NameMerge_"), CanvasRenderTarget = crt,};
            if (App.Model.isLowView) l.LowView();
            else l.SquareView();

            l.SetWriteableBitmap(App.Model.VirtualControl);
            App.Model.Layers.Add(l);
            App.Model.Index=0;//不产生撤销类的改变索引的方法，适用于撤销的操作

            App.Model.isCanUndo = true;//开启撤销
            App.Model.LayersCount = App.Model.Layers.Count;//Undo：撤销

             Jugde();//判断
         }

        //Remove：移除图层
        private void LayerRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            App.Model.isCanUndo = false;//关闭撤销

            int index = App.Model.Index;

            if (App.Model.Layers.Count > 1)
            {
                 //Undo：撤销
                Undo undo = new Undo();
                undo.RemoveInstantiation(index, App.Model.Layers[index]);
                App.UndoAdd(undo);

                App.Model.Layers.RemoveAt(index);
                App.Model.Index=index - 1 > 0 ? index - 1 : 0;
              }

            App.Model.isCanUndo = true;//打开撤销    
            
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
            App.Model.LayersCount = App.Model.Layers.Count;//Undo：撤销

            Jugde();//判断
         }




        #endregion


        private void Jugde()//判断
        {          
            //判断：选区图层
            if (App.Model.isAnimated==true) LayerSelectionButton.IsEnabled = true;
            else LayerSelectionButton.IsEnabled = false;
         
            //判断：向下合并
            if (App.Model.Index < App.Model.Layers.Count - 1 && App.Model.Layers.Count >= 2) LayerMergeDownButton.IsEnabled = true;
            else LayerMergeDownButton.IsEnabled = false;

            //判断：合并可视
            bool isVisual = false;
            foreach (Layer l in App.Model.Layers)
            {
                if (l.Visual == true) isVisual = true;
            }
            if (isVisual == true && App.Model.Layers.Count >= 2) LayerMergeVisualButton.IsEnabled = true;
            else LayerMergeVisualButton.IsEnabled = false;

            //判断：合全部并
            if (App.Model.Layers.Count >= 2) LayerMergeAllButton.IsEnabled = true;
            else LayerMergeAllButton.IsEnabled = false;

            //判断：删除图层
            if (App.Model.Layers.Count >= 2) LayerRemoveButton.IsEnabled = true;
            else LayerRemoveButton.IsEnabled = false;
        }

        public static void Apply()
        {
            App.Model.isCanUndo = false;//关闭撤销

            var index = App.Model.Index;
 
            //新建渲染目标=>层
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            using (CanvasDrawingSession ds = crt.CreateDrawingSession())
            {
                ds.DrawImage(App.Model.SecondCanvasImage);
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
            App.Model.Index=index;

            App.Model.isCanUndo = true;//打开撤销
         }

    }
}
