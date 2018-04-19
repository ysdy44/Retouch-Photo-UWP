using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace 修图.Model
{
    public class PaintBrush : INotifyPropertyChanged
    {
        public float Width = 12;
        public float Hard = 1;
        public float Opacity = 1;
        public float Space = 0.25f;

        public bool isBitmap = false;
        public string Bitmap;
         
         //展示图
        private Uri uri = new Uri("ms-appx:///iron/photo.jpg");
        public Uri Uri
        {
            get { return uri; }
            set
            {
                uri = value;
                this.OnPropertyChanged("Uri");
            }
        }
        //缩略图
         private Uri thumbnail = new Uri("ms-appx:///iron/photo.jpg");
        public Uri Thumbnail
        {
            get { return thumbnail; }
            set
            {
                thumbnail = value;
                this.OnPropertyChanged("Thumbnail");
            }
        }

         //绑定宽度
        public string WidthString => ((int)Width).ToString();


        public PaintBrush()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
