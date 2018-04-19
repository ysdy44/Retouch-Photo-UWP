using System; 
using Windows.Foundation;
 using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Storage;
using Windows.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;

namespace 修图.BarPage.ToolPage
{ 
    public sealed partial class Paint : Page
    {
 
        public Paint()
        {
            this.InitializeComponent();
        }

        #region Global：全局


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Width：宽度
            WidthSlider.Value = Math.Sqrt(App.Setting.PaintWidth)*10; 
           WidthNumberPicker.Value = (int)App.Setting.PaintWidth;
            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;

            //Hard：硬度          
            HardSlider.Value=App.Setting.PaintHard * 100;
            HardNumberPicker.Value = (int)App.Setting.PaintHard * 100;
            HardSlider.ValueChanged += HardSlider_ValueChanged;
            HardNumberPicker.ValueChange += HardNumberPicker_ValueChange;

            // Opacity：不透明度
            OpacitySlider.Value = OpacityNumberPicker.Value = (int)(App.Setting.PaintOpacity * 100);
            OpacitySlider.ValueChanged += OpacitySlider_ValueChanged;
            OpacityNumberPicker.ValueChange += OpacityNumberPicker_ValueChange;

            //Space：间隔
            SpaceSlider.Value = SpaceNumberPicker.Value = (int)(App.Setting.PaintSpace * 100);
            SpaceSlider.ValueChanged += SpaceSlider_ValueChanged;
            SpaceNumberPicker.ValueChange += SpaceNumberPicker_ValueChange;

            Set();
        }

        private void ColorButton_ColorChanged(Color Color)
        {
            Set();
        }


        private void Flyout_Opened(object sender, object e)
        {
            //Width：宽度
            WidthSlider.ValueChanged -= WidthSlider_ValueChanged;
            WidthNumberPicker.ValueChange -= WidthNumberPicker_ValueChange;
            WidthSlider.Value = Math.Sqrt(App.Setting.PaintWidth) * 10;
            WidthNumberPicker.Value = (int)App.Setting.PaintWidth;
            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;

            //Hard：硬度
            HardSlider.ValueChanged -= HardSlider_ValueChanged;
            HardNumberPicker.ValueChange -= HardNumberPicker_ValueChange;
            HardSlider.Value = App.Setting.PaintHard * 100;
            HardNumberPicker.Value = (int)(App.Setting.PaintHard * 100);
            HardSlider.ValueChanged += HardSlider_ValueChanged;
            HardNumberPicker.ValueChange += HardNumberPicker_ValueChange;

            // Opacity：不透明度
            OpacitySlider.ValueChanged -= OpacitySlider_ValueChanged;
            OpacityNumberPicker.ValueChange -= OpacityNumberPicker_ValueChange;
            OpacitySlider.Value = App.Setting.PaintOpacity * 100;
           OpacityNumberPicker.Value = (int)(App.Setting.PaintOpacity * 100);
            OpacitySlider.ValueChanged += OpacitySlider_ValueChanged;
            OpacityNumberPicker.ValueChange += OpacityNumberPicker_ValueChange;

            //Space：间隔
            SpaceSlider.ValueChanged -= SpaceSlider_ValueChanged;
            SpaceNumberPicker.ValueChange -= SpaceNumberPicker_ValueChange;
            SpaceSlider.Value = (App.Setting.PaintSpace * 100);
            SpaceNumberPicker.Value = (int)(App.Setting.PaintSpace * 100);
            SpaceSlider.ValueChanged += SpaceSlider_ValueChanged;
            SpaceNumberPicker.ValueChange += SpaceNumberPicker_ValueChange;
        }


        #endregion


        #region Paint：画笔


        //Width：宽度 
        private void WidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            WidthNumberPicker.ValueChange -= WidthNumberPicker_ValueChange;

            App.Setting.PaintWidth = (float)(e.NewValue* e.NewValue/100);
            WidthNumberPicker.Value = (int)(e.NewValue * e.NewValue / 100);
            Set();

            WidthNumberPicker.ValueChange += WidthNumberPicker_ValueChange;
         }
        private void WidthNumberPicker_ValueChange(object sender, int Value)
        {
            WidthSlider.ValueChanged -= WidthSlider_ValueChanged;

            App.Setting.PaintWidth = Value;
            WidthSlider.Value = Math.Sqrt(App.Setting.PaintWidth)*10;
            Set();

            WidthSlider.ValueChanged += WidthSlider_ValueChanged;
        }



        //Hard：硬度 
        private void HardSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.PaintHard = (float)e.NewValue/100f;
            HardNumberPicker.Value = (int)e.NewValue;
            Set();
        }
        private void HardNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PaintHard = Value / 100f;
            HardSlider.Value = Value;


            Set();
        }



        // Opacity：不透明度 
        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            App.Setting.PaintOpacity = (float)e.NewValue / 100f;
            OpacityNumberPicker.Value = (int)e.NewValue;
            Set();
        }
        private void OpacityNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PaintOpacity = (float)Value / 100f;
            OpacitySlider.Value = Value;
            Set();
        }



        //Space：间隔
        private void SpaceSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            App.Setting.PaintSpace = (float)e.NewValue / 100f;
            SpaceNumberPicker.Value = (int)e.NewValue;
            Set();
        }
        private void SpaceNumberPicker_ValueChange(object sender, int Value)
        {
            App.Setting.PaintSpace = (float)Value / 100f;
       SpaceSlider.Value = Value;
            Set();
        }


        #endregion


        #region Canvas：画布


        Size CanvasSize;

        private void ShowCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasSize = e.NewSize;
        }
        bool isRender = false;
        private void ShowCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (isRender==false)
            {
                isRender = true;

                if (App.Setting.Paint != null)
                {
                    var space = App.Setting.PaintSpace * (float)Math.Sqrt(App.Setting.PaintWidth);
                    for (float x = 10; x < CanvasSize.Width - 10; x += space)
                    {
                        //根据画布的X位置，求Sin高度角度（一条上下弧线）
                        var sinh = x / CanvasSize.Width * Math.PI * 2;
                        float h = 20 * (float)Math.Sin(sinh);//上下浮动

                        //根据画布的X位置，求Sin大小角度（一个上凸曲线）
                        var sins = x / CanvasSize.Width * Math.PI;
                        Vector2 s = new Vector2((float)Math.Sin(sins));//大小浮动
                        ScaleEffect se = new ScaleEffect { Source = App.Setting.PaintShow, Scale = s };

                        args.DrawingSession.DrawImage(se, x, (float)CanvasSize.Height / 2 + h);
                    }
                }
            isRender = false;
            }
        }


        #endregion


        #region Thumbnail：缩略图


        int ThumbnailWidth = 256;
        int ThumbnailHeight = 64;
 
        private void ThumbnailButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CanvasRenderTarget Thumbnail = new CanvasRenderTarget(App.Model.VirtualControl, ThumbnailWidth, ThumbnailHeight);
            using (var ds= Thumbnail.CreateDrawingSession())
            {
                var space = App.Setting.PaintSpace * (float)Math.Sqrt(App.Setting.PaintWidth);
                for (float x = 10; x < ThumbnailWidth - 10; x += space)
                {
                    //根据画布的X位置，求Sin高度角度（一条上下弧线）
                    var sinh = x / ThumbnailWidth * Math.PI * 2;
                    float h = 20 * (float)Math.Sin(sinh);//上下浮动

                    //根据画布的X位置，求Sin大小角度（一个上凸曲线）
                    var sins = x / ThumbnailWidth * Math.PI;
                    Vector2 s = new Vector2((float)Math.Sin(sins));//大小浮动
                    ScaleEffect se = new ScaleEffect { Source = App.Setting.PaintShow, Scale = s };

                   ds.DrawImage(se, x, (float)ThumbnailHeight / 2 + h);
                }
            }

            string ss =       "-" + (App.Setting.PaintWidth).ToString() + "-" + (App.Setting.PaintHard).ToString() + "-" + (App.Setting.PaintOpacity).ToString() + "-" + (App.Setting.PaintSpace).ToString();
            byte[] bytes = Thumbnail.GetPixelBytes();
            修图.Library.Image.SavePng(KnownFolders.SavedPictures, bytes, ThumbnailWidth, ThumbnailHeight, ss);

            string path = "Icon/Clutter/OK.png";
            修图.Library.Toast.ShowToastNotification(path, "已保存到本地相册");


             
            App.Tip(ss);//全局提示

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(ss);
            Clipboard.SetContent(dataPackage);
        }


        #endregion


        private void Set( )
        { 
             if (App.Setting.isPaintBitmap == false)
            {
                App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintWidth, App.Model.Color);
            }
            else
            {
                App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintBitmap, App.Setting.PaintWidth, App.Model.Color);
            }
            ShowCanvas.Invalidate();
        }

        private async void PictureButton_Tapped(object sender, TappedRoutedEventArgs e)
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
                    App.Setting.PaintBitmap = CanvasBitmap.LoadAsync(App.Model.VirtualControl, stream).AsTask().Result;
                 }

            }
        }
    }
}
