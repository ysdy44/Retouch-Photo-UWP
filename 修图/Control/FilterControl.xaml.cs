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
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using 修图.Library;
using 修图.Model;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;

namespace 修图.Control
{
    public sealed partial class FilterControl : UserControl
    {
        public FilterControl()
        {
            this.InitializeComponent();
        }

        ObservableCollection<FilterCode> FilterCodeList = new ObservableCollection<FilterCode>() { };

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Original：原图 
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/Original"),
                Uri = new Uri("ms-appx:///Icon/Filter/Original.png"),
                Array = new float[] { 1.00f, 0.00f, 0.00f, 0.00f, 0.00f, 1.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f }
            });



            //Memory：怀念
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/MemoryMemory"),
                Uri = new Uri("ms-appx:///Icon/Filter/Memory/Memory.png"),
                Array = new float[] { 1.00f, 0.27f, 1.00f, 0.455f, -0.375f, 1.33f, 0.00f, 0.49f, 0.00f, 0.00f, 0.00f, 0.08f, 0.19f, }
            });
            //Old：老照片
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/MemoryOld"),
                Uri = new Uri("ms-appx:///Icon/Filter/Memory/Old.png"),
                Array = new float[] { 0.87f, 0.05f, 031f, 0.00f, 0.015f, 0.52f, 3.66f, 0.39f, 0.66f, 0.00f, 0.00f, 0.00f, 0.00f, }
            });
            //Childhood：童年
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/MemoryChildhood"),
                Uri = new Uri("ms-appx:///Icon/Filter/Memory/Childhood.png"),
                Array = new float[] { 1.01f, 0.34f, 0.75f, 0.035f, -0.45f, 1.06f, 0.00f, 0.11f, -1.00f, 0.00f, 0.32f, 0.14f, 0.17f, }
            });



            //Shadow：光影
             //Morning：早晨
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ShadowMorning"),
                Uri = new Uri("ms-appx:///Icon/Filter/Shadow/Morning.png"),
                Array = new float[] { 1.27f, 0.00f, 0.205f, 0.22f, -0.04f, 1.21f, 0.00f, 0.00f, 0.00f, 0.00f, 0.12f, 0.00f, 0.05f, }
            });
            //Shadow：光影
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ShadowShadow"),
                Uri = new Uri("ms-appx:///Icon/Filter/Shadow/Shadow.png"),
                Array = new float[] { 1.03f, 0.25f, 0.145f, 0.3f, -0.095f, 0.44f, 0.00f, 0.08f, 0.56f, 0.00f, 0.07f, 0.00f, 0.00f, }
            });
            //Dusk：黄昏
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ShadowDusk"),
                Uri = new Uri("ms-appx:///Icon/Filter/Shadow/Dusk.png"),
                Array = new float[] { 1.52f, -0.02f, 0.025f, 0.00f, 0.00f, 1.56f, 0.00f, 0.075f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, }
            });



            //Classic：经典
            //Pure：清纯
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ClassicPure"),
                Uri = new Uri("ms-appx:///Icon/Filter/Classic/Pure.png"),
                Array = new float[] { 1f, 0.51f, 0.08f, 0.88f, 0.31f, 1.26f, 0f, -0.195f, -0.52f, 0f, 0f, 0f, 0 }
            });
            //Cowboy：牛仔
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ClassicCowboy"),
                Uri = new Uri("ms-appx:///Icon/Filter/Classic/Cowboy.png"),
                Array = new float[] { 1f, 0.32f, 0.375f, 0.275f, 0.03f, 1f, 0f, 0.34f, 0f, 0f, 0f, 0f, 0 }
            });
            //Tranquil：恬淡
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ClassicTranquil"),
                Uri = new Uri("ms-appx:///Icon/Filter/Classic/Tranquil.png"),
                Array = new float[] { 1f, 0.38f, 0f, 0f, 0.115f, 1f, 0f, 0f, 0f, 0f, 0.13f, 0f, 0 }
            });
            //Unreal：虚幻
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ClassicUnreal"),
                Uri = new Uri("ms-appx:///Icon/Filter/Classic/Unreal.png"),
                Array = new float[] { 1.06f, 0.16f, 0.895f, 1f, -0.14f, 1.38f, 0f, 0f, -0.13f, 0f, 0.19f, 0.27f, 0.28f }
            });
            //Blues：蓝调
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ClassicBlues"),
                Uri = new Uri("ms-appx:///Icon/Filter/Classic/Blues.png"),
                Array = new float[] { 1.06f, 0.16f, 0.5f, -0.055f, -0.29f, 1.38f, 6f, 0f, 1f, 0f, 0f, 0.04f, 0.09f }
            });
             //Handsome：帅气
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ClassicHandsome"),
                Uri = new Uri("ms-appx:///Icon/Filter/Classic/Handsome.png"),
                Array = new float[] { 1f, -0.1f, 0.775f, 0.555f, -0.06f, 1f, 0f, 0.165f, 0f, 0.34f, 0f, 0f, 0.08f}
            });
             //Sentimental：青涩
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ClassicSentimental"),
                Uri = new Uri("ms-appx:///Icon/Filter/Classic/Sentimental.png"),
                Array = new float[] { 0.98f, 0.13f, 0.72f, -0.815f, -0.365f, 1f, 0f, 0.19f, 0f, 0.34f, 0.11f, 0.12f, 0.18f }
            });
            //Personality：个性
            FilterCodeList.Add(new FilterCode
            {
                Name = App.resourceLoader.GetString("/Filter/ClassicPersonality"),
                Uri = new Uri("ms-appx:///Icon/Filter/Classic/Personality.png"),
                Array = new float[] { 1f, -0.14f, 1f, 0f, 0.685f, 1.48f, 0f, 0.155f, 0.455f, 0f, 0f, 0f, 0 }
            });


            //XXXXX：水水水水水水水水
            // FilterCodeList.Add(new FilterCode
            // {
            //     Name = "XXXXXXX",
            //     Uri = new Uri("ms-appx:///Icon/Filter/Classic/XXXXX.png"),
            //      Array = new float[] { }
            //  });


            FilterListView.ItemsSource = FilterCodeList;
        }



        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            App.Setting.AdjustArray = (e.ClickedItem as FilterCode).Array;
            修图.BarPage.AdjustBarPage.Render();
        }
    }
}
