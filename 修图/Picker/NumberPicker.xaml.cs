using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace 修图.Picker
{
    public sealed partial class NumberPicker : UserControl
    {
        public int Number;//公开的，要的Value，负责接收与传值
        private int NumberCache; //私有的，临时的Value，用来临时记录旧的Value
        private bool IsClick; //打开后是否点过button，判断是否Number归零

        int DefaultValue=0; //固定值：默认值
        int MaxValue = 100; //固定值：Max
        int MinValue = -100;//固定值：Min

        //用户控件，向外传值
        public delegate void ValueChangeHandler(object sender, int Value);
        public event ValueChangeHandler ValueChange = null;

        #region DependencyProperty：依赖属性


        public int Value
        {
            get { return Number; }//get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumberPicker), new PropertyMetadata(0, new PropertyChangedCallback(ValueOnChang)));
        private static void ValueOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumberPicker Con = (NumberPicker)sender;

            int value = (int)e.NewValue;
            Con.Number = value;

            //Value跟随
            Con.Change();

            Con.button.Content = value.ToString() + Con.text;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int Max
        {
            get { return MaxValue; } //get { return (int)GetValue(ValueProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(nameof(Max), typeof(int), typeof(NumberPicker), new PropertyMetadata(0, new PropertyChangedCallback(MaxOnChang)));
        private static void MaxOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumberPicker Con = (NumberPicker)sender;
            Con.MaxValue = (int)e.NewValue;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int Min
        {
            get { return MinValue; } //get { return (int)GetValue(ValueProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(nameof(Min), typeof(int), typeof(NumberPicker), new PropertyMetadata(0, new PropertyChangedCallback(MinOnChang)));
        private static void MinOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumberPicker Con = (NumberPicker)sender;
            Con.MinValue = (int)e.NewValue;
        }





        string text;
        public string Text
        {
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(NumberPicker), new PropertyMetadata("", new PropertyChangedCallback(TextOnChang)));
        private static void TextOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumberPicker Con = (NumberPicker)sender;
            Con.text =(string)e.NewValue;
        }



        public FlyoutPlacementMode Placement
        {
            //get { return (Placement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(nameof(FlyoutPlacementMode), typeof(Placement), typeof(NumberPicker), new PropertyMetadata(FlyoutPlacementMode.Left, new PropertyChangedCallback(PlacementOnChang)));
        private static void PlacementOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumberPicker Con = (NumberPicker)sender;

            FlyoutPlacementMode fpm = (FlyoutPlacementMode)e.NewValue;
 
            switch (fpm)
            {
                case FlyoutPlacementMode.Top:
                    Con.flyout.Placement = FlyoutPlacementMode.Top;
                    Con.flyout.FlyoutPresenterStyle = (Style)Con.Resources["FlyoutPresenterTop"];
                    break;
                case FlyoutPlacementMode.Bottom:
                    Con.flyout.Placement = FlyoutPlacementMode.Bottom;
                    Con.flyout.FlyoutPresenterStyle = (Style)Con.Resources["FlyoutPresenterBottom"];
                    break;
                case FlyoutPlacementMode.Left:
                    Con.flyout.Placement = FlyoutPlacementMode.Left;
                    Con.flyout.FlyoutPresenterStyle = (Style)Con.Resources["FlyoutPresenterLeft"];
                    break;
                case FlyoutPlacementMode.Right:
                    Con.flyout.Placement = FlyoutPlacementMode.Right;
                    Con.flyout.FlyoutPresenterStyle = (Style)Con.Resources["FlyoutPresenterRight"];
                    break;
              //  case FlyoutPlacementMode.Full:
                 //   break;
                default:
                    break;
            }
        }
        #endregion


        public NumberPicker()
        {
            this.InitializeComponent();
            //初始化          
            button.Content = Value.ToString() + text;

            flyout.FlyoutPresenterStyle =(Style) this.Resources["FlyoutPresenterLeft"];
 
            Number = DefaultValue; //修改新Value
            Change();
        }


        #region Control：控件事件
         
        private void button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            NumberCache = Number; //初始化时获取临时Value
            Change();

            IsClick = false; //刷新，现在是未点击状态
            Number = 0;
        }

        private void flyout_Opened(object sender, object e)
        {

        }
        private void flyout_Closed(object sender, object e)
        {
            if (IsClick == false) Number = NumberCache; //未点击原来该啥值就啥值，回复NumberCache
            Judge();
            Change();

            if (Number > Max) Number = Max;
            if (Number < Min) Number = Min;
            Value = Number;
            ValueChange?.Invoke(this, Number); //用户控件：Value改变
        }


        private void 滑条_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Number = (int)e.NewValue; //修改新Value
            button.Content = Number.ToString() + text;

            if (Number > Max) Number = Max;
            if (Number < Min) Number = Min;
            Value =Number;
            ValueChange?.Invoke(this, Number); //用户控件：Value改变
        }
        #endregion

        #region Number：数字按钮

        private void 零_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10;
            Change();
        }

        private void 一_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 1;
            Change();
        }

        private void 二_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 2;
            Change();
        }

        private void 三_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 3;
            Change();
        }

        private void 四_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 4;
            Change();
        }

        private void 五_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 5;
            Change();
        }

        private void 六_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 6;
            Change();
        }

        private void 七_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 7;
            Change();
        }


        private void 八_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 8;
            Change();
        }

        private void 九_Click(object sender, RoutedEventArgs e)
        {
            Number = Number * 10 + 9;
            Change();
        }

        private void 数点_Click(object sender, RoutedEventArgs e)
        {
            //暂无事件
        }

        private void 负号_Click(object sender, RoutedEventArgs e)
        {
            Number = -Number;
            Change();
        }

        private void 退格_Click(object sender, RoutedEventArgs e)
        {
            Number = Number / 10;
            Change();
        }

        private void 确定_Click(object sender, RoutedEventArgs e)
        {
            flyout.Hide();

            if (Number > Max) Number = Max;
            if (Number <Min) Number = Min;
            Value = Number;
            ValueChange?.Invoke(this, Number); //用户控件：Value改变
        }

        private void 取消_Click(object sender, RoutedEventArgs e)
        {
            Number = NumberCache;
            flyout.Hide();
        }

        #endregion



        private void Judge() //判断临时Value来决定要不要ChangeNumber
        {
            //Number变为临时button变量
            if (Number >= MaxValue)
            {
                Number = MaxValue;
            }
            if (Number <= MinValue)
            {
                Number = MinValue;
            }
        }

        private void Change() //button跟随Nmuber
        {
            //Value跟随
            button.Content = Number.ToString() + text;

            //刷新点击状态，为true
            IsClick = true;
        }


        public void Folloe(int Num)//外界向内传值的事件
        {
            Number = Num;
            Change();
        }

        
    }
}
