using System;
using System.Linq;
using System.Xml.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.Storage;
using Windows.Storage.Streams;
 using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using 修图.Library;
using 修图.Model;
using static 修图.Model.Model;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace 修图
{
    public sealed partial class DrawPage : Page
    {
        public DrawPage()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;

            JudgeMode();
        }

        #region Global：全局


        //页面
        private void page_Loaded(object sender, RoutedEventArgs e)
        {
            JudgeWidth(App.Model.ScreenWidth);
            JudgeMode();
            LoadingControl.IsLoading = false;//Con：加载控件
          }
        private void page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            App.Model.ScreenWidth = e.NewSize.Width;
            App.Model.ScreenHeight = e.NewSize.Height;

            JudgeWidth(App.Model.ScreenWidth);
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)//这是新页面准备的时候//执行的事件，(检查导航请求并且准备供显示的页面时)
        {
            base.OnNavigatedTo(e);

        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)//当页面成为非活动的时候 的事件
        {
            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新     
        }

        #endregion

        #region Drop：拖放


        private async void GGGGG_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems) == true)
            {
                var def = e.GetDeferral();
                var items = await e.DataView.GetStorageItemsAsync();

                foreach (var item in items)
                {
                    var file = item as StorageFile;
                    if (file.FileType == ".jpg" | file.FileType == ".png")
                    {
                        using (IRandomAccessStream stream = await file.OpenReadAsync())
                        {
                            App.Model.SecondCanvasBitmap = await CanvasVirtualBitmap.LoadAsync(App.Model.VirtualControl, stream);
                            App.InitializeOther();
                            LayerControl_Layers();
                        }
                    }
                }
            }
        }

        private void GGGGG_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = App.resourceLoader.GetString("DropAcceptable_");//可以接受的图片
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }






        #endregion


        #region Delegate：委托事件


        int tool;//临时工具


        private void LayerControl_Layers()   //Layer：图层 （Tool=-1）
        {
            SplitViewCollapsed();//两侧侧栏：模糊特效 

            //侧栏
            if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath)//如果手机竖屏或手机横屏
            {
                //关闭
                LeftSplitView.IsPaneOpen = RightSplitView.IsPaneOpen = false;
            }

            tool = App.Model.Tool;
            App.Model.Tool = 400;
        }

        private void MaskButton_Mask(int Index) //Mask：选区 （Tool=100~199）
        {
            App.Model.isUndo = false;//Undo：撤销  
            App.Model.isRedo = false;//Redo：重做  

            SplitViewCollapsed();//两侧侧栏：模糊特效 

            tool = App.Model.Tool;
            App.Model.Tool = Index;
        }

        private  void EffectBuuton_Effect(int Index)   //Effect：特效（Tool=200~299）
        {
            App.Model.isUndo = false;//Undo：撤销  
            App.Model.isRedo = false;//Redo：重做  

            SplitViewCollapsed();//两侧侧栏：模糊特效 

            tool = App.Model.Tool;
            App.Model.Tool = Index;
        }

        private void OtherButton_Other(int Index)   //Other：杂项（Tool=300~399）
        {
            App.Model.isUndo = false;//Undo：撤销  
            App.Model.isRedo = false;//Redo：重做  

            SplitViewCollapsed();//两侧侧栏：模糊特效 

            tool = App.Model.Tool; //初始化上中下渲染目标
            App.Model.Tool = Index;
        }


      


        //Animations：动画
        //两侧侧栏：模糊特效
        private void SplitViewCollapsed()
        {
            LeftPane.Saturation(value: 0, duration: 600, delay: 0).StartAsync();
            RightPane.Saturation(value: 0, duration: 600, delay: 0).StartAsync();
        
            LeftGrid.Visibility = Visibility.Visible;
            RightGrid.Visibility = Visibility.Visible;

            //侧栏
            if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath)//如果手机竖屏或手机横屏
            {
                LeftBorder.Visibility = Visibility.Collapsed;
                RightBorder.Visibility = Visibility.Collapsed;
            }

            //顶栏动画  
            MainTitleBar.Visibility = Visibility.Collapsed;
            SecondTitleBar.Visibility = Visibility.Visible;
        }
        private void SplitViewVisible()
        {
            LeftPane.Saturation(value: 1, duration: 600, delay: 0).StartAsync();
            RightPane.Saturation(value: 1, duration: 600, delay: 0).StartAsync();
         

            LeftGrid.Visibility = Visibility.Collapsed;
            RightGrid.Visibility = Visibility.Collapsed;

            //侧栏
            if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath)//如果手机竖屏或手机横屏
            {
                LeftBorder.Visibility = Visibility.Visible;
                RightBorder.Visibility = Visibility.Visible;
            }

            //顶栏动画
            MainTitleBar.Visibility = Visibility.Visible;
            SecondTitleBar.Visibility = Visibility.Collapsed;
        }


        #endregion

        #region Second：第二界面

        //取消
        private void CancelButton_Tapped(object sender, TappedRoutedEventArgs e) { Cancel(); }
        private  void Cancel()
        {
            SplitViewVisible();//两侧侧栏：模糊特效 

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
            App.Model.Tool = tool;//全局工具       

            App.Model.isUndo = true;//Undo：撤销  
            App.Model.isRedo = false;//Redo：重做  

            JudgeMode();
        }

        //确定
        private void ApplyButton_Tapped(object sender, TappedRoutedEventArgs e) { Apply(); }
        private void Apply()
        {
            int t = App.Model.Tool;

            //Mask：粘贴  // Layer：图层 
            if (t == 102 || t == 400) 修图.Control.LayerControl.Apply();
            // Feather：羽化    // Transform：变换选区  
            else if (t == 110 || t == 111) 修图.BarPage.MaskPage.Feather.Apply();
            //Effect：特效  //Fade：渐隐   //Transform：变换   //Transform3D：变换3D  // Adjust：调整 // Liquify：液化
            else if ((t >= 200 && t < 300) || t == 304 || t == 308 || t == 309 || t == -2 || t == -3) 修图.BarPage.OtherPage.Fade.Apply();
            //Cutting：裁切 
            else if (t == 300) 修图.BarPage.OtherPage.Crop.Apply();
            //Gradient：渐变 //Fill：填充 
            else if (t == 303 || t == 307) 修图.BarPage.OtherPage.Gradient.Apply();
            //Grids：网格线
            else if (t == 306) 修图.BarPage.OtherPage.Grids.Apply();
            //Text：文字 
            else if (t == 305) 修图.BarPage.OtherPage.Text.Apply();

            SplitViewVisible();//两侧侧栏：模糊特效 

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新
            App.Model.Tool = tool;//全局工具

            JudgeMode();
        }




        #endregion


        #region Mode：模式
        

        //Mode：按钮
        private void BitmapModeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Model.Tool = 0;
            JudgeMode();

            App.Model.isReRender = true;//重新渲染
            App.Model.Refresh++;//画布刷新    
        }

        private void AdjustModeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //如果图层不可视或透明
            if (App.Model.Layers[App.Model.Index].Visual == false)
            {
                App.Tip();
            }
            else
            {
                App.Model.isUndo = false;//Undo：撤销  
                App.Model.isRedo = false;//Redo：重做  

                App.Judge();//判断选区
                App.InitializeEffect();//初始化

                tool = App.Model.Tool;
                App.Model.Tool = -2;

                JudgeMode();
            }
        }

        private void LiquifyModeButton_Tapped(object sender, TappedRoutedEventArgs e)
        { 
            //如果图层不可视或透明
            if (App.Model.Layers[App.Model.Index].Visual == false)
            {
                App.Tip();
            }
            else
            {
                App.InitializeLiquify();//初始化

                tool = App.Model.Tool;
                App.Model.Tool = -3;
            }
                JudgeMode();
        }

        private void PaintModeButton_Tapped(object sender, TappedRoutedEventArgs e) 
        {
            tool = App.Model.Tool;
            App.Model.Tool = -4;
            JudgeMode();
        }

        private void GeometryModeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
             tool = App.Model.Tool;
            App.Model.Tool =500;
            JudgeMode();
        }



        private void JudgeMode()
        {
            int t = App.Model.Tool;
            //Bitmap：位图
            if (t>= 0&&t<100)
            {
                //顶栏动画
                MainTitleBar.Visibility = Visibility.Visible;
                SecondTitleBar.Visibility = Visibility.Collapsed;

                //用户控件
                ToolControl.Visibility = Visibility.Visible;
                //侧栏展开
                LeftSplitView.OpenPaneLength = 60;
                RightSplitView.OpenPaneLength = 190;

                //模式选择
                ModeListView.SelectedIndex = ModeComboBox.SelectedIndex = 0; 
            }
            else
            {
                //用户控件
                ToolControl.Visibility = Visibility.Collapsed; 
            }

            //Adjust：调整
            if (t== -2)
            {
                //顶栏动画
                MainTitleBar.Visibility = Visibility.Collapsed;
                SecondTitleBar.Visibility = Visibility.Visible;

                //用户控件
                AdjustControl.Visibility = Visibility.Visible;
                FilterControl.Visibility = Visibility.Visible;
                //侧栏展开
                LeftSplitView.OpenPaneLength = 110;
                RightSplitView.OpenPaneLength = 220;

                //模式选择
                ModeListView.SelectedIndex = ModeComboBox.SelectedIndex = 1; 
            }
            else
            {
                //用户控件
                AdjustControl.Visibility = Visibility.Collapsed;
                FilterControl.Visibility = Visibility.Collapsed; 
            }

            //Liquify：液化
            if (t== -3)
            {
                //顶栏动画
                MainTitleBar.Visibility = Visibility.Collapsed;
                SecondTitleBar.Visibility = Visibility.Visible;

                //侧栏展开
                LeftSplitView.OpenPaneLength = 0;
                RightSplitView.OpenPaneLength = 0;

                //模式选择
                ModeListView.SelectedIndex = ModeComboBox.SelectedIndex  = 2; 
            }
            else
            { 
            }


            //Paint：绘画
            if (t== -4)
            {
                //顶栏动画
                MainTitleBar.Visibility = Visibility.Visible; 
                SecondTitleBar.Visibility = Visibility.Collapsed;

                //用户控件
                BrushControl.Visibility = Visibility.Visible;
              //  HistogramControl.Visibility = Visibility.Visible;

                //侧栏展开
                LeftSplitView.OpenPaneLength = 200;
                RightSplitView.OpenPaneLength = 190;

                //模式选择
                ModeListView.SelectedIndex = ModeComboBox.SelectedIndex = 3; 
            }
            else
            {
                //用户控件
                BrushControl.Visibility = Visibility.Collapsed; 
            }


            //Geometry：几何
            if (t >= 500&& t <600)
            {
                //顶栏动画
                MainTitleBar.Visibility = Visibility.Visible;
                SecondTitleBar.Visibility = Visibility.Collapsed;

                //用户控件
                GeometryControl.Visibility = Visibility.Visible;
                //侧栏展开
                LeftSplitView.OpenPaneLength = 60;
                RightSplitView.OpenPaneLength = 190;

                //模式选择
                ModeListView.SelectedIndex= ModeComboBox.SelectedIndex = 4;
             }
            else
            {
                //用户控件
                GeometryControl.Visibility = Visibility.Collapsed; 
            }


            if (t >= 0 || t == -4 || t >500)
            {
                //用户控件
                LayerControl.Visibility = Visibility.Visible;
            }
            else
            {
                //用户控件
                LayerControl.Visibility = Visibility.Collapsed;
            }
        }


        #endregion

        #region Width：屏幕宽度


        //屏幕模式枚举
        private enum WidthEnum
        {
            Initialize,//初始状态

            PhoneNarrow,//手机竖屏
            PhoneStrath,//手机横屏

            Pad,//平板横屏
            Pc//电脑
        }
        WidthEnum owe = WidthEnum.Initialize;//OldWidthEnum：旧屏幕宽度枚举
        WidthEnum nwe = WidthEnum.Initialize;//NewWidthEnum：新屏幕宽度枚举


        private void JudgeWidth(double w)
        {
            //根据屏幕宽度判断
            if (w < 700) nwe = WidthEnum.PhoneNarrow;
            else if (w >= 700 && w < 800) nwe = WidthEnum.PhoneStrath;
            else if (w >= 800 && w < 1000) nwe = WidthEnum.Pad;
            else if (w >= 1000) nwe = WidthEnum.Pc;

            if (nwe != owe)//窗口变化过程中，新旧屏幕模式枚举不一样
            {
                //侧栏          
                if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath)//如果手机竖屏或手机横屏
                {
                    //关闭
                    LeftSplitView.DisplayMode = RightSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
                    LeftSplitView.IsPaneOpen = RightSplitView.IsPaneOpen = false;
                    RightBorder.Visibility = LeftBorder.Visibility = Visibility.Visible;
                   }
                else if (nwe == WidthEnum.Pad || nwe == WidthEnum.Pc)//如果平板或电脑
                {
                    //打开
                    LeftSplitView.DisplayMode = RightSplitView.DisplayMode = SplitViewDisplayMode.Inline;
                    LeftSplitView.IsPaneOpen = RightSplitView.IsPaneOpen = true;
                    RightBorder.Visibility = LeftBorder.Visibility = Visibility.Collapsed;
                }

 
                
                //顶栏按钮
                if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath || nwe == WidthEnum.Pad)//如果手机竖屏或手机横屏或平板
                {
                    //左侧
                    ModeListView.Visibility = Visibility.Collapsed;
                    //右侧
                    MorePanel.Visibility = Visibility.Collapsed;
                    MoreButton.Visibility = Visibility.Visible;

                    //右栏
                    ModeComboBox.Visibility = Visibility.Visible;
                    MoreGrid.Visibility = Visibility.Collapsed;
                }
                else if (nwe == WidthEnum.Pc)//如果电脑
                {
                    //左侧
                    ModeListView.Visibility = Visibility.Visible;
                    //右侧
                    MorePanel.Visibility = Visibility.Visible;
                    MoreButton.Visibility = Visibility.Collapsed;

                    //右栏
                    ModeComboBox.Visibility = Visibility.Collapsed;
                    MoreGrid.Visibility = Visibility.Visible;
                }


 
                //底栏
                if (nwe == WidthEnum.PhoneNarrow || nwe == WidthEnum.PhoneStrath || nwe == WidthEnum.Pad)//如果手机竖屏或手机横屏或平板
                    BottomBorder.Margin = new Thickness(0); //沉浸
                 else if (nwe == WidthEnum.Pc)//如果电脑
                    BottomBorder.Margin = new Thickness(12); //悬浮

                //底栏
                if (nwe == WidthEnum.PhoneNarrow)//如果手机竖屏
                    BottomBorder.CornerRadius = new CornerRadius(0);
                else if (nwe == WidthEnum.PhoneStrath || nwe == WidthEnum.Pad)//如果手机横屏或平板
                    BottomBorder.CornerRadius = new CornerRadius(6, 6, 0, 0);
                else if (nwe == WidthEnum.Pc)//如果电脑
                    BottomBorder.CornerRadius = new CornerRadius(6);

                //底栏
                if (nwe == WidthEnum.PhoneNarrow)//如果手机竖屏
                    BottomBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                else if (nwe == WidthEnum.PhoneStrath || nwe == WidthEnum.Pad || nwe == WidthEnum.Pc)//如果手机横屏或平板或电脑
                    BottomBorder.HorizontalAlignment = HorizontalAlignment.Center;


 
                owe = nwe;
            }
        }









        //右按钮：打开侧栏
        private void RightButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RightSplitView.IsPaneOpen = true;
        }
        private void RightBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (Judge.IsMouse(e, LeftBorder) == true)
                RightSplitView.IsPaneOpen = true;
        }
        //左按钮：打开侧栏
        private void LeftButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LeftSplitView.IsPaneOpen = true;
        }
        private void LeftBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (Judge.IsMouse(e, RightBorder) == true)
                LeftSplitView.IsPaneOpen = true;
        }



        #endregion


        #region Undo：撤销

        private void UndoButtom_Tapped(object sender, TappedRoutedEventArgs e) { Undo(); }
        private void Undo()
        {
            if (App.Model.UndoIndex + 1 == App.Model.Undos.Count)//如果是第一次撤销
            {
                //根据最后的一个撤销的类型，保留当前的状态为新的撤销类
                Undo undo = new Undo();
                switch (App.Model.Undos.Last().undeoType)
                {
                    case UndeoType.Targe:
                        undo.TargeInstantiation(App.Model.Index, App.Model.CurrentRenderTarget);
                        break;
                    case UndeoType.Mask:
                        undo.MaskInstantiation(App.Model.MaskRenderTarget, App.Model.MaskAnimatedTarget);
                        break;

                    case UndeoType.Index:
                        undo.IndexInstantiation(App.Model.Index);
                        break;
                    case UndeoType.Collection:
                        undo.CollectionInstantiation(App.Model.Index, App.Model.Layers);
                        break;
                    //case UndeoTyte.LayerAdd :break;
                    //case UndeoTyte.LayerRemove :break;

                    case UndeoType.Visual:
                        undo.VisualInstantiation(App.Model.Index, App.Model.Layers[App.Model.Index].Visual);
                        break;
                    case UndeoType.Opacity:
                        undo.OpacityInstantiation(App.Model.Index, App.Model.Layers[App.Model.Index].Opacity);
                        break;
                    case UndeoType.Blend:
                        undo.BlendInstantiation(App.Model.Index, App.Model.Layers[App.Model.Index].BlendIndex);
                        break;

                    case UndeoType.Tool:
                        undo.ToolInstantiation(App.Model.Tool);
                        break;

                    default: break;
                }
                App.Model.Undos.Add(undo);
            }

            if (App.Model.Undos.Count > 2 && App.Model.UndoIndex >= 0)
            {
                App.Model.Undos[App.Model.UndoIndex].Perform();//后退      
                App.Model.UndoIndex--;//撤销索引后退一步

                App.Model.isRedo = true;//重做可用
            }
            else App.Model.isUndo = false;//撤销不可用
        }

        private void RedoButton_Tapped(object sender, TappedRoutedEventArgs e) { Redo(); }
        private void Redo()
        {
            if (App.Model.Undos.Count > 1 && App.Model.UndoIndex + 1 < App.Model.Undos.Count)//索引在撤销列范围内
            {
                App.Model.UndoIndex++;//撤销索引后退一步
                App.Model.Undos[App.Model.UndoIndex].Perform();//前进     

                App.Model.isUndo = true;//撤销可用
            }
            else App.Model.isRedo = false;//重做不可用
        }


        #endregion

        #region Key：键盘


        private void page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (App.Model.StartVisibility == Visibility.Collapsed)
            {
                if (e.Key == VirtualKey.Control) App.Model.isCtrl = true;
                if (e.Key == VirtualKey.Shift) App.Model.isShift = true;

                if (App.Model.isCtrl == true) KeyCtrl(e.Key);
                else if (App.Model.isCtrl == false)
                {
                    Key(e.Key);
                    KeyDirection(e.Key);
                }
            }
        }

        private void page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (App.Model.StartVisibility == Visibility.Collapsed)
            {
                if (e.Key == VirtualKey.Control) App.Model.isCtrl = false;
                if (e.Key == VirtualKey.Shift) App.Model.isShift = false;
            }
        }


         


        

        //方向键
        private void KeyDirection(VirtualKey value)
        {
            int t = App.Model.Tool;

            if (t >= 0 && t < 100 || t == -4)//Tool：工具
            {
                switch (value)
                {
                    //上下切换图层
                    case VirtualKey.Up: App.Model.Index = App.Model.Index - 1 >= 0 ? App.Model.Index - 1 : 0; break;
                    case VirtualKey.Down: App.Model.Index = App.Model.Index + 1 <= App.Model.Layers.Count - 1 ? App.Model.Index + 1 : App.Model.Layers.Count - 1; break;
                    //左右切换工具
                    case VirtualKey.Left: App.Model.Tool = App.Model.Tool - 1 >= 0 ? App.Model.Tool - 1 : 0; break;
                    case VirtualKey.Right: App.Model.Tool = App.Model.Tool + 1 <= 15 ? App.Model.Tool + 1 : 15; break;
                    default: break;
                }
            }
            //Layer：图片  //Mask：粘贴   //Transform：变换     //Transform：变换选区
            else if (t == -1 || t == 102 || t == 111 || t == 308)
            {
                switch (value)
                {
                    case VirtualKey.Up: App.Setting.TransformY--; break;
                    case VirtualKey.Down: App.Setting.TransformY++; break;
                    case VirtualKey.Left: App.Setting.TransformX--; break;
                    case VirtualKey.Right: App.Setting.TransformX++; break;
                    default: break;
                }
                修图.BarPage.OtherPage.Transform.Render();
            }
            else if (t == 300)//Crop：裁切
            {
                switch (value)
                {
                    case VirtualKey.Up: App.Setting.CropY--; break;
                    case VirtualKey.Down: App.Setting.CropY++; break;
                    case VirtualKey.Left: App.Setting.CropX--; break;
                    case VirtualKey.Right: App.Setting.CropX++; break;
                    default: break;
                }
                App.Model.Refresh++;//画布刷新 
            }
            else if (t == 303)//Gradient：渐变
            {
                switch (value)
                {
                    case VirtualKey.Up: App.Setting.GradientStartPoint.Y--; App.Setting.GradientEndPoint.Y--; break;
                    case VirtualKey.Down: App.Setting.GradientStartPoint.Y++; App.Setting.GradientEndPoint.Y++; break;
                    case VirtualKey.Left: App.Setting.GradientStartPoint.X--; App.Setting.GradientEndPoint.X--; break;
                    case VirtualKey.Right: App.Setting.GradientStartPoint.X++; App.Setting.GradientEndPoint.X++; break;
                    default: break;
                }
                修图.BarPage.OtherPage.Gradient.Render();
            }
            else if (t == 304)//Fade：渐变
            {
                switch (value)
                {
                    case VirtualKey.Up: App.Setting.FadeStartPoint.Y--; App.Setting.FadeEndPoint.Y--; break;
                    case VirtualKey.Down: App.Setting.FadeStartPoint.Y++; App.Setting.FadeEndPoint.Y++; break;
                    case VirtualKey.Left: App.Setting.FadeStartPoint.X--; App.Setting.FadeEndPoint.X--; break;
                    case VirtualKey.Right: App.Setting.FadeStartPoint.X++; App.Setting.FadeEndPoint.X++; break;
                    default: break;
                }
                修图.BarPage.OtherPage.Fade.Render();
            }
            else if (t == 305)//Text：文字 
            {
                switch (value)
                {
                    case VirtualKey.Up: App.Setting.TextStartPoint.Y--; App.Setting.TextEndPoint.Y--; break;
                    case VirtualKey.Down: App.Setting.TextStartPoint.Y++; App.Setting.TextEndPoint.Y++; break;
                    case VirtualKey.Left: App.Setting.TextStartPoint.X--; App.Setting.TextEndPoint.X--; break;
                    case VirtualKey.Right: App.Setting.TextStartPoint.X++; App.Setting.TextEndPoint.X++; break;
                    default: break;
                }
                修图.BarPage.OtherPage.Text.Render();
            }
        }

        //快捷键
        private void Key(VirtualKey value)
        {
            int t = App.Model.Tool;

            if (t != 305&&t!= 503)//Tool：工具
            {
                switch (value)
                {
                    //确定 / 取消
                    //  case VirtualKey.Enter: Apply(); break;
                    // case VirtualKey.Escape: Cancel(); break;


                    //放大缩小 左上右下
                    case VirtualKey.Q:
                        if ((App.Model.CanvasWidth > 70 && App.Model.CanvasHeight > 70))//防止画布过小崩溃
                        {
                            App.Model.CanvasWidth *= 1.1;
                            App.Model.CanvasHeight *= 1.1;

                            App.Model.X = (App.Model.GridWidth / 2 - (App.Model.GridWidth / 2 - App.Model.X) * 1.1);
                            App.Model.Y = (App.Model.GridHeight / 2 - (App.Model.GridHeight / 2 - App.Model.Y) * 1.1);
                            App.Model.isReStroke = true;
                            App.Model.Refresh++;
                        }
                        break;
                    case VirtualKey.E:
                        if ((App.Model.CanvasWidth > 70 && App.Model.CanvasHeight > 70))//防止画布过小崩溃
                        {
                            App.Model.CanvasWidth /= 1.1;
                            App.Model.CanvasHeight /= 1.1;

                            App.Model.X = (App.Model.GridWidth / 2 - (App.Model.GridWidth / 2 - App.Model.X) / 1.1);
                            App.Model.Y = (App.Model.GridHeight / 2 - (App.Model.GridHeight / 2 - App.Model.Y) / 1.1);
                            App.Model.isReStroke = true;
                            App.Model.Refresh++;
                        }
                        break;
                    case VirtualKey.A: App.Model.X += 10; App.Model.Refresh++; break;
                    case VirtualKey.D: App.Model.X -= 10; App.Model.Refresh++; break;
                    case VirtualKey.W: App.Model.Y += 10; App.Model.Refresh++; break;
                    case VirtualKey.S: App.Model.Y -= 10; App.Model.Refresh++; break;


                         
                     //慢捷键
                    case VirtualKey.F1:   App.Model.Tool =App.Model.Tool>=0&&App.Model.Tool<10 ? 0:500; break;
                    case VirtualKey.F2: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 1:501; break;
                    case VirtualKey.F3: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 2 : 502; break;
                    case VirtualKey.F4: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 3 : 503; break;
                    case VirtualKey.F5: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 4 : 504; break;
                    case VirtualKey.F6: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 5 : 505; break;
                    case VirtualKey.F7: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 6 : 506; break;
                    case VirtualKey.F8: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 7 : 507; break;
                    case VirtualKey.F9: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 8 : 508; break;
                    case VirtualKey.F10: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 9 : 509; break;
                    case VirtualKey.F11: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 10 : 510; break;
                    case VirtualKey.F12: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 11 : 511; break;
                    case VirtualKey.F13: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 12 : 512; break;
                    case VirtualKey.F14: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 13 : 513; break;
                    case VirtualKey.F15: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 14 : 514; break;
                    case VirtualKey.F16: App.Model.Tool = App.Model.Tool >= 0 && App.Model.Tool < 10 ? 15 : 515; break;




                    default: break;
                }

            }
        }

        //康戳键
        private void KeyCtrl(VirtualKey value)
        {
            switch (value)
            {
                //Undo：撤销
                case VirtualKey.Z: if (App.Model.isUndo) Undo(); break;
                case VirtualKey.Y: if (App.Model.isRedo) Redo(); break;
                case VirtualKey.S:  HomeContentDialog.ShowAsync();  break;

                //Mask：选区
                case VirtualKey.X: 修图.Control.MaskButton.Cut(); break;
                case VirtualKey.C: 修图.Control.MaskButton.Copy(); break;
                case VirtualKey.V:
                     DataPackageView dataPackageView = Clipboard.GetContent();
                    if (dataPackageView.Contains(StandardDataFormats.Bitmap))
                    {
                        IRandomAccessStreamReference imageReceived = null;
                        imageReceived = dataPackageView.GetBitmapAsync().AsTask().Result;
                        if (imageReceived != null)
                        {
                            using (IRandomAccessStream stream = imageReceived.OpenReadAsync().AsTask().Result)
                            {
                                App.Model.SecondCanvasBitmap = CanvasVirtualBitmap.LoadAsync(App.Model.VirtualControl, stream).AsTask().Result;
                                App.InitializeOther();
                                LayerControl_Layers();
                            }
                        }
                    }
                    else if (App.Model.isClipboard == true)
                    {

                        App.InitializeEffect();
                        MaskButton_Mask(102);
                    }

                    break;
                case VirtualKey.Delete: 修图.Control.MaskButton.Clear(); break;

                case VirtualKey.E: 修图.Control.MaskButton.Extract(); break;
                case VirtualKey.M: 修图.Control.MaskButton.Merge(); break;

                case VirtualKey.A: 修图.Control.MaskButton.All(); break;
                case VirtualKey.D: 修图.Control.MaskButton.Deselect(); break;
                case VirtualKey.P: 修图.Control.MaskButton.Pixel(); break;
                case VirtualKey.I: 修图.Control.MaskButton.Invert(); break;

                case VirtualKey.F: MaskButton_Mask(110); break;
                case VirtualKey.T: MaskButton_Mask(111); break;
            }
        }


        #endregion



        #region Home：回家按钮


        private async void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //如果操作次数大于2，显示消息框，否则直接退回
            //   if (App.Model.Refresh>2)
            await HomeContentDialog.ShowAsync();
            //    else
            //        Frame.GoBack();
        }
        private async void HomeContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            HomeContentDialog.Hide();
            LoadingControl.IsLoading = true;//Con：加载控件
            await Task.Delay(400);

            //1、创建一个XDocument对象  
            XDocument xDoc = new XDocument();
            XDeclaration XDec = new XDeclaration("1.0", "utf-8", "no");
            //设置xml的文档定义  
            xDoc.Declaration = XDec;


            //2、创建根节点  
            XElement root = new XElement("Layers");
            xDoc.Add(root);

            XElement Width = new XElement("Width", App.Model.Width);
            root.Add(Width);
            XElement Height = new XElement("Height", App.Model.Height);
            root.Add(Height);
            XElement Tool = new XElement("Tool", App.Model.Tool);
            root.Add(Tool);
            XElement Index = new XElement("Index", App.Model.Index);
            root.Add(Index);


            //3、创建主图片 
            ICanvasImage ci = App.Model.NullRenderTarget;
            for (int i = App.Model.Layers.Count - 1; i >= 0; i--)//自下而上渲染
            {
                ci = App.RenderTransform(App.Model.Layers[i], ci);//渲染
            }
            using (CanvasDrawingSession ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Color.FromArgb(0,0,0,0));
                ds.DrawImage(ci);
            }
            var MainBytes = App.Model.SecondBottomRenderTarget.GetPixelBytes();//把位图转为byte[]
            var MainString = Convert.ToBase64String(MainBytes);//把btye[]转为字符串
            XElement MainCanvasRender = new XElement("MainRenderTarget", MainString);//字符串写入xml节点
            root.Add(MainCanvasRender);


            //4、循环创建节点  
            foreach (var l in App.Model.Layers)
            {
                XElement Layer = new XElement("Layer");
                root.Add(Layer);

                //4、创建元素
                XElement LayerName = new XElement("LayerName", l.Name);
                Layer.Add(LayerName);
                XElement LayerVisual = new XElement("LayerVisual", l.Visual);
                Layer.Add(LayerVisual);
                XElement LayerOpacity = new XElement("LayerOpacity", l.Opacity);
                Layer.Add(LayerOpacity);
                XElement LayerBlendIndex = new XElement("LayerBlendIndex", l.BlendIndex);
                Layer.Add(LayerBlendIndex);

                XElement LayerWidth = new XElement("LayerWidth", l.CanvasRenderTarget.SizeInPixels.Width);
                Layer.Add(LayerWidth);
                XElement LayerHeight = new XElement("LayerHeight", l.CanvasRenderTarget.SizeInPixels.Height);
                Layer.Add(LayerHeight);

                var Bytes = l.CanvasRenderTarget.GetPixelBytes();//把位图转为byte[]
                var str = Convert.ToBase64String(Bytes);//把btye[]转为字符串
                XElement CanvasRenderTarget = new XElement("CanvasRenderTarget", str);//字符串写入xml节点
                Layer.Add(CanvasRenderTarget);
            }


            //5、保存
            string path = ApplicationData.Current.LocalFolder.Path + "/" + App.Model.Name + ".photo"; //将XML文件加载进来
            xDoc.Save(path);


            //6、缩略图 （裁切成宽高最大256的图片）
           

            //缩略图缩放比例
            float scale = App.Model.Width < App.Model.Height ? 256.0f / App.Model.Width : 256.0f / App.Model.Height;

            //缩放后宽高并确定左右上下偏移
            float W = scale * App.Model.Width;
            float H = scale * App.Model.Height;

            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, W, H);
            using (CanvasDrawingSession ds = crt.CreateDrawingSession())
            {
                //绘制缩略图
                ds.DrawImage(new ScaleEffect
                {
                    Source = App.Model.MainRenderTarget,
                    Scale = new Vector2(scale)
                });
            }
            Library.Image.SavePng(ApplicationData.Current.LocalFolder,crt,App.Model.Name, CreationCollisionOption.ReplaceExisting);

            LoadingControl.IsLoading = false;//Con：加载控件

            // Frame.GoBack();
            App.Model.StartVisibility = Visibility.Visible;
        }

        private void HomeContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
             HomeContentDialog.Hide();
             //Frame.GoBack();
            App.Model.StartVisibility = Visibility.Visible;
        }

        private void HomeContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            HomeContentDialog.Hide();
        }





        #endregion

   
    }
}
