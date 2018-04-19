using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
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

 
namespace 修图.BarPage.ToolPage
{ 
    public sealed partial class Cursor : Page
    {
        public Cursor()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
         }


        private void recoverButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (var l in App.Model.Layers)
            { 
                    l.Vect.X = 0;
                    l.Vect.Y = 0;
             }

            App.Model.isCursor = false;//光标不可用
            App.Model.isReRender = true;//重新渲染 
            App.Model.Refresh++;//画布刷新
        }



        //栅格化
        private void RasterizeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Rasterize();
        }
        public static void Rasterize()
        {
            foreach (var l in App.Model.Layers)
            {
                l.Rasterize();
                l.SetWriteableBitmap(App.Model.VirtualControl);
                l.isSelected = false;
            }
            App.Tip(App.resourceLoader.GetString("/Tool/CursorRasterized_"));
 
            App.Model.isCursor = false;//光标不可用
            App.Model.isReRender = true;//重新渲染 
            App.Model.Refresh++;//画布刷新
        }
    }
}
