using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using 修图.Library;
using 修图.Model;

namespace 修图.Picker
{
    public sealed partial class ColorPicker : UserControl
    {
        SolidColorBrush White = new SolidColorBrush(Colors.White);
        SolidColorBrush Gray = new SolidColorBrush(Colors.Gray);

        int Mode=1;


        //Delegate
        public delegate void ColorChangedHandler(Color Color, SolidColorBrush Brush);
        public event ColorChangedHandler ColorChanged;

        public delegate void StrawChangedHandler( );
        public event StrawChangedHandler StrawChanged;

        #region DependencyProperty：依赖属性

        //刷新        
        private bool isstraw;
        public bool isStraw
        {
            get { return isstraw; }
            set { SetValue(isStrawProperty, value); }
        }

        public static readonly DependencyProperty isStrawProperty =
            DependencyProperty.Register("isStraw", typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, new PropertyChangedCallback(isStrawOnChang)));

        private static void isStrawOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker Con = (ColorPicker)sender;

            Con.StrawButton.IsEnabled =(bool) e.NewValue;
        }

        SolidColorBrush  ColorBrush = new SolidColorBrush(Colors.Gray);

        //刷新        
        private Color color;
        public Color Color
        {
            get { return color; }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Gray, new PropertyChangedCallback(ColorOnChang)));

        private static void ColorOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker Con = (ColorPicker)sender;

            Color coo = (Color)e.NewValue;
                Con.color = coo;

            Con.ColorBrush.Color = coo;
                Con.Ellipse.Fill = Con.ColorBrush;

            Con.Opacity = coo.A;
        }

        #endregion


        public ColorPicker()
        {
            this.InitializeComponent();
            this.DataContext = App.Model;

            FollowRGB();
            FollowHSL();
            FollowCopy();

            //色环
            赋形(180, 180); //一开始180*180大小
            圆环赋色();
            矩形赋色();//根据色相
            色相变();
            饱和度变();
            亮度变();
        }
        

        #region Global：全局


        private void StrawButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StrawChanged?.Invoke();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 0
            if (Mode == 0)
            {
                ListGrid.Visibility = Visibility.Visible;
                ListButton.IsChecked = true;
             }
            else
            {
                ListGrid.Visibility = Visibility.Collapsed;
                ListButton.IsChecked = false;
            }

            // 1
            if (Mode == 1)
            {
                CopyGrid.Visibility = Visibility.Visible;
                CopyButton.IsChecked = true;
            }
            else
            {
                CopyGrid.Visibility = Visibility.Collapsed;
                CopyButton.IsChecked = false;
            }

            // 2
            if (Mode == 2)
            {
                RGBGrid.Visibility = Visibility.Visible;
                RGBButton.IsChecked = true;
            }
            else
            {
                RGBGrid.Visibility = Visibility.Collapsed;
                RGBButton.IsChecked = false;
            }

            // 3
            if (Mode == 3)
            {
                HSLGrid.Visibility = Visibility.Visible;
                HSLButton.IsChecked = true;
            }
            else
            {
                HSLGrid.Visibility = Visibility.Collapsed;
                HSLButton.IsChecked = false;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;

            // 0
            if (tb == ListButton)
            {
                ListButton.IsChecked = true;
                ListGrid.Visibility = Visibility.Visible;
                Mode = 0;
            }
            else
            {
                ListButton.IsChecked = false;
                ListGrid.Visibility = Visibility.Collapsed;
            }

            // 1
            if (tb == CopyButton)
            {
                if (Mode != 1)
                {
                    色相变();
                    饱和度变();
                    亮度变();
                }

                CopyButton.IsChecked = true;
                CopyGrid.Visibility = Visibility.Visible;
                Mode = 1;
            }
            else
            {
                CopyButton.IsChecked = false;
                CopyGrid.Visibility = Visibility.Collapsed;
            }

            // 2
            if (tb == RGBButton)
            {
                if (Mode != 2) FollowRGB();

                RGBButton.IsChecked = true;
                RGBGrid.Visibility = Visibility.Visible;
                Mode = 2;
            }
            else
            {
                RGBButton.IsChecked = false;
                RGBGrid.Visibility = Visibility.Collapsed;
            }

            // 3
            if (tb == HSLButton)
            {
                if (Mode != 3)  FollowHSL(false); 

                HSLButton.IsChecked = true;
                HSLGrid.Visibility = Visibility.Visible;
                Mode = 3;
            }
            else
            {
                HSLButton.IsChecked = false;
                HSLGrid.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Opacity：透明度

       

        int Opacity=255;
        private void OpacityNumberPicker_ValueChange(object sender, int Value)
        {
            Opacity = Value;
            OpacitySlider.Value = Opacity;

            ColorBrush.Color = Color.FromArgb((byte)Opacity, Color.R, Color.G, Color.B);
            Ellipse.Fill = ColorBrush;
        }
         private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Opacity = (int)e.NewValue;
            OpacityNumberPicker.Value = Opacity;

            ColorBrush.Color = Color.FromArgb((byte)Opacity, Color.R, Color.G, Color.B);
            Ellipse.Fill = ColorBrush;
        }


        //颜色于字符串转换
        private Color StringToColor(string s)
        {
            //转化为八进制颜色字符串（原理我也不知道，网上抄的）
            int rgb = int.Parse(s, System.Globalization.NumberStyles.HexNumber);
            byte r = (byte)((rgb >> 16) & 0xff);
            byte g = (byte)((rgb >> 8) & 0xff);
            byte b = (byte)((rgb >> 0) & 0xff);

            return Color.FromArgb(255, r, g, b);
        }
        private string ColorToString(Color c)
        {
            return c.R.ToString("x2") + c.G.ToString("x2") + c.B.ToString("x2").ToString();
        }




        private void Flyout_Opened(object sender, object e)
        {
            TextBox.Text = ColorToString(Color);

            Opacity = Color.A;
            OpacityNumberPicker.Value = Opacity;
            OpacitySlider.Value = Opacity;
        }

        private void Flyout_Closed(object sender, object e)
        {
            var c = Color;
            try
            {
                c = StringToColor(TextBox.Text);
            }
            catch (Exception)
            {
            }
            c.A = (byte)Opacity;
            Color = c;
            ColorChanged?.Invoke(Color.FromArgb(color.A, color.R, color.G, color.B), ColorBrush);
        }


        #endregion


        #region List：色块


        //ListColorBrushObservableCollection：LCBOC
        public static ObservableCollection<ColorBrushCode> LCBOC = new ObservableCollection<ColorBrushCode>()
        {
            new ColorBrushCode { Color = Color.FromArgb(255, 0, 0, 0) },
            new ColorBrushCode { Color = Color.FromArgb(255, 64, 64, 64) },
            new ColorBrushCode { Color = Color.FromArgb(255, 191, 191, 191) },
            new ColorBrushCode { Color = Color.FromArgb(255, 255, 255, 255) },

            new ColorBrushCode { Color = Color.FromArgb(255, 195, 220, 239) },
            new ColorBrushCode { Color = Color.FromArgb(255, 146, 190, 226) },
            new ColorBrushCode { Color = Color.FromArgb(255, 33, 133, 216) },
            new ColorBrushCode { Color = Color.FromArgb(255, 30, 85, 181) },

            new ColorBrushCode { Color = Color.FromArgb(255, 19, 54, 136) },
            new ColorBrushCode { Color = Color.FromArgb(255, 31, 52, 80) },
            new ColorBrushCode { Color = Color.FromArgb(255, 19, 32, 64) },
            new ColorBrushCode { Color = Color.FromArgb(255, 239, 220, 211) },

            new ColorBrushCode { Color = Color.FromArgb(255, 244, 153, 153) },
            new ColorBrushCode { Color = Color.FromArgb(255, 231, 98, 98) },
            new ColorBrushCode { Color = Color.FromArgb(255, 170, 67, 78) },
            new ColorBrushCode { Color = Color.FromArgb(255, 146, 41, 42) },

            new ColorBrushCode { Color = Color.FromArgb(255, 106, 72, 73) },
            new ColorBrushCode { Color = Color.FromArgb(255, 52, 34, 30) },
            new ColorBrushCode { Color = Color.FromArgb(255, 67, 87, 114) },
            new ColorBrushCode { Color = Color.FromArgb(255, 44, 163, 168) },

            new ColorBrushCode { Color = Color.FromArgb(255, 84, 154, 119) },
            new ColorBrushCode { Color = Color.FromArgb(255, 254, 170, 57) },
            new ColorBrushCode { Color = Color.FromArgb(255, 253, 96, 64) },
            new ColorBrushCode { Color = Color.FromArgb(255, 207, 33, 87) },
           };


        //列图
        private void ListGridView_Loaded(object sender, RoutedEventArgs e)
        {
            ListGridView.ItemsSource = LCBOC;//绑定 
        }
        private void ListGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ColorBrushCode Code = (ColorBrushCode)e.ClickedItem;
            if (Color!= Code.Color)
            {
                Color = Code.Color;
                ColorChanged?.Invoke(Color.FromArgb(color.A, color.R, color.G, color.B), ColorBrush);

                //RGB
                //FollowHSL();
                //FollowRGB();
                // FollowCopy();
                // 色相变();
                //饱和度变();
                //亮度变();
            }
         } 



        //加减
        private void AddButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DeleteButton.IsEnabled = true;

            LCBOC.Add(new ColorBrushCode { Color = Color });
        }
        private async void DeleteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ListGridView.SelectedIndex < 0) ListGridView.SelectedIndex = 0;

            if (LCBOC.Count > 1)
            {
                //索引
                int Index = ListGridView.SelectedIndex;
                Color c = LCBOC[Index].Color;
                if (Index < 0) Index = 0;
                LCBOC[Index].Color = Color.FromArgb(0, 0, 0, 0);
                await Task.Delay(200);

                //删除
                LCBOC.RemoveAt(Index);

                //删除后改变索引
                if (LCBOC.Count == 0)
                    DeleteButton.IsEnabled = false;
                else if (LCBOC.Count == 1)
                    ListGridView.SelectedIndex = 0;
                else if (LCBOC.Count == 2)
                    ListGridView.SelectedIndex = 1;
                else
                {
                    if (Index > LCBOC.Count)
                        ListGridView.SelectedIndex = LCBOC.Count - 1;
                    else
                        ListGridView.SelectedIndex = Index - 1 >= 0 ? Index - 1 : 0;
                }
            }
            else
            {
                DeleteButton.IsEnabled = false;
            }
        }


        #endregion


        #region Copy：色环

        int 圆直径 = 180; //圆的直径
        int 圆边距 = 2; //圆环的边距
        int 矩边长 = 120; //矩形的宽高

        double 横位移;//起始位置
        double 纵位移;//起始位置
        double 横变化;//变化位置
        double 纵变化;//变化位置
        bool 是否拖动 = false;//自身是否拖动




        private void 赋形(int 宽度, int 高度)
        {
            //《计算》
            //网格的宽和高，谁比较小谁就是圆的直径
            if (高度 <= 宽度) 圆直径 = 高度;
            else 圆直径 = 宽度;
            //圆环宽1/30
            圆边距 = 2;// 圆直径 / 30;
            //宽高为圆环的两倍差距
            矩边长 = (int)((圆直径 / 2 - 2 * 圆边距) * 1.414);

            //《赋形》
            //圆网格
            圆网格.Width = 圆网格.Height = 圆直径;
            //圆环
            圆.Width = 圆.Height = 圆直径;
            圆.StrokeThickness = 圆边距;
            圆触.Width = 圆触.Height = 圆直径;

            //矩网格
            矩网格.Width = 矩网格.Height = 矩边长;
            //矩形
            矩.Width = 矩.Height = 矩边长;
            矩触.Width = 矩触.Height = 矩边长;
        }








        private void 圆环赋色()
        {
            Color[,] color = new Color[圆直径, 圆直径]; //数组格式为 [y, x]，与位图[x , y]相反

            for (int y = 0; y < 圆直径; y++)
            {
                for (int x = 0; x < 圆直径; x++)
                {
                    color[y, x] = Method.HSLtoRGB(((Math.Atan2(y - 圆直径 / 2, x - 圆直径 / 2) * 180.0 / Math.PI) + 450) % 360); //调用：获取颜色
                }
            }

            圆环.ImageSource =Library.Image.ColortoBitmap(color); // 赋予笔刷
        }
         
        private void 矩形赋色()
        {
              Color[,]  矩形图片 = new Color[矩边长, 矩边长]; //数组格式为 [y, x]，与位图[x , y]相反

            double ss; //饱和度（1~2）
            double ll; //亮度（0~1）

            for (int y = 0; y < 矩边长; y++)
            {
                for (int x = 0; x < 矩边长; x++)
                {
                    ss = (double)x / (double)矩边长;
                    ll = (1.0d - (double)y / (double)矩边长);
                    矩形图片[y, x] = Method.HSLtoRGB(Opacity, H, ss, ll); //调用：获取颜色
                }
            }

            矩形.ImageSource =  Library.Image.ColortoBitmap(矩形图片); // 赋予笔刷
        }









        //控件拖拽


        //《圆形》
        private void 圆拉扯(double 横, double 纵) //根据角度改变位置方法
        {
            //《原文》
            //double angle = Math.Atan2(y - 圆直径 / 2, x - 圆直径 / 2);  //计算点的所在Pi角度
            //double Degrees = angle * 180.0 / Math.PI;  //计算点的所在360角度
            //H= (Degrees + 450) % 360;  //偏转加取值

            //《精简》（看不懂就看上面的原文）:计算360角度
            H = ((Math.Atan2(纵 - 圆直径 / 2, 横 - 圆直径 / 2) * 180.0 / Math.PI) + 450) % 360; //偏转加取值
            矩形赋色();//一.《形形色色》：根据色相

            //计算X轴Y轴距离       
            横 = (圆直径 - 圆边距) / 2 * Math.Sin(H / 180 * Math.PI);
            纵 = (圆直径 - 圆边距) / 2 * -Math.Cos(H / 180 * Math.PI);

            //改变位置
            圆指.Margin = new Thickness(圆直径 / 2 + 横 - 圆指.Width / 2, 圆直径 / 2 + 纵 - 圆指.Height / 2, 0, 0);



            var left = 矩指.Margin.Left - 10;
            if (left < 0) S = 0;
            else if (left > 矩边长 - 30) S = 1;
            else S = (left + 矩指.Width / 2) / (double)矩边长;

            var top = 矩指.Margin.Top - 10;
            if (top < 0) L = 1;
            else if (top > 矩边长 - 30) L = 0;
            else L = 1.0d - ((top + 矩指.Height / 2) / (double)矩边长);



            //HSL
            Hi = (int)H;
            HSlider.Value = Hi;
            HNumberPicker.Value = Hi;
             Color = Method.HSLtoRGB(Opacity, H, S, L);
            ColorChanged?.Invoke(Color, ColorBrush);
            FollowRGB(false, false, false);
        }


        bool 是否圆开始;
        private void 圆触_DragStarted(object sender, DragStartedEventArgs e)
        {
            //判断
            是否圆开始 = true; // 开启《初始执行的事件》
            是否拖动 = true; //自身是否拖动

            //起始时，点击的位置就是圆点的位置
            圆拉扯(e.HorizontalOffset, e.VerticalOffset);

            //起始时，点击的位置就是横位移和纵位移的位置
            横位移 = e.HorizontalOffset;
            纵位移 = e.VerticalOffset;
        }
        private void 圆触_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //《初始执行的事件》
            if (是否圆开始)//从开始到变化的过程中执行一次
            {
                横变化 = 横位移; //把起始时的位置赋予到变化中，横变化（实时变化） +横位移（初始位置）才是真正的位置
                纵变化 = 纵位移;
            }
            是否圆开始 = false;//关掉《初始执行的事件》

            横变化 += e.HorizontalChange;
            纵变化 += e.VerticalChange;

            圆拉扯(横变化, 纵变化);//二.《拉拉扯扯》
        }
        private void 圆触_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            是否拖动 = false; //自身是否拖动           
        }





        //《矩形》
        private void 矩拉扯(double 横, double 纵) //2.根据位置确定不超过边界
        {
            double x, y;

            //确定不超过横边界
            if (横 < 0) x = 0;
            else if (横 > 矩边长) x = 矩边长;
            else x = 横;

            //确定不超过纵边界
            if (纵 < 0) y = 0;
            else if (纵 > 矩边长) y = 矩边长;
            else y = 纵;

            矩指.Margin = new Thickness(x - 矩指.Width / 2, y - 矩指.Height / 2, 0, 0);


            var left = 矩指.Margin.Left-10;
            if (left < 0) S = 0;
            else if (left > 矩边长 - 30) S = 1;
            else S = (left + 矩指.Width / 2) / (double)矩边长;

            var top = 矩指.Margin.Top-10;
            if (top < 0) L = 1;
            else if (top > 矩边长 - 30) L = 0;
            else L = 1.0d - ((top + 矩指.Height / 2) / (double)矩边长);

            //HSL
            Si = (int)(S*100); 

            Li = (int)(L * 100); 

             Color = Method.HSLtoRGB(Opacity, H, S, L);
            ColorChanged?.Invoke(Color, ColorBrush);
         }

        bool 是否矩开始;
        private void 矩触_DragStarted(object sender, DragStartedEventArgs e)
        {
            //判断
            是否矩开始 = true; // 开启《初始执行的事件》
            是否拖动 = true; //自身是否拖动

            //起始时，点击的位置就是圆点的位置
            矩拉扯(e.HorizontalOffset, e.VerticalOffset);//二.《拉拉扯扯》

            if (e.HorizontalOffset >= 0 && e.HorizontalOffset <= 矩边长 && e.VerticalOffset >= 0 && e.VerticalOffset <= 矩边长)
                矩指.Margin = new Thickness(e.HorizontalOffset - 矩指.Width / 2, e.VerticalOffset - 矩指.Height / 2, 0, 0);

            //起始时，点击的位置就是横位移和纵位移的位置
            横位移 = e.HorizontalOffset;
            纵位移 = e.VerticalOffset;
        }
        private void 矩触_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //《初始执行的事件》
            if (是否矩开始)//从开始到变化的过程中执行一次
            {
                //把起始时的位置赋予到变化中，横变化（实时变化） +横位移（初始位置）才是真正的位置
                横变化 = 横位移;
                纵变化 = 纵位移;
            }
            是否矩开始 = false;//关掉《初始执行的事件》

            横变化 = 横变化 + e.HorizontalChange; //实时横变化
            纵变化 = 纵变化 + e.VerticalChange;//实时纵变化

            矩拉扯(横变化, 纵变化);//二.《拉拉扯扯》
        }
        private void 矩触_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            是否拖动 = false; //自身是否拖动
        }








        //方法


        private void 色相变()//根据HSL改变位置（给依赖属性的在改变事件用的，需要判断）
        {
            if (是否拖动 == false)//自己没有拖动
            {
                矩形赋色();
                圆指.Margin = new Thickness(圆直径 / 2 + (圆直径 - 圆边距) / 2 * Math.Sin(H / 180 * Math.PI) - 圆指.Width / 2, 圆直径 / 2 + (圆直径 - 圆边距) / 2 * -Math.Cos(H / 180 * Math.PI) - 圆指.Height / 2, 0, 0);
            }
        }

        private void 饱和度变()//根据HSL改变位置（给依赖属性的在改变事件用的，需要判断）
        {
            if (是否拖动 == false)//自己没有拖动
                矩指.Margin = new Thickness(S * (double)矩边长 - 矩指.Width / 2, 矩指.Margin.Top, 0, 0);
        }

        private void 亮度变()//根据HSL改变位置（给依赖属性的在改变事件用的，需要判断）
        {
            if (是否拖动 == false)//自己没有拖动
                矩指.Margin = new Thickness(矩指.Margin.Left, ((1.0d - L) * (double)矩边长) - 矩指.Height / 2, 0, 0);
        }




        #endregion


        #region Copy：参数


        //RGB
        private void RNumberPicker_ValueChange(object sender, int Value)
        {
            color.R = (byte)Value;
             Color = color;
             ColorChanged?.Invoke(Color, ColorBrush);

            FollowRGB();
            FollowHSL();
            FollowCopy();
        }
        private void GNumberPicker_ValueChange(object sender, int Value)
        {
            color.G = (byte)Value;
             Color = color;
            ColorChanged?.Invoke(Color, ColorBrush);

            FollowRGB();
            FollowHSL();
            FollowCopy();
        }
        private void BNumberPicker_ValueChange(object sender, int Value)
        {
            color.B = (byte)Value;
             Color = color;
            ColorChanged?.Invoke(Color, ColorBrush);

            FollowRGB();
            FollowHSL();
            FollowCopy();
        }







        private void FollowCopy()
        {
            RNumberPicker.Value = Color.R;
            GNumberPicker.Value = Color.G;
            BNumberPicker.Value = Color.B;

            HNumberPicker.Value = Hi;
            SNumberPicker.Value = Si;
            LNumberPicker.Value = Li;
        }

        #endregion


        #region RGB：红绿蓝


        //事件 
        private void RSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            GSlider.ValueChanged -= GSlider_ValueChanged;
            BSlider.ValueChanged -= BSlider_ValueChanged;

            HSlider.ValueChanged -= HSlider_ValueChanged;
            SSlider.ValueChanged -= SSlider_ValueChanged;
            LSlider.ValueChanged -= LSlider_ValueChanged;

            //事件
            color.R = (byte)e.NewValue;
              Color = color;
            ColorChanged?.Invoke(Color.FromArgb(color.A, color.R, color.G, color.B), ColorBrush);
            FollowRGB(false, true, true);
            FollowHSL();
            FollowCopy();

            GSlider.ValueChanged += GSlider_ValueChanged;
            BSlider.ValueChanged += BSlider_ValueChanged;

            HSlider.ValueChanged += HSlider_ValueChanged;
            SSlider.ValueChanged += SSlider_ValueChanged;
            LSlider.ValueChanged += LSlider_ValueChanged;

            //  Copy
            色相变();
            饱和度变();
            亮度变();
        }
        private void GSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            RSlider.ValueChanged -= RSlider_ValueChanged;
            BSlider.ValueChanged -= BSlider_ValueChanged;

            HSlider.ValueChanged -= HSlider_ValueChanged;
            SSlider.ValueChanged -= SSlider_ValueChanged;
            LSlider.ValueChanged -= LSlider_ValueChanged;

            //事件
            color.G = (byte)e.NewValue;
             Color = color;
            ColorChanged?.Invoke(Color.FromArgb(color.A, color.R, color.G, color.B), ColorBrush);
            FollowRGB(true, false, true);
            FollowHSL();
            FollowCopy();

            RSlider.ValueChanged += RSlider_ValueChanged;
            BSlider.ValueChanged += BSlider_ValueChanged;

            HSlider.ValueChanged += HSlider_ValueChanged;
            SSlider.ValueChanged += SSlider_ValueChanged;
            LSlider.ValueChanged += LSlider_ValueChanged;

            //  Copy
            色相变();
            饱和度变();
            亮度变();
        }
        private void BSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            RSlider.ValueChanged -= RSlider_ValueChanged;
            GSlider.ValueChanged -= GSlider_ValueChanged;

            HSlider.ValueChanged -= HSlider_ValueChanged;
            SSlider.ValueChanged -= SSlider_ValueChanged;
            LSlider.ValueChanged -= LSlider_ValueChanged;

            //事件       
            color.B= (byte)e.NewValue;
             Color = color;
            ColorChanged?.Invoke(Color.FromArgb(color.A, color.R, color.G, color.B), ColorBrush);
            FollowRGB(true, true, false);
            FollowHSL();
            FollowCopy();

            RSlider.ValueChanged += RSlider_ValueChanged;
            GSlider.ValueChanged += GSlider_ValueChanged;

            HSlider.ValueChanged += HSlider_ValueChanged;
            SSlider.ValueChanged += SSlider_ValueChanged;
            LSlider.ValueChanged += LSlider_ValueChanged;

            //  Copy
            色相变();
            饱和度变();
            亮度变();
        }

        


        //方法
        private void FollowRGB(bool isR = true, bool isG = true, bool isB = true)
        {
            //R
            if (isR) RSlider.Value = color.R;

            RNumberPicker.Value = color.R;
            RLeft.Color = Color.FromArgb(255, 0, color.G, color.B);
            RRight.Color = Color.FromArgb(255, 255, color.G, color.B);


            //G
            if (isG) GSlider.Value = color.G;

            GNumberPicker.Value = color.G;
            GLeft.Color = Color.FromArgb(255, color.R, 0, color.B);
            GRight.Color = Color.FromArgb(255, color.R, 255, color.B);


            //B
            if (isB) BSlider.Value = color.B;

            BNumberPicker.Value = color.B;
            BLeft.Color = Color.FromArgb(255, color.R, color.G, 0);
            BRight.Color = Color.FromArgb(255, color.R, color.G, 255);
        }



        #endregion


        #region HSL：色相饱和度明度
        //事件

        private void HSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {  
            SSlider.ValueChanged -= SSlider_ValueChanged;
            LSlider.ValueChanged -= LSlider_ValueChanged;

            //参数
            H = e.NewValue;
            Hi = (int)H;
            //跟随
              Color = Method.HSLtoRGB(Opacity, H, S, L);
            ColorChanged?.Invoke(Color, ColorBrush);
            FollowHSL(false);

            SSlider.ValueChanged += SSlider_ValueChanged;
            LSlider.ValueChanged += LSlider_ValueChanged;
             
        }

        private void SSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        { 
            HSlider.ValueChanged -= HSlider_ValueChanged;
            LSlider.ValueChanged -= LSlider_ValueChanged;

            //参数
            S = e.NewValue / 100;
            Si = (int)(S * 100);
            //跟随
              Color = Method.HSLtoRGB(Opacity, H, S, L);
            ColorChanged?.Invoke(Color, ColorBrush);
            FollowHSL(false);

            HSlider.ValueChanged += HSlider_ValueChanged;
            LSlider.ValueChanged += LSlider_ValueChanged;
         }
        private void LSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        { 
            HSlider.ValueChanged -= HSlider_ValueChanged;
            SSlider.ValueChanged -= SSlider_ValueChanged;

            //参数
            L = e.NewValue / 100;
            Li = (int)(L * 100);
            //跟随
            Color = Method.HSLtoRGB(Opacity, H, S, L);
            ColorChanged?.Invoke(Color, ColorBrush);
            FollowHSL(false);

            HSlider.ValueChanged += HSlider_ValueChanged;
            SSlider.ValueChanged += SSlider_ValueChanged;
        }








        //HSl
        private void HNumberPicker_ValueChange(object sender, int Value)
        {
            HSlider.ValueChanged -= HSlider_ValueChanged;
         
            HSlider.Value = H = Value;
            Color = Method.HSLtoRGB(Opacity, H, S, L);
            ColorChanged?.Invoke(Color, ColorBrush);

            ChangeHSL();

            HSlider.ValueChanged += HSlider_ValueChanged;
        }
        private void SNumberPicker_ValueChange(object sender, int Value)
        {
            SSlider.ValueChanged -= SSlider_ValueChanged;

           double s = SSlider.Value = Value;
            S = s / 100;
            Color = Method.HSLtoRGB(Opacity, H, S, L);
            ColorChanged?.Invoke(Color, ColorBrush);

            ChangeHSL();

            SSlider.ValueChanged += SSlider_ValueChanged;
        }
        private void LNumberPicker_ValueChange(object sender, int Value)
        { 
            LSlider.ValueChanged -= LSlider_ValueChanged;
    
            double l = LSlider.Value = Value;
            L = l / 100;
            Color = Method.HSLtoRGB(Opacity, H, S, L);
            ColorChanged?.Invoke(Color, ColorBrush);

            ChangeHSL();

            LSlider.ValueChanged += LSlider_ValueChanged;
         }







        //double变量
        double H;//（0~360）
        double S;//（0~1）
        double L;//（0~1）

        //int变量
        int Hi;//（0~360）
        int Si;//（0~100）
        int Li;//（0~100）

        private void FollowHSL(bool isHSL = true)
        {
            HSlider.ValueChanged -= HSlider_ValueChanged;
            SSlider.ValueChanged -= SSlider_ValueChanged;
            LSlider.ValueChanged -= LSlider_ValueChanged;
            HNumberPicker.ValueChange -= HNumberPicker_ValueChange;
            SNumberPicker.ValueChange -= SNumberPicker_ValueChange;
            LNumberPicker.ValueChange -= LNumberPicker_ValueChange;

            if (isHSL)
            {
                var hsl = Method.RGBtoHSL(Color);

                //H
                H = hsl.H;
                Hi = (int)H;
                HSlider.Value = Hi;

                //S
                S = hsl.S;
                Si = (int)(S * 100);
                SSlider.Value = Si;

                //L
                L = hsl.L;
                Li = (int)(L * 100);
                LSlider.Value = Li;
            }

            //H          
             HNumberPicker.Value = Hi;
            HG.Color = HA.Color = Method.HSLtoRGB(255, 0, S, L);
            HB.Color = Method.HSLtoRGB(255, 60, S, L);
            HC.Color = Method.HSLtoRGB(255, 120, S, L);
            HD.Color = Method.HSLtoRGB(255, 180, S, L);
            HE.Color = Method.HSLtoRGB(255, 240, S, L);
            HF.Color = Method.HSLtoRGB(255, 300, S, L);
 
            //S
            SNumberPicker.Value = Si;
            SLeft.Color = Method.HSLtoRGB(255, H, 0.0d, L);
            SRight.Color = Method.HSLtoRGB(255, H, 1.0d, L);

            //L
            LNumberPicker.Value = Li;
            LLeft.Color = Method.HSLtoRGB(255, H, S, 0.0d);
            LRight.Color = Method.HSLtoRGB(255, H, S, 1.0d);


            HSlider.ValueChanged += HSlider_ValueChanged;
            SSlider.ValueChanged += SSlider_ValueChanged;
            LSlider.ValueChanged += LSlider_ValueChanged;
            HNumberPicker.ValueChange += HNumberPicker_ValueChange;
            SNumberPicker.ValueChange += SNumberPicker_ValueChange;
            LNumberPicker.ValueChange += LNumberPicker_ValueChange;
        }


        private void ChangeHSL()
        {
             //H
            HG.Color = HA.Color = Method.HSLtoRGB(255, 0, S, L);
            HB.Color = Method.HSLtoRGB(255, 60, S, L);
            HC.Color = Method.HSLtoRGB(255, 120, S, L);
            HD.Color = Method.HSLtoRGB(255, 180, S, L);
            HE.Color = Method.HSLtoRGB(255, 240, S, L);
            HF.Color = Method.HSLtoRGB(255, 300, S, L);
            //S
            SLeft.Color = Method.HSLtoRGB(255, H, 0.0d, L);
            SRight.Color = Method.HSLtoRGB(255, H, 1.0d, L);
            //L
            LLeft.Color = Method.HSLtoRGB(255, H, S, 0.0d);
            LRight.Color = Method.HSLtoRGB(255, H, S, 1.0d);
        }








        #endregion




    }
}
