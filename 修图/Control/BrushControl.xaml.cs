using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 修图.Model;
 
namespace 修图.Control
{
    public sealed partial class BrushControl : UserControl
    {
        public BrushControl()
        {
            this.InitializeComponent();
        }


        #region Global：全局
         

        private void ColorPicker_StrawChanged()
        {
            App.Model.StrawVisibility = Visibility.Collapsed;
        }

         private void ColorPicker_ColorChanged(Windows.UI.Color Color, SolidColorBrush Brush)
        {
            if (App.Setting.isPaintBitmap==false)
            {
                App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintWidth, Color);
            }
            else
            {
                App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintBitmap,App.Setting.PaintWidth, Color);
            }
        }



        #endregion


        #region  Mode：笔刷模式


        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BrushList.Clear();

            switch (ModeComboBox.SelectedIndex)
            {
                case 0:
                    foreach (var item in Universal) BrushList.Add(item);
                    break;
                case 1:
                    foreach (var item in Flash) BrushList.Add(item);
                    break;
                case 2:
                    foreach (var item in Splodge) BrushList.Add(item);
                    break;
                case 3:
                    foreach (var item in Scratch) BrushList.Add(item);
                    break;
                default:
                    break;
            }
        }
        readonly IReadOnlyList<PaintBrush> Universal = new List<PaintBrush>
        {
            new PaintBrush{Width = 12,Hard = 1,Opacity = 1,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/00-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/00-12-1-1-0.25.png"),},
            new PaintBrush{Width = 4,Hard = 0.83f,Opacity = 1,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/01-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/01-4-0.83-1-0.25.png"),},
            new PaintBrush{Width = 21.16f,Hard = 0.83f,Opacity = 0.59f,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/02-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/02-21.16-0.83-0.59-0.25.png"),},
            new PaintBrush{Width = 108.16f,Hard = 0.01f,Opacity = 0.18f,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/03-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/03-108.16-0.01-0.18-0.25.png"),},
            new PaintBrush{Width = 20.25f,Hard = 1,Opacity = 0.23f,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/04-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/04-20.25-1-0.23-0.25.png"),},

            new PaintBrush{Width = 210.25f,Hard = 0.01f,Opacity = 0.15f,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/05-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/05-210.25-0.01-0.15-0.25.png"),},
            new PaintBrush{Width = 3.24f,Hard = 1,Opacity = 1,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/06-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/06-3.24-1-1-0.25.png"),},
            new PaintBrush{Width = 11.56f,Hard = 0.01f,Opacity = 1,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/07-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/07-11.56-0.01-1-0.25.png"),},
            new PaintBrush{Width = 27.04f,Hard = 0.34f,Opacity = 1,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/08-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/08-27.04-0.34-1-0.25.png"),},
            new PaintBrush{Width = 75.69f,Hard = 0.34f,Opacity = 0.46f,Space = 0.25f,
                Thumbnail= new Uri("ms-appx:///Icon/Brush/Universal/09-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Universal/09-75.69-0.34-0.46-0.25.png"),},
        };


        readonly IReadOnlyList<PaintBrush> Flash = new List<PaintBrush>
        {
            new PaintBrush{Width = 300,Hard = 0.7f,Opacity = 9,Space = 0.8f,
                isBitmap = true,Bitmap = "Icon/Brush/Flash/00.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Flash/00-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Flash/00-400-0.7-0.9-0.8.png"),},
            new PaintBrush{  Width = 500,Hard = 1,Opacity = 0.85f,Space = 0.55f,
                isBitmap = true,Bitmap = "Icon/Brush/Flash/01.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Flash/01-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Flash/01-500-1-0.85-0.55.png"),},
            new PaintBrush{ Width = 428.49f,Hard = 0.01f,Opacity = 0.66f,Space = 0.6f,
                isBitmap = true,Bitmap = "Icon/Brush/Flash/02.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Flash/02-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Flash/02-428.49-0.01-0.66-0.6.png"),},
            new PaintBrush{ Width = 306.25f,Hard = 0.7f,Opacity = 0.73f,Space = 0.8f,
                isBitmap = true,Bitmap = "Icon/Brush/Flash/03.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Flash/03-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Flash/03-306.25-0.7-0.73-0.8.png"),},
            new PaintBrush{  Width = 1024,Hard = 1,Opacity = 9,Space = 0.3f,
                isBitmap = true,Bitmap = "Icon/Brush/Flash/04.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Flash/04-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Flash/04-1024-1-9-0.3.png"),},
          };


        readonly IReadOnlyList<PaintBrush> Splodge = new List<PaintBrush>
        {
            new PaintBrush{Width =120,Hard = 0,Opacity = 0.7f,Space =0.33f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/00.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/00-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/00-120-0.01-0.7-0.33.png"),},
            new PaintBrush{Width =158.76f,Hard = 0,Opacity = 1,Space =0.22f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/01.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/01-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/01-158.76-0.01-1-0.22.png"),},
            new PaintBrush{Width =150,Hard = 1,Opacity = 1,Space =0.33f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/02.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/02-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/02-150-1-1-0.33.png"),},
            new PaintBrush{Width =136.89f,Hard = 0.27f,Opacity = 0.89f,Space =0.33f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/03.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/03-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/03-136.89-0.27-0.89-0.33.png"),},
            new PaintBrush{Width =86.49f,Hard = 0.09f,Opacity = 0.69f,Space =0.3f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/04.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/04-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/04-86.49-0.09-0.69-0.3.png"),},

            new PaintBrush{Width =108.16f,Hard =0.14f,Opacity = 0.79f,Space =0.4f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/05.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/05-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/05-108.16-0.14-0.79-0.4.png"),},
            new PaintBrush{Width =180,Hard =0.27f,Opacity = 0.89f,Space =0.33f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/06.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/06-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/06-180-0.27-0.89-0.33.png"),},
            new PaintBrush{Width =249.64f,Hard =1,Opacity = 1,Space =0.66f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/07.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/07-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/07-249.64-1-1-0.66.png"),},
            new PaintBrush{Width =272.25f,Hard =0.07f,Opacity = 0.56f,Space =0.4f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/08.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/08-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/08-272.25-0.07-0.56-0.4.png"),},
            new PaintBrush{Width =160,Hard =0.09f,Opacity = 0.84f,Space =0.45f,
                isBitmap = true,Bitmap = "Icon/Brush/Splodge/09.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Splodge/09-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Splodge/09-160-0.09-0.84-0.45.png"),},
         };


        readonly IReadOnlyList<PaintBrush> Scratch = new List<PaintBrush>
        {
            new PaintBrush{Width =222,Hard = 0.8f,Opacity =0.49f,Space =0.45f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/00.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/00-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/00-222-0.8-0.49-0.45.png"),},
            new PaintBrush{Width =233,Hard = 0,Opacity =1,Space =0.33f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/01.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/01-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/01-233-0.01-1-0.33.png"),},
            new PaintBrush{Width =158.76f,Hard = 1,Opacity =1,Space =0.5f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/02.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/02-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/02-158.76-1-1-0.5.png"),},
            new PaintBrush{Width =132.25f,Hard = 0.23f,Opacity =0.21f,Space =0.22f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/03.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/03-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/03-132.25-0.23-0.21-0.22.png"),},
            new PaintBrush{Width =193.21f,Hard = 0.23f,Opacity =0.21f,Space =0.22f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/04.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/04-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/04-193.21-0.23-0.21-0.22.png"),},

            new PaintBrush{Width =225,Hard = 0.87f,Opacity =0.8f,Space =0.45f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/05.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/05-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/05-225-0.87-0.8-0.45.png"),},
            new PaintBrush{Width =112.36f,Hard = 0,Opacity =0.8f,Space =0.66f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/06.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/06-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/06-112.36-0.01-0.8-0.66.png"),},
            new PaintBrush{Width =102,Hard = 0.59f,Opacity =1,Space =0.66f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/07.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/07-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/07-102.01-0.59-1-0.66.png"),},
            new PaintBrush{Width =121,Hard = 0,Opacity =0.58f,Space =0.8f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/08.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/08-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/08-121-0.01-0.58-0.8.png"),},
            new PaintBrush{Width =166.41f,Hard = 0.17f,Opacity =0.41f,Space =0.33f,
                isBitmap = true,Bitmap = "Icon/Brush/Scratch/09.png",
                Thumbnail = new Uri("ms-appx:///Icon/Brush/Scratch/09-.png"),
                Uri = new Uri("ms-appx:///Icon/Brush/Scratch/09-166.41-0.17-0.41-0.33.png"),},
        };


        #endregion


        #region  Brush：笔刷


        ObservableCollection<PaintBrush> BrushList = new ObservableCollection<PaintBrush> { };

        private async void BrushListView_Loaded(object sender, RoutedEventArgs e)
        {
            BrushList.Clear();

            foreach (var item in Universal)
            {
                BrushList.Add(item);//遍历添加
            }
            
             BrushListView.ItemsSource = BrushList;
        }


         //列项点击
        private async void BrushListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is PaintBrush)
            {
                 PaintBrush pb = e.ClickedItem as PaintBrush;

                //画笔四大参数
                App.Setting.PaintWidth = pb.Width;
                App.Setting.PaintHard = pb.Hard;
                App.Setting.PaintOpacity = pb.Opacity;
                App.Setting.PaintSpace = pb.Space;

                //如果不是位图
                if (pb.isBitmap == false)
                {
                    App.Setting.isPaintBitmap = false;
                    App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintWidth, App.Model.Color);
                }
                else if (pb.isBitmap == true)
                {
                    App.Setting.isPaintBitmap = true;
                    App.Setting.PaintBitmap = await CanvasBitmap.LoadAsync(App.Model.VirtualControl, pb.Bitmap);
                    App.Setting.PaintSet(App.Model.VirtualControl, App.Setting.PaintBitmap, App.Setting.PaintWidth, App.Model.Color);
                }
            }
        }


        #endregion
 

    }
}
