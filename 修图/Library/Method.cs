using System;
using Windows.Foundation;
using Windows.UI;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace 修图.Library
{
    class Method
    {

        //根据向量求角度
        public static float Tanh(Vector2 vector)
        {
            float tan = (float)Math.Atan(Math.Abs(vector.Y / vector.X));

            if (vector.X > 0 && vector.Y > 0)//第一象限
                return tan;
            else if (vector.X > 0 && vector.Y < 0)//第二象限
                return -tan;
            else if (vector.X < 0 && vector.Y > 0)//第三象限  
                return (float)Math.PI - tan;
            else
                return tan - (float)Math.PI;
        }
        public static float Tanh(float vectorX, float vectorY)
        {
            float tan = (float)Math.Atan(Math.Abs(vectorY / vectorX));

            if (vectorX > 0 && vectorY > 0)//第一象限
                return tan;
            else if (vectorX > 0 && vectorY < 0)//第二象限
                return -tan;
            else if (vectorX < 0 && vectorY > 0)//第三象限  
                return (float)Math.PI - tan;
            else
                return tan - (float)Math.PI;
        }






        //输出A与B的距离
        public static double 两点距(Point A, Point B)
        {
            return Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));//椭圆半焦距
        }

        //在直线AB上，使得输出点P与A点距离为distance
        public static Point 距在线上点( double distance,Point A, Point B)
        {
            var Scale = distance / 两点距(A, B);
            return new Point(-Scale * (B.X - A.X) + A.X, -Scale * (B.Y - A.Y) + A.Y);
        }

        //在直线AB上，使得输出点P与A点距离的比例为scale（0~1）
        public static Point 比在线上点(float scale, Point A, Point B)
        {
            return new Point((B.X - A.X)* scale + A.X, (B.Y - A.Y)* scale+A.Y); 
        }

        //输出P与线AB的距离
        public static double 点线距(Point P,Point A, Point B)
        {
            double dis = 0;
            if (A.X == B.X)
            {
                dis = Math.Abs(P.X - A.X);
                return dis;
            }
            double lineK = (B.Y - A.Y) / (B.X - A.X);
            double lineC = (B.X * A.Y - A.X * B.Y) / (B.X - A.X);
            dis = Math.Abs(lineK * P.X - P.Y + lineC) / (Math.Sqrt(lineK * lineK + 1));
            return dis;
        }

        //P在直线AB上的映射点，输出距离A的距离
        public static double 点在线上距(Point P, Point A, Point B)
        {
            var 斜边 = 两点距(P, A);
            var 高 = 点线距(P, A, B);
            return Math.Sqrt(斜边 * 斜边 - 高 * 高);
        }





        public static double 两边距(double A, double B)//求c2（c2=a2+b2）
        {
            return Math.Sqrt(A * A + B * B);//椭圆半焦距
        }

        public static double 点线距(Point P, Point A, Point B, double 两点距)//两点距是AB的距离
        {
            var 上距离 = (A.X - B.X) * (P.X - A.X) + (A.Y - B.Y) * (P.Y - A.Y);

            return Math.Abs(上距离) / 两点距;
        }

        public static Point 点与线垂直点(Point P, Point A, Point B)//P与线AB的距离
        {
            double XX = A.X - B.X; //缓存值：X1 - X2
            double YY = A.Y - B.Y; // 缓存值：Y1 - Y2
            double XY = A.X * B.Y - A.Y * B.X;// 缓存值：X1*Y2 - X2*Y1

            double 上式 = P.Y + XX / YY * P.X - XY / XX;//算式：分子
            double 下式 = YY / XX + XX / YY;//算式：分母

            double X = 上式 / 下式;//垂直点：X
            double Y = YY / XX * X + XY / XX;//垂直点：Y

            return new Point(X, Y);
        }

        public static double 点角度(Point O, Point P)//返回360度
        {
            //除数不能为0
            double tan = Math.Atan(Math.Abs((P.Y - O.Y) / (P.X - O.X))) * 180 / Math.PI;

            if (P.X > O.X && P.Y > O.Y)//第一象限
                return -tan;
            else if (P.X > O.X && P.Y < O.Y)//第二象限
                return tan;
            else if (P.X < O.X && P.Y > O.Y)//第三象限
                return tan - 180;
            else
                return 180 - tan;
        }




        public struct HSL
        {
            public double H;
            public double S;
            public double L;
        }

        //输入色相（0~360），输出最艳的颜色
        public static Color HSLtoRGB(double H)
        {
            double hh = H / 60;
            double x = 1 - Math.Abs(hh % 2 - 1);  //6个色相内的内部渐变的数值
            double R, G, B;

            if (hh < 1) { R = 1; G = x; B = 0; }
            else if (hh < 2) { R = x; G = 1; B = 0; }
            else if (hh < 3) { R = 0; G = 1; B = x; }
            else if (hh < 4) { R = 0; G = x; B = 1; }
            else if (hh < 5) { R = x; G = 0; B = 1; }
            else { R = 1; G = 0; B = x; }

            return Color.FromArgb(255, (byte)(255 * R), (byte)(255 * G), (byte)(255 * B));
        }
          //HSL转换为RGB   H:0~360,S:0~1,L:0~1 
        public static Color HSLtoRGB(int A, double H, double S, double L)
        {
            double hh = H % 360.0;

            if (S == 0.0)
            {
                byte lllllllll = (byte)(L * 255.0);
                return Color.FromArgb((byte)A, lllllllll, lllllllll, lllllllll);
            }
            else
            {
                double dhh = hh / 60.0;
                int nhh = (int)Math.Floor(dhh);
                double rhh = dhh - nhh;

                double rr = L * (1.0 - S);
                double gg = L * (1.0 - (S * rhh));
                double bb = L * (1.0 - (S * (1.0 - rhh)));

                switch (nhh)
                {
                    case 0: return Color.FromArgb((byte)A, (byte)(L * 255.0), (byte)(bb * 255.0), (byte)(rr * 255.0));
                    case 1: return Color.FromArgb((byte)A, (byte)(gg * 255.0), (byte)(L * 255.0), (byte)(rr * 255.0));
                    case 2: return Color.FromArgb((byte)A, (byte)(rr * 255.0), (byte)(L * 255.0), (byte)(bb * 255.0));
                    case 3: return Color.FromArgb((byte)A, (byte)(rr * 255.0), (byte)(gg * 255.0), (byte)(L * 255.0));
                    case 4: return Color.FromArgb((byte)A, (byte)(bb * 255.0), (byte)(rr * 255.0), (byte)(L * 255.0));
                    default: return Color.FromArgb((byte)A, (byte)(L * 255.0), (byte)(rr * 255.0), (byte)(gg * 255.0));
                }
            }
        }

        //RGB转换为HSL  H:0~360,S:0~1,L:0~1 
        public static HSL RGBtoHSL(Color coo)
        {
            double r = coo.R / 255.0;
            double g = coo.G / 255.0;
            double b = coo.B / 255.0;
            double max;//大
            double min;//小
            double dist;//差
            double r2, g2, b2;

            double h = 0; // default to black
            double s = 0;
            double l = 0;

            max = Math.Max(Math.Max(r, g), b);
            min =  Math.Min(Math.Min(r, g), b);
            l = (min + max) / 2.0;

            if (l <= 0.0)  return new HSL { H = 0, S = 0, L =0 };

            dist = max - min;
            s = dist;

            if (s > 0.0) s /= (l <= 0.5) ? (max + min) : (2.0 - max - min);
            else return new HSL { H = 0, S =0, L =0};

            r2 = (max - r) / dist;
            g2 = (max - g) / dist;
            b2 = (max - b) / dist;

            if (r == max) h = (g == min ? 5.0 + b2 : 1.0 - g2);
            else if (g == max)h = (b == min ? 1.0 + r2 : 3.0 - b2);
            else h = (r == min ? 3.0 + g2 : 5.0 - r2);
     
            return new HSL { H = h*60d, S = s, L = l*2 };
        }

        


        //混合颜色
        public static Color BlendColor(double Opacity, Color Backgroud)
        {
            byte R = (byte)(Opacity * Backgroud.R);
            byte G = (byte)(Opacity * Backgroud.G);
            byte B = (byte)(Opacity * Backgroud.B);

            return Color.FromArgb(255, R, G, B);
        }
        public static Color BlendColor(double Opacity, Color Backgroud, Color Foreground)
        {
            double UnOpacity = 1 - Opacity;

            byte R = (byte)(Opacity * Foreground.R + UnOpacity * Backgroud.R);
            byte G = (byte)(Opacity * Foreground.G + UnOpacity * Backgroud.G);
            byte B = (byte)(Opacity * Foreground.B + UnOpacity * Backgroud.B);

            return Color.FromArgb(255, R, G, B);
        }




    }
}
