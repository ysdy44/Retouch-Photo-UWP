using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI.Text;

namespace 修图.Library
{
    public class Geometry
    {

        //Word：单词
        public static (CanvasGeometry Geometry, Vector2 Vect) CreateWord(ICanvasResourceCreator rc, Vector2 start, Vector2 end, string text,string fontFamily , FontStyle fontStyle)
        {
            Vector2 Vect;
            Vect.X = Math.Min(start.X, end.X);
            Vect.Y = Math.Min(start.Y, end.Y);
            float h = Math.Abs(start.Y - end.Y);

            CanvasTextFormat format = new CanvasTextFormat()
            {
                 FontFamily= fontFamily,
                 FontStyle= fontStyle,
                 FontSize = h > 2 ? h : 2
            };
            CanvasTextLayout layoout = new CanvasTextLayout(rc, text, format, App.Model.Width, h);
            CanvasGeometry geometry = CanvasGeometry.CreateText(layoout);
 
            return (geometry, Vect);
        }
        //Triangle：三角形
        public static CanvasGeometry CreateTriangle(ICanvasResourceCreator rc, Vector2 start, Vector2 end)
        {
             Vector2[] points =
            {
                new Vector2(start.X, end.Y),//左下角
                new Vector2(end.X, end.Y),//右下角
                new Vector2((start.X+end.X)/2f, start.Y),//上面
              };
            return CanvasGeometry.CreatePolygon(rc, points);
        }
         
        //Diamond：菱形
        public static CanvasGeometry CreateDiamond(ICanvasResourceCreator rc, Vector2 start, Vector2 end)
        {
            Vector2 end2 = start - (end - start);

               Vector2[] points =
           {
                new Vector2(end2.X, start.Y),//左
                new Vector2(start.X, end2.Y),//上
                new Vector2(end.X, start.Y),//右
                new Vector2(start.X, end.Y),//下
              };
            return CanvasGeometry.CreatePolygon(rc, points);
        }



        //Pentagon：五边形
        public static CanvasGeometry CreatePentagon(ICanvasResourceCreator rc, Vector2 start, Vector2 end,int count)
        {
             Vector2 vector = end - start;
            float radius = vector.Length();//边长

            float rotation = 修图.Library.Method.Tanh(vector);//初始角度
            float angle = (float)Math.PI * 2f / count;//间隔角度

            Vector2[] points = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                Vector2 v = new Point(Math.Cos(rotation), Math.Sin(rotation)).ToVector2();//借本向量
                points[i] = v * radius + start;//五边点

                rotation += angle;
            }

            return CanvasGeometry.CreatePolygon(rc, points);
        }

        //Star：五角星（边数、半径百分比）
        public static CanvasGeometry CreateStar(ICanvasResourceCreator rc, Vector2 start, Vector2 end, int count,float  InnerRadius)
        {
            //基本单位
             int countdouble = count * 2;

            Vector2 vector = end - start;//基本向量
            float radius = vector.Length();//牙齿边长
            float radiushadf = radius * InnerRadius;//切口边长

            float rotation = 修图.Library.Method.Tanh(vector);//初始角度
            float anglehalf = (float)Math.PI * 2f / countdouble;//间隔角度

            Vector2[] points = new Vector2[countdouble];
            for (int i = 0; i < countdouble; i++)
            {
                Vector2 v = new Point(Math.Cos(rotation), Math.Sin(rotation)).ToVector2();//借本向量

                if (i % 2 == 0) points[i] = v * radius + start;//牙齿
                else points[i] = v * radiushadf + start;//切口

                rotation += anglehalf;
            }

            return CanvasGeometry.CreatePolygon(rc, points);
        }

        //Pie：饼图（内半径百分比、开始角度，扫描角度）
        public static CanvasGeometry CreatePie(ICanvasResourceCreator rc, Vector2 start, Vector2 end,float innerRadius, float sweepAngle)
        {
            sweepAngle = (float)Math.PI * 2 - sweepAngle;

               //基本参数
               Vector2 vector = end - start;//基本向量
            float radius = vector.Length();//牙齿边长
            float radiusHalf = radius * innerRadius;//切口边长

            float startAngle = 修图.Library.Method.Tanh(vector);//起始角度

            //计算四个点
            Vector2 startVector = new Point(Math.Cos(startAngle), Math.Sin(startAngle)).ToVector2();//借本向量起始
            Vector2 endVector = new Point(Math.Cos(sweepAngle + startAngle), Math.Sin(sweepAngle + startAngle)).ToVector2();//借本向量终末

            Vector2 startVectorTooth = startVector * radius + start;//起始牙齿
            Vector2 startVectorNotch = startVector * radiusHalf + start;//起始切口

            Vector2 endVectorTooth = endVector * radius + start;//起始牙齿
            Vector2 endVectorNotch = endVector * radiusHalf + start;//起始切口


            //画几何
            CanvasPathBuilder pathBuilder = new CanvasPathBuilder(rc);

            if (sweepAngle < Math.PI)
            {
                pathBuilder.BeginFigure(startVectorNotch);//起始切口点
                pathBuilder.AddArc(endVectorNotch, radiusHalf, radiusHalf, sweepAngle, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Large);//终末切口点

                pathBuilder.AddLine(endVectorTooth); //终末牙齿点
                pathBuilder.AddArc(startVectorTooth, radius, radius, sweepAngle, CanvasSweepDirection.Clockwise, CanvasArcSize.Large);//起始牙齿点
            }
            else
            {
                pathBuilder.BeginFigure(startVectorNotch);//起始切口点
                pathBuilder.AddArc(endVectorNotch, radiusHalf, radiusHalf, sweepAngle, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);//终末切口点

                pathBuilder.AddLine(endVectorTooth); //终末牙齿点
                pathBuilder.AddArc(startVectorTooth, radius, radius, sweepAngle, CanvasSweepDirection.Clockwise, CanvasArcSize.Small);//起始牙齿点
            }
 
            pathBuilder.EndFigure(CanvasFigureLoop.Closed);
            return CanvasGeometry.CreatePath(pathBuilder);
        }
         
        //Cog：齿轮（边数、内半径、牙齿、切口）
        public static CanvasGeometry CreateCog(ICanvasResourceCreator rc, Vector2 start, Vector2 end, int count ,float innerRadius, float tooth, float notch)
        {
            //基本单位
             int countquadra = count * 4;
 
            Vector2 vector = end - start;//基本向量
            float rotation = 修图.Library.Method.Tanh(vector);//初始角度

            float radius = vector.Length();//牙齿半径
            float radiusNotch = radius * innerRadius;//切口半径
 
            float angle = (float)Math.PI * 2f / count;//角度
            float angleTooth = angle * tooth;//牙齿角度
            float angleNotch = angle * notch;//切口角度
            float angleDiffHalf = (angleNotch - angleTooth) / 2;//牙齿与切口的角度差的一半

             Vector2[] points = new Vector2[countquadra];
            for (int i = 0; i < countquadra; i++)
            {
                Vector2 v = new Point(Math.Cos(rotation), Math.Sin(rotation)).ToVector2();

                if (i % 4 == 0)//凸左下点
                {
                    points[i] = v * radiusNotch + start;
                    rotation += angleDiffHalf;
                 }
                else if (i % 4 ==1)//凸左上点
                {
                     points[i] = v * radius+ start;
                    rotation += angleTooth;
                }
                else if (i % 4 == 2)//凸右上点
                {
                    points[i] = v * radius + start;
                    rotation += angleDiffHalf;
                }
                else if (i % 4 == 3)//凸右下点
                {
                    points[i] = v * radiusNotch + start;
                    rotation += angle - angleNotch;
                 }
            }

            return CanvasGeometry.CreatePolygon(rc, points);
        }


        //Arrow：箭头
        public static CanvasGeometry CreateArrow(ICanvasResourceCreator rc,  Vector2 start, Vector2 end, float length)
        {
            //基本单位
            float angle = (float)Math.PI / 6;//箭头角度

            //基本向量
            Vector2 vector = start - end;
            float rotation = 修图.Library.Method.Tanh(vector);//初始角度

             //两侧
            float angleleft = rotation + angle;
            float angleRight = rotation - angle;
            Vector2 left = new Point(Math.Cos(angleleft), Math.Sin(angleleft)).ToVector2();
            Vector2 right = new Point(Math.Cos(angleRight), Math.Sin(angleRight)).ToVector2();

            //两侧牙齿
            Vector2 leftTooth = left * length + end;
            Vector2 rightTooth = right * length + end;

            //两侧切口
            Vector2 leftNotch = (leftTooth + leftTooth + rightTooth) / 3;
            Vector2 rightNotch = (leftTooth + rightTooth + rightTooth) / 3;

            Vector2[] points = { end, leftTooth, leftNotch, start, rightNotch, rightTooth, };
            return CanvasGeometry.CreatePolygon(rc, points);
        }

        //Capsule：胶囊
        public static CanvasGeometry CreateCapsule(ICanvasResourceCreator rc, Vector2 start, Vector2 end,float radius)
        {
            //基本向量
             Vector2 vector = end - start;
            float rotation = 修图.Library.Method.Tanh(vector);//初始角度


            //左右角度/向量
            double rotationLeft = rotation + Math.PI / 2;
            double rotationRight = rotation - Math.PI / 2;
            Vector2 left = new Point(Math.Cos(rotationLeft), Math.Sin(rotationLeft)).ToVector2()* radius;
            Vector2 right = new Point(Math.Cos(rotationRight), Math.Sin(rotationRight)).ToVector2()*radius;

            //计算四个位置
            Vector2 startLeft = start + left;
            Vector2 startRight = start + right;
            Vector2 endLeft = end + left;
            Vector2 endRight = end + right;


            //画几何
            CanvasPathBuilder pathBuilder = new CanvasPathBuilder(rc);

            pathBuilder.BeginFigure(startLeft);//左上角
            pathBuilder.AddArc(startRight, radius, radius, (float)Math.PI, CanvasSweepDirection.Clockwise, CanvasArcSize.Large);//左下角

            pathBuilder.AddLine(endRight);
            pathBuilder.AddArc(endLeft, radius, radius, (float)Math.PI, CanvasSweepDirection.Clockwise, CanvasArcSize.Large);//右下角

            pathBuilder.EndFigure(CanvasFigureLoop.Closed);
            return CanvasGeometry.CreatePath(pathBuilder);
        }


        //Heart：心形
        public static CanvasGeometry CreateHeart(ICanvasResourceCreator rc, Vector2 start, Vector2 end)
        {
            //基本向量
            Vector2 vector = end - start;
            float rotation = 修图.Library.Method.Tanh(vector);//初始角度
            float LengthHalf = vector.Length() / 2;

            Vector2 center = (end + start) / 2;//正方形中心点
            float radius = LengthHalf / (float)Math.Sqrt(2.0d);//圆半径

            //左右角度/向量
            double rotationLeft = rotation + Math.PI / 2;
            double rotationRight = rotation - Math.PI / 2;
            Vector2 left = new Point(Math.Cos(rotationLeft), Math.Sin(rotationLeft)).ToVector2() * LengthHalf+center;//左点
            Vector2 right = new Point(Math.Cos(rotationRight), Math.Sin(rotationRight)).ToVector2() * LengthHalf + center;//右点


            //画几何
            CanvasPathBuilder pathBuilder = new CanvasPathBuilder(rc);
            pathBuilder.BeginFigure(start);//屁眼

            pathBuilder.AddArc(left, radius, radius, (float)Math.PI, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Large);//左下角

            pathBuilder.AddLine(end);
            pathBuilder.AddLine(right);

            pathBuilder.AddArc(start, radius, radius, (float)Math.PI, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Large);//右下角

            pathBuilder.EndFigure(CanvasFigureLoop.Closed);
            return CanvasGeometry.CreatePath(pathBuilder);
        }






    }
}
