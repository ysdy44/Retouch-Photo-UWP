using Windows.Devices.Input;
using Windows.Foundation; 
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace 修图.Library
{
   public class Judge
    {
        //返回压力
        public static float Pressure(PointerRoutedEventArgs e, UIElement con)
        {
            return e.GetCurrentPoint(con).Properties.Pressure;
        }
  
        //返回点
        public static Point Position(PointerRoutedEventArgs e, UIElement con)
        {
            return e.GetCurrentPoint(con).Position;
        }
        //返回滚距
        public static double WheelDelta(PointerRoutedEventArgs e, UIElement con)
        {
            return  e.GetCurrentPoint(con).Properties.MouseWheelDelta;
        }




        //是触摸
        public static bool IsTouch(PointerRoutedEventArgs e, UIElement con)
        {
            return e.GetCurrentPoint(con).PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch;
        }



        //是笔
        public static bool IsPen(PointerRoutedEventArgs e, UIElement con)
        {
            return e.GetCurrentPoint(con).PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen && e.GetCurrentPoint(con).Properties.IsBarrelButtonPressed == false && e.GetCurrentPoint(con).IsInContact;
        }
        //是橡皮
        public static bool IsBarrel(PointerRoutedEventArgs e, UIElement con)
        {
            return e.GetCurrentPoint(con).PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen && e.GetCurrentPoint(con).Properties.IsBarrelButtonPressed == true && e.GetCurrentPoint(con).IsInContact;
        }





        //是左键
        public static bool IsMouse(PointerRoutedEventArgs e, UIElement con)
        {
            return e.GetCurrentPoint(con).PointerDevice.PointerDeviceType == PointerDeviceType.Mouse;
        }
        //是左键
        public static bool IsLeft(PointerRoutedEventArgs e, UIElement con)
        {
            return e.GetCurrentPoint(con).PointerDevice.PointerDeviceType == PointerDeviceType.Mouse && e.GetCurrentPoint(con).Properties.IsLeftButtonPressed;
        }
        //是右键
        public static bool IsRight(PointerRoutedEventArgs e, UIElement con)
        {
            return e.GetCurrentPoint(con).Properties.IsRightButtonPressed || e.GetCurrentPoint(con).Properties.IsMiddleButtonPressed;
        }



    }
}
