using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Threading.Tasks;

namespace 修图
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;
        } 
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
           // await (StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Icon/XDocument/Logo.photo")).AsTask().Result).CopyAsync(ApplicationData.Current.LocalFolder);
           // await (StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Icon/XDocument/Logo.png")).AsTask().Result).CopyAsync(ApplicationData.Current.LocalFolder);

            App.Model.ScreenWidth = grid.ActualWidth;
            App.Model.ScreenHeight = grid.ActualHeight;

            //DPI：真正的DPI
            App.Model.DPIReady = this.CanvasVirtualControl.Dpi;
            //DPI：使得DPI为96
            this.CanvasVirtualControl.DpiScale *= App.Model.DPI / this.CanvasVirtualControl.Dpi;
            App.Model.VirtualControl = this.CanvasVirtualControl;

             Frame.Navigate(typeof(DrawPage));

          }

    }
}





