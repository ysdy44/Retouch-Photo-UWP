using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
 using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
 
 
namespace 修图.Control
{
    public sealed partial class ColorButton : UserControl
    {
        //Delegate
        public delegate void ColorChangedHandler(Color Color);
        public event ColorChangedHandler ColorChanged;

        public delegate void StrawChangedHandler();
        public event StrawChangedHandler StrawChanged;

        #region DependencyProperty：依赖属性


        //刷新        
         public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorButton), new PropertyMetadata(Colors.Gray, new PropertyChangedCallback(ColorOnChang)));

        private static void ColorOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorButton Con = (ColorButton)sender;

            Con.ColorPicker.Color = (Color)e.NewValue;
            Con.Border.Background = new SolidColorBrush((Color)e.NewValue);
         }


        //刷新        
         public bool isStraw
        {
            get { return (bool)GetValue(isStrawProperty); }
            set { SetValue(isStrawProperty, value); }
        }

        public static readonly DependencyProperty isStrawProperty =
            DependencyProperty.Register("isStraw", typeof(bool), typeof(ColorButton), new PropertyMetadata(true, new PropertyChangedCallback(isStrawOnChang)));

        private static void isStrawOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorButton Con = (ColorButton)sender;

            if (Con.isSalf ==false)  Con.ColorPicker.isStraw = (bool)e.NewValue;
        }


        #endregion

        bool isSalf = false;//是否自己

        public ColorButton()
        {
            this.InitializeComponent();
          }


        #region Global：全局


        private void ColorFlyout_Opened(object sender, object e)
        {
          //  if (color!=ColorPicker.Color) ColorPicker.Color = color;
           }
        private void ColorFlyout_Closed(object sender, object e)
        {
         //   ColorChanged?.Invoke(Color.FromArgb(Color.A, Color.R, Color.G, Color.B));
        }


     //点击就选色
        private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(grid);

            ColorPicker.RequestedTheme = App.Model.Theme;
        }


        //颜色选择器变化
        private void ColorPicker_ColorChanged(Windows.UI.Color Color, SolidColorBrush Brush)
        {
            isSalf = true;
             this.Color = Color.FromArgb(Color.A, Color.R, Color.G, Color.B); //改变颜色 
            ColorChanged?.Invoke(this.Color);

            isSalf = false ;

            Border.Background = Brush;
        }
        //吸管变化
        private void ColorPicker_StrawChanged()
        {
            StrawChanged?.Invoke();

            ColorFlyout.Hide();
            App.Model.StrawVisibility = Visibility.Collapsed;
        }


        #endregion
  

  
    }
}
