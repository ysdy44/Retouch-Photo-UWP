using System; 
using Windows.Foundation; 
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls; 
using Windows.UI.Xaml.Input; 
using 修图.Library;
 
namespace 修图.Picker
{
    public sealed partial class RotationPicker : UserControl
    {
        //网格面板中心点
        Point Center;

        //拇指的具体位置
        Point Position
        {
            set
            {
                Canvas.SetLeft(Thumb, value.X- Thumb.Width/2);
                Canvas.SetTop(Thumb, value.Y - Thumb.Height / 2);
            }
        }

        //圆环的半径
        double Radius;

        //旋转角度（0~360）
        double rotation;
        double Rotation
        {
            set
            {
                rotation = value;

                double angle = (value+90) / 180 * Math.PI;
                double sin = Math.Sin(angle);
                double cos = Math.Cos(angle);

                Position = new Point( Center.X + sin * Radius,Center.Y + cos * Radius );
            }
            get
            {
                return rotation;
            }
        }

        //拇指尺寸
        double ThumbSize = 20;

          //用户控件，向外传值
        public delegate void AngleChangeHandler(object sender, double Angle,double Rotation);
        public event AngleChangeHandler AngleChange = null;

        #region DependencyProperty：依赖属性


        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
          public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(RotationPicker), new PropertyMetadata(0, new PropertyChangedCallback(AngleOnChang)));

        private static void AngleOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RotationPicker Con = (RotationPicker)sender;

            var angle = (double)e.NewValue;
            Con.Rotation = angle * Math.PI * 180; 
        }


        #endregion


        public RotationPicker()
        {
            this.InitializeComponent();
 
            Thumb.Width = Thumb.Height = ThumbSize;
          }


        #region Global：全局


        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        { 
            Center.X = e.NewSize.Width / 2;
            Center.Y = e.NewSize.Height / 2;

             Radius = Math.Min(e.NewSize.Width, e.NewSize.Height) / 2 - ThumbSize/2;
            Ellipse.Width = Ellipse.Height = Radius * 2;

             Canvas.SetLeft(Ellipse, Center.X - Radius);
            Canvas.SetTop(Ellipse, Center.Y - Radius);

             Rotation = rotation;
            AngleChange?.Invoke(this, Rotation / 180 * Math.PI, Rotation); //用户控件：Value改变
        }


        #endregion


        #region Point：指针事件


        bool isPressed;
  
        Point point;//开始

        private void thume_DragStarted(object sender, Windows.UI.Xaml.Controls.Primitives.DragStartedEventArgs e)
        {
            point = new Point(e.HorizontalOffset,e.VerticalOffset);
  
            if (isPressed == false)//&&Math.Abs(distance-Radius)< ThumbSize/2)
            {
                isPressed = true;
                Start(point);
            }
        }

        private void thume_DragDelta(object sender, Windows.UI.Xaml.Controls.Primitives.DragDeltaEventArgs e)
        {
            point.X += e.HorizontalChange;
            point.Y += e.VerticalChange;

            if (isPressed == true)
            {
                Delta(point);
            }
        }

        private void thume_DragCompleted(object sender, Windows.UI.Xaml.Controls.Primitives.DragCompletedEventArgs e)
        {
             point.X += e.HorizontalChange;
            point.Y += e.VerticalChange;

            if (isPressed == true)
            {
                isPressed = false;
                Complete(point);
            }
        }



        //滚轮变化
        private void thume_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            Wheel_Changed(Judge.Position(e, Canvas), Judge.WheelDelta(e, Canvas));
         }


        #endregion

        
        #region Event：封装事件

          
        //移动事件
        private void Start(Point p)
        {
            Rotation = Method.点角度(Center, p);
            AngleChange?.Invoke(this, Rotation / 180 * Math.PI, Rotation); //用户控件：Value改变
        }
        private void Delta(Point p)
        {
            Rotation = Method.点角度(Center, p);
            AngleChange?.Invoke(this, Rotation / 180 * Math.PI, Rotation); //用户控件：Value改变
        }
        private void Complete(Point p)
        {
             Rotation = Method.点角度(Center, p);
            AngleChange?.Invoke(this, Rotation / 180 * Math.PI, Rotation); //用户控件：Value改变
        }


        //滚轮
        private void Wheel_Changed(Point p, double d)
        {
             Rotation +=d/100;
            AngleChange?.Invoke(this, Rotation / 180 * Math.PI, Rotation); //用户控件：Value改变
        }



        #endregion
 
    }
}
