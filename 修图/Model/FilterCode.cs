using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;

namespace 修图.Model
{
    class FilterCode : INotifyPropertyChanged
    {
        //名称
        private string name = "Filter";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.OnPropertyChanged("Tittle");
            }
        }

        //数据
        public float[] Array = new float[]
        {
            1.00f, 0.00f, 0.00f, 0.00f, 0.00f,

            1.00f, 0.00f, 0.00f, 0.00f,

            0.00f, 0.00f, 0.00f, 0.00f
        };


        //缩略图
        private Uri uri = new Uri("ms-appx:///iron/Filter/Original.png");
        public Uri Uri
        {
            get { return uri; }
            set
            {
                uri = value;
                this.OnPropertyChanged("Uri");
            }
        }


        public FilterCode()
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
