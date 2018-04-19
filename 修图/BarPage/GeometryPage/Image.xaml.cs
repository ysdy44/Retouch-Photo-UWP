
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;

namespace 修图.BarPage.GeometryPage
{ 
    public sealed partial class Image : Page
    {
        public Image()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //图片
             App.Setting.Image = await CanvasBitmap.LoadAsync(App.Model.VirtualControl, "Icon/Clutter/GeometryImage.PNG");
            App.Setting.ImageWidth = App.Setting.Image.SizeInPixels.Width;
            App.Setting.ImageHeight = App.Setting.Image.SizeInPixels.Height;
           }


        private async void PickerButton_Tapped(object sender, TappedRoutedEventArgs e)
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
                    App.Setting.Image = await CanvasBitmap.LoadAsync(App.Model.VirtualControl, stream);
                    App.Setting.ImageWidth = App.Setting.Image.SizeInPixels.Width;
                    App.Setting.ImageHeight = App.Setting.Image.SizeInPixels.Height;
                 }
            }
        }




        private void RatioButton_Loaded(object sender, RoutedEventArgs e)
        {
            RatioButton.Checked += RatioButton_Checked;
            RatioButton.Unchecked += RatioButton_Unchecked;
        }
        private void RatioButton_Checked(object sender, RoutedEventArgs e)
        {
            App.Setting.isImageRatio = true;
        }
        private void RatioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Setting.isImageRatio = false;
        }
    }
}
