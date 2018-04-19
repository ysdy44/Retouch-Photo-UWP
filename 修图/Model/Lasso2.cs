using System.Numerics;
using Windows.UI;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;

namespace 修图.Model
{
    class Lasso2
    {
        //笔刷
        CanvasGradientStop[] stop = new CanvasGradientStop[2] {new CanvasGradientStop{Color= Colors.White,Position= 0 } ,new CanvasGradientStop{Color= Colors.Black,Position= 1 }};
        CanvasLinearGradientBrush lgb;

        //点与间隔更新点
        Vector2 StartPoint;
        Vector2 EndPoint;
        Vector2 Space;

        //画
        ICanvasImage Image;
        CanvasCommandList CommandList;
 
        //初始化：？？？？？？
        public Lasso2(ICanvasResourceCreator rc,float Distance=6,float Spa=1)
        {
            CommandList = new CanvasCommandList(rc);
 
            StartPoint = new Vector2(0, 0);
             EndPoint = new Vector2(Distance, Distance);
             Space = new Vector2(Spa, Spa);



            lgb = new CanvasLinearGradientBrush(rc, stop, CanvasEdgeBehavior.Mirror, CanvasAlphaMode.Premultiplied);
            lgb.StartPoint = StartPoint;
            lgb.EndPoint = EndPoint;
        }


        //方法：设置（写到创建资源事件里）
        public void Set(ICanvasResourceCreator rc, float sx, float sy, ICanvasImage ci)
        {
            //放大
            ScaleEffect se = new ScaleEffect
            {
                Source = ci,
                Scale = new Vector2(1 / sx, 1 / sy),
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
            };

            //剪裁四周的边线
            var rect = se.GetBounds(rc);
            CropEffect sce = new CropEffect
            {
                Source = se,
                SourceRectangle = new Rect(2, 2, rect.Width - 4, rect.Height - 4),
            };

            //恢复正常大小
            CanvasCommandList ccl = new CanvasCommandList(rc);
            using (var ds = ccl.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(sce);
            }

            //变成线框
            Image = new LuminanceToAlphaEffect//亮度转不透明度
            {
                Source = new EdgeDetectionEffect//边缘检测
                {
                    Amount =1,
                    Source = ccl
                },
            };
        }

        //方法：更新（写到Canvas_Update事件里）
        public void Update()
        {
             StartPoint -= Space;
            EndPoint -= Space;
            lgb.StartPoint = StartPoint;
            lgb.EndPoint = EndPoint;
        }

        //方法：绘画（写到Canvas_Draw事件里）
        Rect r = new Rect();
        public void Draw(ICanvasResourceCreator rc, CanvasDrawingSession ds, double W, double H, float X = 0, float Y = 0)
        {
            if (this.Image != null)
            {
                CommandList = new CanvasCommandList(rc);
                r.Width = W;
                r.Height = H;

                using (var dds = CommandList.CreateDrawingSession())
                {
                    dds.FillRectangle(0, 0, (float)W, (float)H, lgb);
                    dds.DrawImage(this.Image, 0, 0, r, 1, CanvasImageInterpolation.NearestNeighbor, CanvasComposite.DestinationIn);
                }
                ds.DrawImage(CommandList, X, Y);
            }
        }


    }
}
