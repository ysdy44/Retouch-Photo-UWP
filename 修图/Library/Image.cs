using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;

namespace 修图.Library
{
  public  class Image
    {
        //保存不同的格式到相册的“保存图片”
        public static async void SaveJpeg(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".jpg", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
        }
        public static async Task SavePng(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".png", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
        }
        public static async Task SaveBmp(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".bmp", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
        }
        public static async Task SaveGif(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".gif", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.GifEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
        }
        public static async Task SaveTiff(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".tif", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.TiffEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
        }

        public static async Task SaveJpeg(IStorageFolder Folder, CanvasBitmap cb, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".jpg", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await cb.SaveAsync(fileStream, CanvasBitmapFileFormat.Jpeg);
            }
        }
        public static async void SavePng(IStorageFolder Folder, CanvasBitmap cb, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".png", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await cb.SaveAsync(fileStream, CanvasBitmapFileFormat.Png);
             }
        }
        public static async void SaveBmp(IStorageFolder Folder, CanvasBitmap cb, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".bmp", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await cb.SaveAsync(fileStream, CanvasBitmapFileFormat.Bmp);
            }
        }
        public static async void SaveGif(IStorageFolder Folder, CanvasBitmap cb, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".gif", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await cb.SaveAsync(fileStream, CanvasBitmapFileFormat.Gif);
            }
        }
        public static async void SaveTiff(IStorageFolder Folder, CanvasBitmap cb, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".tif", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await cb.SaveAsync(fileStream, CanvasBitmapFileFormat.Tiff);
            }
        }

        public static async Task<StorageFile> GetJpeg(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".jpg", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
            return file;
        }
        public static async Task<StorageFile> GetPng(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".png", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
            return file;
        }
        public static async Task<StorageFile> GetBmp(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".bmp", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
            return file;
        }
        public static async Task<StorageFile> GetGif(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".gif", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.GifEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
            return file;
        }
        public static async Task<StorageFile> GetTiff(IStorageFolder Folder, byte[] bytes, int Width, int Height, string Name, CreationCollisionOption options = CreationCollisionOption.GenerateUniqueName)
        {
            StorageFile file = await Folder.CreateFileAsync(Name + ".tif", options);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.TiffEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)Width, (uint)Height, .0, .0, bytes);
                await encoder.FlushAsync();
            }
            return file;
        }

        
        
        //流转图
        public static WriteableBitmap BuffertoBitmap(CanvasRenderTarget crt)
        {
            byte[] buffer = crt.GetPixelBytes();
            int Width = (int)crt.SizeInPixels.Width;
            int Height = (int)crt.SizeInPixels.Height;

            WriteableBitmap writeableBitmap = new WriteableBitmap(Width, Height);//创建新Bitmap，宽高是图片宽高
            Stream stream = writeableBitmap.PixelBuffer.AsStream(); //Bitmap=>Stream
            stream.Seek(0, SeekOrigin.Begin);//Stream.Seek寻找（从零开始）
            stream.Write(buffer, 0, buffer.Length);//Stream.Write写入，写入Buffer（从零到其长度）

            return writeableBitmap;
        }

        //生成中心法线贴图
        public static Color[] Displacement(int Width, int Height)
        {
            int WidthHalf = Width / 2;
            int HeightHalf = Height / 2;

            Color[] Dis = new Color[Width * Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    double w = x- WidthHalf;
                    double h =y - HeightHalf;
                    
                        double Hor = w / WidthHalf;
                        double Ver = h / HeightHalf;

                        Dis[x + y * Width] = Color.FromArgb(255, (byte)(127 + 128 * Hor), (byte)(127 + 128 * Ver), 255);
                }
            }
            return Dis;
        }

        //组转图
        public static WriteableBitmap ColortoBitmap(Color[,] color)
        {
            //《组转化成流》
            //////////////////////////////////////////////////////////////////////////////////////////////////
            int height = color.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int width = color.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            Byte[] buffer = new Byte[color.Length * 4]; //创建流，长度是数组长度的4倍，存储BGRA四个分量

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color cooo = color[y, x]; //临时颜色

                    int Num = (y * width + x) * 4;//计算第一个点的位置

                    buffer[Num] = cooo.B;//蓝色B
                    buffer[Num + 1] = cooo.G;//绿色G
                    buffer[Num + 2] = cooo.R;//红色R
                    buffer[Num + 3] = cooo.A;//透明度Alpha
                }
            }
            //《流转化成图》
            //////////////////////////////////////////////////////////////////////////////////////////////////
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height);//创建新Bitmap，宽高是图片宽高
            Stream stream = writeableBitmap.PixelBuffer.AsStream(); //Bitmap=>Stream
            stream.Seek(0, SeekOrigin.Begin);//Stream.Seek寻找（从零开始）
            stream.Write(buffer, 0, buffer.Length);//Stream.Write写入，写入Buffer（从零到其长度）

            return writeableBitmap;
        }
        
    }
}
