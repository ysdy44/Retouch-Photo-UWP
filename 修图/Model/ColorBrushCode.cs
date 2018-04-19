using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace 修图.Model
{
   public  class ColorBrushCode
    {
        //颜色
        public String Name { get; set; }
        public Color Color { get; set; }
        public SolidColorBrush Brush { get => new SolidColorBrush(Color); }

        //参数
        public string R { get => "R：" + Color.R.ToString(); }
        public string G { get => "G：" + Color.G.ToString(); }
        public string B { get => "B：" + Color.B.ToString(); }

        //十六进制代码
        public string Code { get => "#" + Color.R.ToString("x2") + Color.G.ToString("x2") + Color.B.ToString("x2"); }

        //字体颜色（自适应：笔刷偏白字体就更黑，笔刷偏黑字体就更白）
    //    public SolidColorBrush Foreground { get => Color.R + Color.G + Color.B < 384 ? new SolidColorBrush(Method.改变亮度(Color, 0.5)) : new SolidColorBrush(Method.改变亮度(Color, -0.5)); }

    }
}
