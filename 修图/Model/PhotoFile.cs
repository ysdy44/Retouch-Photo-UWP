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
    public class PhotoFile : INotifyPropertyChanged
    {

         //名称
        private string name = "Tittle";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.OnPropertyChanged("Tittle");
            }
        }

        //描述
        private string describe = "Describe";
        public string Describe
        {
            get { return describe; }
            set
            {
                describe = value;
                this.OnPropertyChanged("Describe");
            }
        }


        //缩略图
        private Uri uri = new Uri("ms-appx:///Iron/photo.jpg");
        public Uri Uri
        {
            get { return uri; }
            set
            {
                uri = value;
                this.OnPropertyChanged("Uri");
            }
        }
 

        //日期
        public DateTimeOffset Time;

        //路径
        public string Path;

        //是否为最后一个添加的 
        private Visibility isadd = Visibility.Collapsed;
        public Visibility isAdd
        {
            get { return isadd; }
            set
            {
                isadd = value;
                this.OnPropertyChanged("isAdd");
            }
        }

         

        public PhotoFile()
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
