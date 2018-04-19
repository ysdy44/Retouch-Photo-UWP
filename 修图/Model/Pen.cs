using System.Numerics;

namespace 修图.Model
{
    public class Pen
    {
        public bool isChecked;
        public Vector2 Vect { get; set; }
        public Vector2 Left { get; set; }
        public Vector2 Right { get; set; }


        //构造函数
        public Pen(Vector2 v)
        {
            Vect = v;
            Left = v;
            Right = v;
        }



        //移动：左中右整体移动
        public void Move(Vector2 v)
        {
            Vect += v;
            Left += v;
            Right += v;
        }



        //设置：设置中点，左右跟随
        public void Set(Vector2 v)
        {
            var changed = v - Vect;
            Vect = v;
            Left += changed;
            Right += changed;
        }



        //设置左：右点跟随
        public void SetLeft(Vector2 v)
        {
            Left = v;

            if (App.Setting.PenDash == 0)
            {
                Vect = v;
                Right = v;
            }
            else if (App.Setting.PenDash == 1)
                Right = 2 * Vect - v;
            else if (App.Setting.PenDash == 3)
            {
                //计算Vect与Left
                float vld = Vector2.Distance(Vect, v);
                float vlsin = (Vect.Y - v.Y) / vld;
                float vlcos = (Vect.X - v.X) / vld;
                //改变Right
                Right = new Vector2(Vect.X + vlcos * RightDistance, Vect.Y + vlsin * RightDistance);
            }
        }
        //辅助：向量右与主向量的距离
        public float RightDistance;
        public void SetRightDistance()
        {
            RightDistance = Vector2.Distance(Vect, Right);
        }



        //设置右：左点跟随
        public void SetRight(Vector2 v)
        {
            Right = v;

            if (App.Setting.PenDash == 0)
            {
                Vect = v;
                Left = v;
            }
          else  if (App.Setting.PenDash == 1)
                Left = 2 * Vect - v;
            else if (App.Setting.PenDash == 3)
            {
                //计算Vect与Right
                float vld = Vector2.Distance(Vect, Right);
                float vlsin = (Vect.Y - Right.Y) / vld;
                float vlcos = (Vect.X - Right.X) / vld;
                //改变Left
                Left = new Vector2(Vect.X + vlcos * LeftDistance, Vect.Y + vlsin * LeftDistance);
            }
        }
        //辅助：向量右与主向量的距离
        public float LeftDistance;
        public void SetLeftDistance()
        {
            LeftDistance = Vector2.Distance(Vect, Left);
        }
    }
}
