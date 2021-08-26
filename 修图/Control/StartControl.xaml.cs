using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.System;
using Windows.Storage;
using Microsoft.Graphics.Canvas;
using System.Xml.Linq;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Windows.ApplicationModel.DataTransfer;
using System.Net.Http;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using 修图.Model;
using 修图.Library;

namespace 修图.Control
{
    public sealed partial class StartControl : UserControl
    {

        #region DependencyProperty：依赖属性

        public Visibility StartVisibility
        {
            set { SetValue(StartVisibilityProperty, value); }
        }
        public static readonly DependencyProperty StartVisibilityProperty =
            DependencyProperty.Register("StartVisibility", typeof(Visibility), typeof(StartControl), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(StartVisibilityOnChang)));
        private static void StartVisibilityOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            StartControl Con = (StartControl)sender;
            Con.Grid.Visibility = (Visibility)e.NewValue;


            if ((Visibility)e.NewValue == Visibility.Visible)
            {
                Con.Grid.Visibility = Visibility.Visible;
                Con.GridViewLoaded();
            }

            else if ((Visibility)e.NewValue == Visibility.Collapsed)
            {
                Con.Grid.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        public StartControl()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        }


        #region Global：全局


        private void page_Loaded(object sender, RoutedEventArgs e)
        {
            //File：文件关联
            if (App.StorageItem != null)
            {
                Pictures((StorageFile)App.StorageItem);
            }
        }


        private void RefreshButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GridViewLoaded();//GridView：刷新
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
                        Pictures(items.Single() as StorageFile);//Pictures
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


        #region  Main：全局判断


        private void MainTop()//Main：Top为空白
        {
            //新建层
            CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, App.Model.Width, App.Model.Height);
            Layer l = new Layer { CanvasRenderTarget = crt };
            if (App.Model.isLowView) l.LowView();
            else l.SquareView();
            l.SetWriteableBitmap(App.Model.VirtualControl);

            //格式化
            App.Formatting(l);
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

        private void MainBottom(XDocument xdoc)//Main：Bottom为项目文档
        {
            //格式化
            App.Formatting();

            //读取数据
            var dataInfo = from a in xdoc.Descendants("Layer")
                           select new
                           {
                               LayerName = a.Element("LayerName").Value,
                               LayerVisual = a.Element("LayerVisual"),
                               LayerOpacity = a.Element("LayerOpacity"),
                               LayerBlendIndex = a.Element("LayerBlendIndex"),
                               LayerWidth = a.Element("LayerWidth"),
                               LayerHeight = a.Element("LayerHeight"),
                               CanvasRenderTarget = a.Element("CanvasRenderTarget").Value,
                           };

            //新建图层
            foreach (var date in dataInfo)
            {
                //新建渲染目标
                CanvasRenderTarget crt = new CanvasRenderTarget(App.Model.VirtualControl, (int)date.LayerWidth, (int)date.LayerHeight);
                string Target = date.CanvasRenderTarget;
                var bytes = Convert.FromBase64String(Target);
                crt.SetPixelBytes(bytes);

                //新建层
                Layer l = new Layer
                {
                    Name = date.LayerName,
                    Visual = (bool)date.LayerVisual,
                    Opacity = (int)date.LayerOpacity,
                    BlendIndex = (int)date.LayerBlendIndex,
                    CanvasRenderTarget = crt
                };
                if (App.Model.isLowView) l.LowView();
                else l.SquareView();
                l.SetWriteableBitmap(App.Model.VirtualControl);//设置缩略图

                App.Model.Layers.Add(l);
            }


            //属性
            App.Model.Tool = (int)xdoc.Descendants("Tool").Single();
            App.Model.Index = (int)xdoc.Descendants("Index").Single();
        }

        #endregion



        #region GridView：网格视图


        PhotoFile AddPhotoFile; //Blank：空白添加

        ObservableCollection<PhotoFile> PhotoFileList = new ObservableCollection<PhotoFile>() { };

        //加载完成
        private async void AdaptiveGridViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Blank：空白添加
            AddPhotoFile = new PhotoFile()
            {
                Name = App.resourceLoader.GetString("/Home/Blank_"),
                Describe = App.resourceLoader.GetString("/Home/BlankDescribe_"),
                Uri = new Uri("ms-appx:///Icon/Clutter/Add.png"),
                isAdd = Visibility.Visible
            };

            AdaptiveGridViewControl.ItemsSource = PhotoFileList;
            GridViewLoaded();
        }
        private async void GridViewLoaded()
        {
            PhotoFileList.Clear();
            IReadOnlyList<StorageFile> FileList = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            string FolderPath = ApplicationData.Current.LocalFolder.Path;

            IOrderedEnumerable<StorageFile> list = FileList.OrderByDescending(i => i.DateCreated);//文件夹按照时间排序
            foreach (StorageFile flie in list)
            {
                if (flie.FileType == ".photo")
                {
                    DateTimeOffset time = flie.DateCreated;
                    //添加
                    PhotoFileList.Add(new PhotoFile()
                    {
                        Name = flie.DisplayName,
                        Time = time,
                        Describe = time.Year.ToString() + "." + time.Month.ToString() + "." + time.Day.ToString(),//+ "." + time.Hour.ToString() + "." + time.Minute.ToString() + "." + time.Second.ToString(),
                        Uri = new Uri(FolderPath + "/" + flie.DisplayName + ".png", UriKind.Relative),
                        Path = flie.Path,
                    });
                }
            }

            PhotoFileList.Add(AddPhotoFile);//空白添加     
        }

        //点击进入
        private void AdaptiveGridViewControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            PhotoFile pf = e.ClickedItem as PhotoFile;

            if (pf.isAdd == Visibility.Visible)
            {
                AddContentDialog.ShowAsync();//Add
            }
            else
            {
                try
                {
                    var xdoc = XDocument.Load(pf.Path);

                    App.Model.Name = pf.Name;//重命名

                    //宽高
                    App.Model.Width = (int)xdoc.Descendants("Width").Single();
                    App.Model.Height = (int)xdoc.Descendants("Height").Single();

                    MainBottom(xdoc);

                    App.Model.StartVisibility = Visibility.Collapsed;
                }
                catch (Exception)
                {
                }
            }
        }

        //选择变化
        private void AdaptiveGridViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int Count = AdaptiveGridViewControl.SelectedItems.Count;
            App.Model.Text = Count.ToString(); //Second：标题栏文字

            if (Count > 0)
                App.Model.isMain = true;//Main：按钮可用
            else
                App.Model.isMain = false;//Main：按钮可用
        }




        #endregion

        #region MentItem：右键菜单


        PhotoFile RightPhotoFile;
        private void AdaptiveGridViewControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            AdaptiveGridView agv = sender as AdaptiveGridView;

            if (agv.SelectionMode == ListViewSelectionMode.None)//GridView：多选
            {
                if (e.OriginalSource is FrameworkElement element)//获取控件元素
                {
                    if (element.DataContext is PhotoFile file)
                    {
                        if (file.isAdd == Visibility.Collapsed)
                        {
                            RightPhotoFile = file;

                            Point p = e.GetPosition(agv);
                            GridView_MenuFlyout.ShowAt(agv, p);//显示
                        }
                    }
                }
            }
        }

        private void RenameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RenameContentDialog.ShowAsync();//Rename

            var pf = AdaptiveGridViewControl.SelectedItems.Last() as PhotoFile;
            if (pf.isAdd == Visibility.Collapsed)
            {
                RenameText.Text = pf.Name;
            }
        }


        private void SaveMenuItemJPEG_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                string Name = RightPhotoFile.Name;

                //宽高
                int Width = (int)XDoc.Descendants("Width").Single();
                int Height = (int)XDoc.Descendants("Height").Single();

                //读取数据
                string Target = XDoc.Descendants("MainRenderTarget").Single().Value;
                byte[] bytes = Convert.FromBase64String(Target);

                //保存图片 
                App.Model.SecondTopRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                App.Model.SecondTopRenderTarget.SetPixelBytes(bytes);
                App.Model.SecondBottomRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                using (var ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
                {
                    ds.Clear(Colors.White);
                    ds.DrawImage(App.Model.SecondTopRenderTarget);
                }
                修图.Library.Image.SaveJpeg(KnownFolders.SavedPictures, App.Model.SecondBottomRenderTarget, Name);

                string path = "Icon/Clutter/OK.png";
                修图.Library.Toast.ShowToastNotification(path, "已保存到本地相册");
            }
        }
        private void SaveMenuItemPNG_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                修图.Library.Image.SavePng(KnownFolders.SavedPictures, Convert.FromBase64String(XDoc.Descendants("MainRenderTarget").Single().Value), (int)XDoc.Descendants("Width").Single(), (int)XDoc.Descendants("Height").Single(), RightPhotoFile.Name);
                修图.Library.Toast.ShowToastNotification("Icon/Clutter/OK.png", "已保存到本地相册");
            }
        }
        private void SaveMenuItemBMP_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                修图.Library.Image.SaveBmp(KnownFolders.SavedPictures, Convert.FromBase64String(XDoc.Descendants("MainRenderTarget").Single().Value), (int)XDoc.Descendants("Width").Single(), (int)XDoc.Descendants("Height").Single(), RightPhotoFile.Name);
                修图.Library.Toast.ShowToastNotification("Icon/Clutter/OK.png", "已保存到本地相册");
            }
        }
        private void SaveMenuItemGIF_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                修图.Library.Image.SaveGif(KnownFolders.SavedPictures, Convert.FromBase64String(XDoc.Descendants("MainRenderTarget").Single().Value), (int)XDoc.Descendants("Width").Single(), (int)XDoc.Descendants("Height").Single(), RightPhotoFile.Name);
                修图.Library.Toast.ShowToastNotification("Icon/Clutter/OK.png", "已保存到本地相册");
            }
        }
        private void SaveMenuItemTIFF_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                修图.Library.Image.SaveTiff(KnownFolders.SavedPictures, Convert.FromBase64String(XDoc.Descendants("MainRenderTarget").Single().Value), (int)XDoc.Descendants("Width").Single(), (int)XDoc.Descendants("Height").Single(), RightPhotoFile.Name);
                修图.Library.Toast.ShowToastNotification("Icon/Clutter/OK.png", "已保存到本地相册");
            }
        }


        private void ShareMenuItemJPEG_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                //删光临时文件夹
                IEnumerable<StorageFile> DelelteFiles = ApplicationData.Current.TemporaryFolder.GetFilesAsync().AsTask().Result;
                foreach (var file in DelelteFiles)
                {
                    file.DeleteAsync();
                }

                //遍历网格列表
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                string Name = RightPhotoFile.Name;

                //宽高
                int Width = (int)XDoc.Descendants("Width").Single();
                int Height = (int)XDoc.Descendants("Height").Single();

                //读取数据
                string Target = XDoc.Descendants("MainRenderTarget").Single().Value;
                byte[] bytes = Convert.FromBase64String(Target);

                //保存图片 
                App.Model.SecondTopRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                App.Model.SecondTopRenderTarget.SetPixelBytes(bytes);
                App.Model.SecondBottomRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                using (var ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
                {
                    ds.Clear(Colors.White);
                    ds.DrawImage(App.Model.SecondTopRenderTarget);
                }
                修图.Library.Image.SaveJpeg(ApplicationData.Current.TemporaryFolder, App.Model.SecondBottomRenderTarget, Name);

                //Share
                dataTransferManager.DataRequested += OnDataRequested;
                DataTransferManager.ShowShareUI();
            }
        }
        private void ShareMenuItemPNG_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                //删光临时文件夹
                foreach (var file in ApplicationData.Current.TemporaryFolder.GetFilesAsync().AsTask().Result) file.DeleteAsync();

                //遍历网格列表
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                修图.Library.Image.SavePng(ApplicationData.Current.TemporaryFolder, Convert.FromBase64String(XDoc.Descendants("MainRenderTarget").Single().Value), (int)XDoc.Descendants("Width").Single(), (int)XDoc.Descendants("Height").Single(), RightPhotoFile.Name);

                //Share
                dataTransferManager.DataRequested += OnDataRequested;
                DataTransferManager.ShowShareUI();
            }
        }
        private void ShareMenuItemBMP_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                //删光临时文件夹
                foreach (var file in ApplicationData.Current.TemporaryFolder.GetFilesAsync().AsTask().Result) file.DeleteAsync();

                //遍历网格列表
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                修图.Library.Image.SaveBmp(ApplicationData.Current.TemporaryFolder, Convert.FromBase64String(XDoc.Descendants("MainRenderTarget").Single().Value), (int)XDoc.Descendants("Width").Single(), (int)XDoc.Descendants("Height").Single(), RightPhotoFile.Name);

                //Share
                dataTransferManager.DataRequested += OnDataRequested;
                DataTransferManager.ShowShareUI();
            }
        }
        private void ShareMenuItemGIF_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                //删光临时文件夹
                foreach (var file in ApplicationData.Current.TemporaryFolder.GetFilesAsync().AsTask().Result) file.DeleteAsync();

                //遍历网格列表
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                修图.Library.Image.SaveGif(ApplicationData.Current.TemporaryFolder, Convert.FromBase64String(XDoc.Descendants("MainRenderTarget").Single().Value), (int)XDoc.Descendants("Width").Single(), (int)XDoc.Descendants("Height").Single(), RightPhotoFile.Name);

                //Share
                dataTransferManager.DataRequested += OnDataRequested;
                DataTransferManager.ShowShareUI();
            }
        }
        private void ShareMenuItemTIFF_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                //删光临时文件夹
                foreach (var file in ApplicationData.Current.TemporaryFolder.GetFilesAsync().AsTask().Result) file.DeleteAsync();

                //遍历网格列表
                var XDoc = XDocument.Load(RightPhotoFile.Path);
                修图.Library.Image.SaveTiff(ApplicationData.Current.TemporaryFolder, Convert.FromBase64String(XDoc.Descendants("MainRenderTarget").Single().Value), (int)XDoc.Descendants("Width").Single(), (int)XDoc.Descendants("Height").Single(), RightPhotoFile.Name);

                //Share
                dataTransferManager.DataRequested += OnDataRequested;
                DataTransferManager.ShowShareUI();
            }
        }


        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                var NewName = Named(RightPhotoFile.Name);
                //复制文件
                var xDoc = XDocument.Load(RightPhotoFile.Path);
                string path = ApplicationData.Current.LocalFolder.Path + "/" + NewName + ".photo"; //将XML文件加载进来
                xDoc.Save(path);


                //6、缩略图 （裁切成宽高最大256的图片）

                //宽高
                int Width = (int)xDoc.Descendants("Width").Single();
                int Height = (int)xDoc.Descendants("Height").Single();

                //渲染目标
                string Target = xDoc.Descendants("MainRenderTarget").Single().Value;
                byte[] bytes = Convert.FromBase64String(Target); App.Model.MainRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                App.Model.MainRenderTarget.SetPixelBytes(bytes);

                //缩略图缩放比例
                float scale = Width < Height ? 256.0f / Width : 256.0f / Height;

                //缩放后宽高并确定左右上下偏移
                float W = scale * Width;
                float H = scale * Height;

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
                Library.Image.SavePng(ApplicationData.Current.LocalFolder, crt, NewName);

                GridViewLoaded();
            }
            RightPhotoFile = null;
        }

        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (RightPhotoFile != null)
            {
                //遍历，删除
                IReadOnlyList<StorageFile> FileList = await ApplicationData.Current.LocalFolder.GetFilesAsync();

                foreach (StorageFile file in FileList)
                {
                    //删除右键文件
                    if (file.DisplayName == RightPhotoFile.Name)
                        await file.DeleteAsync();
                }

                //移除右键项目
                PhotoFileList.Remove(RightPhotoFile);
            }
            RightPhotoFile = null;
        }


        #endregion

        #region TextSreach：文字搜索


        //文字变化
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var matchingContacts = GetMatchingContacts(sender.Text);

                sender.ItemsSource = matchingContacts.ToList();
            }
        }

        public IEnumerable<PhotoFile> GetMatchingContacts(string query)
        {
            return PhotoFileList
                .Where(c => c.Name.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) > -1)
                .OrderByDescending(c => c.Name.StartsWith(query, StringComparison.CurrentCultureIgnoreCase));
        }
        //查询提交
        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                PhotoFile pf = args.ChosenSuggestion as PhotoFile;

                if (pf.isAdd == Visibility.Visible)
                {
                    AddContentDialog.ShowAsync();//Add
                }
                else
                {
                    string path = pf.Path;
                    var xdoc = XDocument.Load(path);

                    MainBottom(xdoc);
                    App.Model.StartVisibility = Visibility.Collapsed;
                }
            }
            else
            {
                // Do a fuzzy search on the query text
                //模糊搜索查询文本吗
                //    var matchingContacts = GetMatchingContacts(args.QueryText);

                // Choose the first match, or clear the selection if there are no matches.
                //选择第一个匹配,或明确的选择如果没有匹配。
                //SelectContact(matchingContacts.FirstOrDefault());
            }
        }



        #endregion



        #region Name：命名


        //返回一个不重名的字符串，如果重名就后缀+1
        private string Named(string s)
        {
            bool isContian = false;
            foreach (var photo in PhotoFileList)
            {
                if (string.Equals(photo.Name, s))
                {
                    isContian = true;
                    break;
                }
            }
            if (isContian == true) return ReName(s, 1);
            else return s;
        }
        //传递字符串和数字，不断递归使得后缀+1
        private string ReName(string s, int n)
        {
            bool isContian = false;
            string sn = s + n.ToString();
            foreach (var photo in PhotoFileList)
            {
                if (string.Equals(photo.Name, sn))
                {
                    isContian = true;
                    break;
                }
            }
            if (isContian == true) return ReName(s, n + 1);
            else return sn;
        }


        #endregion

        #region Notification：应用通知


        private void InAppNotificationShow(InAppNotification inAppNotification, ListViewSelectionMode mode = ListViewSelectionMode.Multiple)//Notification
        {
            inAppNotification.Show();

            AdaptiveGridViewControl.IsItemClickEnabled = false;//不可点击
            AdaptiveGridViewControl.SelectionMode = mode;//多选状态

            PhotoFileList.Remove(AddPhotoFile);//空白添加：移除
        }

        private void InAppNotificationDismiss(InAppNotification inAppNotification)//Notification
        {
            inAppNotification.Dismiss();

            AdaptiveGridViewControl.IsItemClickEnabled = true;//可点击
            AdaptiveGridViewControl.SelectionMode = ListViewSelectionMode.None;//无选状态

            PhotoFileList.Add(AddPhotoFile);//空白添加：添加
        }


        #endregion


        #region Rename


        private void RenameContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var pf = AdaptiveGridViewControl.SelectedItems.Last() as PhotoFile;
            if (pf.isAdd == Visibility.Collapsed)
            {
                pf.Name = RenameText.Text;

                //文件改名
                StorageFile file = StorageFile.GetFileFromPathAsync(pf.Path).AsTask().Result;
                file.RenameAsync(RenameText.Text);
                pf.Path = file.Path;
            }
        }


        #endregion

        #region Add


        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            AddContentDialog.ShowAsync();//Add
        }



        private void AddWidthNumberPicker_Loaded(object sender, RoutedEventArgs e)
        {
            AddWidthNumberPicker.Value = App.Model.Width;
        }
        private void AddWidthNumberPicker_ValueChange(object sender, int Value)
        {
            App.Model.Width = Value;
        }
        private void AddHeighNumberPicker_Loaded(object sender, RoutedEventArgs e)
        {
            AddHeighNumberPicker.Value = App.Model.Height;
        }
        private void AddHeightNumberPicker_ValueChange(object sender, int Value)
        {
            App.Model.Height = Value;
        }



        private void AddContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Name：名称
            string name = App.resourceLoader.GetString("/Home/Untitled_");//多语言适配，英语用“Untitled”，中文用“未命名”
            App.Model.Name = Named(name);

            MainTop();
            App.Model.StartVisibility = Visibility.Collapsed;
        }


        #endregion

        #region  Pictures


        private void PicturesFlyoutx_Opened(object sender, object e)
        {

        }
        private async void PicturesPicturesButton_Tapped(object sender, TappedRoutedEventArgs e)
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

            if (file != null && App.Model.VirtualControl.Device != null)
            {
                Pictures(file);//Pictures
            }
        }
        private async void PicturesDestopButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //文件选择器
            FileOpenPicker openPicker = new FileOpenPicker();
            //选择视图模式
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            //初始位置
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            //添加文件类型
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            StorageFile file = await openPicker.PickSingleFileAsync();//打开选择器

            if (file != null && App.Model.VirtualControl.Device != null)
            {
                Pictures(file);//Pictures
            }
        }




        private async void Pictures(StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                App.Model.Name = Named(file.DisplayName);//Name：名称

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



        #endregion

        #region Save：保存


        private void SaveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            InAppNotificationShow(SaveInAppNotification);//Notification
        }
        private void SaveCancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            InAppNotificationDismiss(SaveInAppNotification);//Notification
        }

        //保存索引：JPG PNG GIF BMP TIFF
        int SaveIndex = 0;
        private void SaveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveIndex = (sender as ComboBox).SelectedIndex;
        }
        private async void SaveSaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LoadingControl.IsLoading = true;//Con：加载控件

            await Save();

            InAppNotificationDismiss(SaveInAppNotification);//Notification

            LoadingControl.IsLoading = false;//Con：加载控件

            string path = "Icon/Clutter/OK.png";
            修图.Library.Toast.ShowToastNotification(path, "已保存到本地相册");
        }

        private async Task Save()
        {

            foreach (Object photoFile in AdaptiveGridViewControl.SelectedItems)
            {
                PhotoFile pf = photoFile as PhotoFile;
                var XDoc = XDocument.Load(pf.Path);

                string Name = pf.Name;

                //宽高
                int Width = (int)XDoc.Descendants("Width").Single();
                int Height = (int)XDoc.Descendants("Height").Single();

                //读取数据
                string Target = XDoc.Descendants("MainRenderTarget").Single().Value;
                byte[] bytes = Convert.FromBase64String(Target);

                //保存图片 
                switch (SaveIndex)
                {
                    case 0:
                        App.Model.SecondTopRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                        App.Model.SecondTopRenderTarget.SetPixelBytes(bytes);
                        App.Model.SecondBottomRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                        using (var ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
                        {
                            ds.Clear(Colors.White);
                            ds.DrawImage(App.Model.SecondTopRenderTarget);
                        }
                        await 修图.Library.Image.SaveJpeg(KnownFolders.SavedPictures, App.Model.SecondBottomRenderTarget, Name);
                        break;
                    case 1: await 修图.Library.Image.SavePng(KnownFolders.SavedPictures, bytes, Width, Height, Name); break;
                    case 2: await 修图.Library.Image.SaveBmp(KnownFolders.SavedPictures, bytes, Width, Height, Name); break; ;
                    case 3: await 修图.Library.Image.SaveGif(KnownFolders.SavedPictures, bytes, Width, Height, Name); break;
                    case 4: await 修图.Library.Image.SaveTiff(KnownFolders.SavedPictures, bytes, Width, Height, Name); break;
                    default: break;
                }
            }
        }

        #endregion

        #region Share


        private void ShareAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            InAppNotificationShow(ShareInAppNotification);//Notification
        }
        private void ShareCancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            InAppNotificationDismiss(ShareInAppNotification);//Notification
        }






        //分享索引：JPG PNG GIF BMP TIFF
        int ShareIndex = 0;
        private void ShareComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShareIndex = (sender as ComboBox).SelectedIndex;
        }






        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();

        private async void ShareShareButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //删光临时文件夹
            IEnumerable<StorageFile> DelelteFiles = ApplicationData.Current.TemporaryFolder.GetFilesAsync().AsTask().Result;
            foreach (var file in DelelteFiles)
            {
                await file.DeleteAsync();
            }


            //遍历网格列表
            foreach (Object photoFile in AdaptiveGridViewControl.SelectedItems)
            {
                PhotoFile pf = photoFile as PhotoFile;
                var XDoc = XDocument.Load(pf.Path);

                string Name = pf.Name;

                //宽高
                int Width = (int)XDoc.Descendants("Width").Single();
                int Height = (int)XDoc.Descendants("Height").Single();

                //读取数据
                string Target = XDoc.Descendants("MainRenderTarget").Single().Value;
                byte[] bytes = Convert.FromBase64String(Target);

                //保存图片 
                switch (ShareIndex)
                {
                    case 0:
                        App.Model.SecondTopRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                        App.Model.SecondTopRenderTarget.SetPixelBytes(bytes);
                        App.Model.SecondBottomRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                        using (var ds = App.Model.SecondBottomRenderTarget.CreateDrawingSession())
                        {
                            ds.Clear(Colors.White);
                            ds.DrawImage(App.Model.SecondTopRenderTarget);
                        }
                        修图.Library.Image.SaveJpeg(ApplicationData.Current.TemporaryFolder, App.Model.SecondBottomRenderTarget, Name);
                        break;
                    case 1: 修图.Library.Image.SavePng(ApplicationData.Current.TemporaryFolder, bytes, Width, Height, Name); break;
                    case 2: 修图.Library.Image.SaveBmp(ApplicationData.Current.TemporaryFolder, bytes, Width, Height, Name); break; ;
                    case 3: 修图.Library.Image.SaveGif(ApplicationData.Current.TemporaryFolder, bytes, Width, Height, Name); break;
                    case 4: 修图.Library.Image.SaveTiff(ApplicationData.Current.TemporaryFolder, bytes, Width, Height, Name); break;
                    default: break;
                }
            }


            //Share
            dataTransferManager.DataRequested += OnDataRequested;
            DataTransferManager.ShowShareUI();
        }

        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            //Share
            ShareSourceData shareSourceData = new ShareSourceData("  ", "  ");
            shareSourceData.SetStorageItems(ApplicationData.Current.TemporaryFolder.GetFilesAsync().AsTask().Result);
            e.Request.SetData(shareSourceData);
        }



        #endregion

        #region Delete：删除


        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            InAppNotificationShow(DeleteInAppNotification);//Notification
        }
        private void DeleteCancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            InAppNotificationDismiss(DeleteInAppNotification);//Notification
        }
        private async void DeleteDeleteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //把要删除的项添加到删除列
            List<PhotoFile> DeleteList = new List<PhotoFile> { };
            foreach (Object photoFile in AdaptiveGridViewControl.SelectedItems)
                DeleteList.Add(photoFile as PhotoFile);

            //遍历，删除
            IReadOnlyList<StorageFile> FileList = await ApplicationData.Current.LocalFolder.GetFilesAsync();

            foreach (StorageFile file in FileList)
                foreach (PhotoFile item in DeleteList)
                    //删除文件
                    if (file.DisplayName == item.Name)
                        await file.DeleteAsync();

            //移除项目
            foreach (PhotoFile item in DeleteList)
                PhotoFileList.Remove(item);

            DeleteList.Clear();

            InAppNotificationDismiss(DeleteInAppNotification);//Notification
        }


        #endregion

        #region Copy：复制


        private void CopyAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            InAppNotificationShow(CopyInAppNotification);//Notification
        }
        private void CopyCancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            InAppNotificationDismiss(CopyInAppNotification);//Notification
        }
        private async void CopyCopyButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (Object photoFile in AdaptiveGridViewControl.SelectedItems)
            {
                if (photoFile is PhotoFile item)//as转换
                {
                    var NewName = Named(item.Name);
                    //复制文件
                    var xDoc = XDocument.Load(item.Path);
                    string path = ApplicationData.Current.LocalFolder.Path + "/" + NewName + ".photo"; //将XML文件加载进来
                    xDoc.Save(path);


                    //6、缩略图 （裁切成宽高最大256的图片）

                    //宽高
                    int Width = (int)xDoc.Descendants("Width").Single();
                    int Height = (int)xDoc.Descendants("Height").Single();

                    //渲染目标
                    string Target = xDoc.Descendants("MainRenderTarget").Single().Value;
                    byte[] bytes = Convert.FromBase64String(Target); App.Model.MainRenderTarget = new CanvasRenderTarget(App.Model.VirtualControl, Width, Height);
                    App.Model.MainRenderTarget.SetPixelBytes(bytes);

                    //缩略图缩放比例
                    float scale = Width < Height ? 256.0f / Width : 256.0f / Height;

                    //缩放后宽高并确定左右上下偏移
                    float W = scale * Width;
                    float H = scale * Height;

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
                    Library.Image.SavePng(ApplicationData.Current.LocalFolder, crt, NewName);
                }
            }

            Task.Delay(200);
            GridViewLoaded();
            InAppNotificationDismiss(CopyInAppNotification);//Notification
        }

        #endregion



        #region Setting：设置


        private async void LocalAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }

        private void MoneyAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            MoneyContentDialog.ShowAsync();
        }



        private void MoreAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            MoreContentDialog.ShowAsync();
        }



        private void MoreShareAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //Share
            dataTransferManager.DataRequested += MoreOnDataRequested;
            DataTransferManager.ShowShareUI();
        }
        private async void MoreOnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            //Share
            ShareSourceData shareSourceData = new ShareSourceData("  ", "  ");
            shareSourceData.SetText(
                "Retouch Photo\r\n" +
                "——form Windows10 UWP Shop\r\n" +
                " https://www.microsoft.com/store/productId/9N2SVF2769GH");
            e.Request.SetData(shareSourceData);
        }





        private void OpenSourceAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("https://github.com/ysdy44/Retouch-Photo-UWP.git"));
        }


        #endregion

    }
}
